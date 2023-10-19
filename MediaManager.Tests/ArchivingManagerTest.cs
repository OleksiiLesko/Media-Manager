using MediaManager.ArchivingEventManager;
using MediaManager.Common;
using MediaManager.Domain.DTOs;
using MediaManager.RabbitMQClient;
using MediaManager.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaManager.Tests
{
    public class ArchivingManagerTest
    {
        private readonly ArchivingManager _archivingManager;
        private readonly Mock<ILogger<ArchivingManager>> _loggerMock;
        private readonly Mock<IFileManager> _fileManagerMock;
        private readonly Mock<IRepository> _repositoryMock;
        private readonly Mock<IRabbitMQService> _rabbitMQServiceMock;
        private readonly CallEvent _callEventFailure;
        private readonly CallEvent _callEventPartial;
        private readonly CallEvent _callEventSuccess;

        public ArchivingManagerTest()
        {
            _loggerMock = new Mock<ILogger<ArchivingManager>>();
            _fileManagerMock = new Mock<IFileManager>();
            _repositoryMock = new Mock<IRepository>();
            _rabbitMQServiceMock = new Mock<IRabbitMQService>();
            _archivingManager = new ArchivingManager(
                _loggerMock.Object,
                _fileManagerMock.Object,
                _repositoryMock.Object,
                _rabbitMQServiceMock.Object);
            _callEventFailure = CreateCallEvent(1, ArchivingStatus.FailedToArchive, ArchivingStatus.FailedToArchive);
            _callEventPartial = CreateCallEvent(1, ArchivingStatus.Archived, ArchivingStatus.FailedToArchive);
            _callEventSuccess = CreateCallEvent(1, ArchivingStatus.Archived, ArchivingStatus.Archived);
        }
        [Fact]
        public void HandleArchivingEvent_Success()
        {

            _fileManagerMock.Setup(fm => fm.ArchiveCallEventAsync(It.IsAny<CallEvent>()));
            _repositoryMock.Setup(repo => repo.GetRecordingsArchivingStatuses(It.IsAny<CallEvent>())).Returns(_callEventSuccess.Recordings);

            var result = _archivingManager.HandleArchivingEvent(_callEventSuccess);

            _fileManagerMock.Verify(fm => fm.ArchiveCallEventAsync(_callEventSuccess), Times.Once);

            _repositoryMock.Verify(r => r.GetRecordingsArchivingStatuses(_callEventSuccess), Times.Once);

            var archivingEvent = new CallArchivingEvent(_callEventSuccess);
            var routingKey = $"archiving-event.{archivingEvent.CallId}.{archivingEvent.ArchivingStatus.ToString().ToLower()}";

            _rabbitMQServiceMock.Verify(rmq => rmq.SendMessage(routingKey, It.IsAny<byte[]>()), Times.Once);

            Assert.Equal(ArchivingStatus.Success, _callEventSuccess.ArchivingStatus);
            Assert.NotNull(result);
        }
        [Fact]
        public void HandleArchivingEvent_ExceptionThrown()
        {
            _repositoryMock.Setup(repo => repo.GetRecordingsArchivingStatuses(_callEventSuccess))
                      .Throws(new Exception(""));

            var result = _archivingManager.HandleArchivingEvent(_callEventSuccess);

            Assert.Null(result);

        }
        [Fact]
        public void HandleArchivingEvent_PartialArchivingStatus()
        {
            _fileManagerMock.Setup(fm => fm.ArchiveCallEventAsync(It.IsAny<CallEvent>()));
            _repositoryMock.Setup(repo => repo.GetRecordingsArchivingStatuses(It.IsAny<CallEvent>())).Returns(_callEventPartial.Recordings);

            var result = _archivingManager.HandleArchivingEvent(_callEventPartial);

            Assert.Equal(ArchivingStatus.Partial, _callEventPartial.ArchivingStatus);
            Assert.NotNull(result);


        }
        [Fact]
        public void HandleArchivingEvent_FailureArchivingStatus()
        {
            _fileManagerMock.Setup(fm => fm.ArchiveCallEventAsync(It.IsAny<CallEvent>()));
            _repositoryMock.Setup(repo => repo.GetRecordingsArchivingStatuses(It.IsAny<CallEvent>())).Returns(_callEventFailure.Recordings);

            var result = _archivingManager.HandleArchivingEvent(_callEventFailure);

            Assert.Equal(ArchivingStatus.Failure, _callEventFailure.ArchivingStatus);
            Assert.NotNull(result);

        }
        private CallEvent CreateCallEvent(int callId, ArchivingStatus firstArchivingStatus, ArchivingStatus secondArchivingStatus)
        {
            return new CallEvent
            {
                CallId = callId,
                Recordings = new List<Recording>
        {
           new Recording
            {
                RecordingId = 3,
                RecorderId = 3,
                RecordingArchivingStatus = firstArchivingStatus
            },
            new Recording
            {
                RecordingId = 4,
                RecorderId = 4,
                RecordingArchivingStatus = secondArchivingStatus
            }
        }
            };
        }
    }
}
