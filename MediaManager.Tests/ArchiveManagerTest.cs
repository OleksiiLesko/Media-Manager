using MediaManager.Common;
using MediaManager.Domain.DTOs;
using MediaManager.RabbitMQClient;
using MediaManager.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaManager.Tests
{
    public class ArchiveManagerTest
    {
        readonly Mock<IConfiguration> _configuration;
        private readonly Mock<ILogger<ArchiveManager>> _loggerMock;
        private readonly Mock<IRepository> _repositoryMock;
        public ArchiveManagerTest()
        {
            _configuration = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<ArchiveManager>>();
            _repositoryMock = new Mock<IRepository>();
        }
        [Fact]
        public async Task ArchiveCallEventAsync_CopiesRecordingsAndSetsArchivingStatus()
        {
            _configuration.Setup(c => c["ArchiveEventsPath"]).Returns("test-archive-path");

            _repositoryMock.Setup(r => r.SetArchivingStatus(It.IsAny<CallEvent>(), It.IsAny<string>(), It.IsAny<ArchivingStatus>()));

            var callEvent = new CallEvent
            {
                CallId = 1,
                CallStartTime = DateTime.UtcNow,
                CallEndTime = DateTime.UtcNow,
                Recordings = new List<Recording>
                {
                    new Recording
                    {
                        RecordedFilePath = "C:\\Users\\olesko\\source\\Recordings\\Recording1.txt"
                    }
                }
            };

            var archiveManager = new ArchiveManager(_configuration.Object, _loggerMock.Object, _repositoryMock.Object);

            await archiveManager.ArchiveCallEventAsync(callEvent);

            _repositoryMock.Verify(r => r.SetArchivingStatus(callEvent, It.IsAny<string>(), ArchivingStatus.Archived), Times.Once);
        }
        [Fact]
        public async Task ArchiveCallEventAsync_HandlesExceptionAndSetsFailedArchivingStatus()
        {
            _configuration.Setup(c => c["ArchiveEventsPath"]).Returns("test-archive-path");

            _repositoryMock.Setup(r => r.SetArchivingStatus(It.IsAny<CallEvent>(), It.IsAny<string>(), ArchivingStatus.FailedToArchive));

            var callEvent = new CallEvent
            {
                CallId = 1,
                CallStartTime = DateTime.UtcNow,
                CallEndTime = DateTime.UtcNow,
                Recordings = new List<Recording>
                {
                    new Recording
                    {
                        RecordedFilePath = "invalid-destination-path"
                    }
                }
            };

            var archiveManager = new ArchiveManager(_configuration.Object, _loggerMock.Object, _repositoryMock.Object);

            await archiveManager.ArchiveCallEventAsync(callEvent);

            _repositoryMock.Verify(r => r.SetArchivingStatus(callEvent, It.IsAny<string>(), ArchivingStatus.FailedToArchive), Times.Once);
        }
    }
}
