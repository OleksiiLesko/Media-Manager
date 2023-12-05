using MediaManager.ArchivingRuleManager;
using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MediaManager.Tests
{
    public class ArchivingRuleManagerServiceTest
    {
        private readonly Mock<ILogger<ArchivingRuleManagerService>> _loggerMock;
        private readonly Mock<IOptionsMonitor<RulesSettings>> _rulesSettingsMock;
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private readonly ArchivingRuleManagerService _service;

        public ArchivingRuleManagerServiceTest()
        {
            _loggerMock = new Mock<ILogger<ArchivingRuleManagerService>>();
            _rulesSettingsMock = new Mock<IOptionsMonitor<RulesSettings>>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();

            var rulesSettingsInstance = new RulesSettings
            {
                MediaTypeArchivingRule = new MediaTypeArchivingRule
                {
                    Enabled = true,
                    MediaTypes = new List<MediaType> { MediaType.Voice }
                }
            };
            _rulesSettingsMock.Setup(x => x.CurrentValue).Returns(rulesSettingsInstance);
            _service = new ArchivingRuleManagerService(_loggerMock.Object,
                _rulesSettingsMock.Object,
                _loggerFactoryMock.Object);
        }
        [Fact]
        public void ApplyRules_WhenCalled_ReturnsFilteredCall()
        {
            var callEvent = new CallEvent
            {
                CallId = 1,
                Recordings = new List<Recording>
            {
                    new Recording { RecordingId = 1, MediaType = MediaType.Voice },
                    new Recording { RecordingId = 2, MediaType = MediaType.Screen },
                    new Recording { RecordingId = 3, MediaType = MediaType.Voice }
            }
            };
            var result = _service.ApplyRules(callEvent);

            Assert.True(result.Recordings.Count == 2);
        }
    }
}
