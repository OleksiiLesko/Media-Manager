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
        private readonly Mock<IOptionsMonitor<MediaTypeRuleConfig>> _mediaTypeRuleConfigMock;
        private readonly Mock<IOptionsMonitor<CallDirectionRuleConfig>> _callDirectionRuleConfigMock;
        private readonly Mock<IOptionsMonitor<CallDurationRuleConfig>> _callDurationRuleConfigMock;
        private readonly Mock<IArchivingRuleFactory> _archivingRuleFactoryMock;
        private readonly Mock<IOperatorFactory> _operatorFactoryMock;
        private ArchivingRuleManagerService _service;

        public ArchivingRuleManagerServiceTest()
        {
            _loggerMock = new Mock<ILogger<ArchivingRuleManagerService>>();
            _mediaTypeRuleConfigMock = new Mock<IOptionsMonitor<MediaTypeRuleConfig>>();
            _callDirectionRuleConfigMock = new Mock<IOptionsMonitor<CallDirectionRuleConfig>>();
            _callDurationRuleConfigMock = new Mock<IOptionsMonitor<CallDurationRuleConfig>>();
            _archivingRuleFactoryMock = new Mock<IArchivingRuleFactory>();
            _operatorFactoryMock = new Mock<IOperatorFactory>();
        }
        [Fact]
        public void ApplyRules_ReturnsFilteredCall()
        {
            var mediaTypeRuleConfig = new MediaTypeRuleConfig
            {
                Enabled = false,
                StopOnFailure = false,
                MediaTypes = new List<MediaType> { MediaType.Voice }
            };

            var callDirectionRuleConfig = new CallDirectionRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                CallDirections = new List<CallDirection> { CallDirection.Incoming }
            };
            var callDurationRuleConfig = new CallDurationRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                ComparisonOperator = ">=",
                ParsedComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                CallDurationSeconds = 2700
            };

            SetRulesSettings(mediaTypeRuleConfig, callDirectionRuleConfig, callDurationRuleConfig);

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
        [Fact]
        public void ApplyRules_ShouldReturnCall_WhenUsedEnabledRules()
        {
            var mediaTypeRuleConfig = new MediaTypeRuleConfig
            {
                Enabled = false,
                StopOnFailure = false,
                MediaTypes = new List<MediaType> { MediaType.Voice }
            };

            var callDirectionRuleConfig = new CallDirectionRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                CallDirections = new List<CallDirection> { CallDirection.Incoming }
            };
            var callDurationRuleConfig = new CallDurationRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                ComparisonOperator = ">=",
                ParsedComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                CallDurationSeconds = 2700
            };

            SetRulesSettings(mediaTypeRuleConfig, callDirectionRuleConfig, callDurationRuleConfig);

            var callEvent = new CallEvent
            {
                CallId = 1,
                CallDirection = CallDirection.Incoming,
                Recordings = new List<Recording>
            {
                    new Recording { RecordingId = 1, MediaType = MediaType.Voice },
                    new Recording { RecordingId = 2, MediaType = MediaType.Screen },
                    new Recording { RecordingId = 3, MediaType = MediaType.Voice }
            }
            };
            var result = _service.ApplyRules(callEvent);

            Assert.Contains(result.Recordings, r => r.RecordingId == 2);
        }
        [Fact]
        public void ApplyRules_ShouldReturnNull_WhenUsedStopOnFailure()
        {
            var mediaTypeRuleConfig = new MediaTypeRuleConfig
            {
                Enabled = true,
                StopOnFailure = true,
                MediaTypes = new List<MediaType> { MediaType.Voice }
            };

            var callDirectionRuleConfig = new CallDirectionRuleConfig
            {
                Enabled = true,
                StopOnFailure = true,
                CallDirections = new List<CallDirection> { CallDirection.Incoming }
            };
            var callDurationRuleConfig = new CallDurationRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                ComparisonOperator = ">=",
                ParsedComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                CallDurationSeconds = 2700
            };

            SetRulesSettings(mediaTypeRuleConfig, callDirectionRuleConfig, callDurationRuleConfig);

            var callEvent = new CallEvent
            {
                CallId = 1,
                CallDirection = CallDirection.Unknown,
                Recordings = new List<Recording>
            {
                    new Recording { RecordingId = 1, MediaType = MediaType.Voice },
                    new Recording { RecordingId = 2, MediaType = MediaType.Screen },
                    new Recording { RecordingId = 3, MediaType = MediaType.Voice }
            }
            };
            var result = _service.ApplyRules(callEvent);

            Assert.True(result == null);
        }
        private void SetRulesSettings(MediaTypeRuleConfig mediaTypeRuleConfig,
            CallDirectionRuleConfig callDirectionRuleConfig, CallDurationRuleConfig callDurationRuleConfig)
        {
            _mediaTypeRuleConfigMock.Setup(x => x.CurrentValue).Returns(mediaTypeRuleConfig);
            _callDirectionRuleConfigMock.Setup(x => x.CurrentValue).Returns(callDirectionRuleConfig);
            _callDurationRuleConfigMock.Setup(x => x.CurrentValue).Returns(callDurationRuleConfig);

            var enabledRules = new List<IArchivingRule>
            {
                new CallDirectionArchivingRule(Mock.Of<ILogger<CallDirectionArchivingRule>>(), _callDirectionRuleConfigMock.Object),
                new CallDurationArchivingRule(Mock.Of<ILogger<CallDurationArchivingRule>>(), _operatorFactoryMock.Object, _callDurationRuleConfigMock.Object),
                new MediaTypeArchivingRule(Mock.Of<ILogger<MediaTypeArchivingRule>>(), _mediaTypeRuleConfigMock.Object)
            };

            _archivingRuleFactoryMock.Setup(x => x.CreateAllEnabledArchivingRules()).Returns(enabledRules);

            _operatorFactoryMock.Setup(x => x.CreateOperator(It.IsAny<ComparisonOperator>()))
                                .Returns(Mock.Of<IComparisonOperator>());

            _service = new ArchivingRuleManagerService(_loggerMock.Object, _archivingRuleFactoryMock.Object);
        }
    }
}
