using MediaManager.Domain.DTOs;
using MediaManager.RabbitMQClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.IO.Abstractions;

namespace MediaManager.Tests
{
    public class ArchiveManagerTest
    {
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<ILogger<ArchiveManager>> _loggerMock;
        private readonly CallEvent _callEvent;
        public ArchiveManagerTest()
        {
            _configuration = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<ArchiveManager>>();
            _callEvent = new CallEvent();
        }
        [Fact]
        public async Task ArchiveCallEventAsync_SuccessfulCopy()
        {
            _configuration.Setup(x => x["ArchiveEventsPath"]).Returns("source_file_path");

            var archiveManager = new ArchiveManager(_configuration.Object, _loggerMock.Object);

            var callEvent = new CallEvent
            {
                CallId = 1,
                CallEndTime = DateTime.Now,
                Recordings = new List<Recording>
                {
                    new Recording
                    {
                        RecordedFilePath = "source_file_path"
                    }
                }
            };

            await archiveManager.ArchiveCallEventAsync(callEvent);

            _configuration.Verify(x => x["ArchiveEventsPath"], Times.Once);
        }
        [Fact]
        public async Task ArchiveCallEventAsync_CopyError()
        {
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["ArchiveEventsPath"]).Returns("test_archive_path");

            var archiveManager = new ArchiveManager(configurationMock.Object, _loggerMock.Object);

            await Assert.ThrowsAsync<NullReferenceException>(async () => await archiveManager.ArchiveCallEventAsync(_callEvent));
        }
    }
}
