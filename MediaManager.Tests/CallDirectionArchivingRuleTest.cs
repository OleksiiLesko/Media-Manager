using MediaManager.ArchivingRuleManager;
using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MediaManager.Tests
{
    public class CallDirectionArchivingRuleTest
    {
        private readonly Mock<ILogger<CallDirectionArchivingRule>> _loggerMock;
        private readonly Mock<IOptionsMonitor<CallDirectionRuleConfig>> _callDirectionRuleConfigMock;
        public CallDirectionArchivingRuleTest()
        {
            _loggerMock = new Mock<ILogger<CallDirectionArchivingRule>>();
            _callDirectionRuleConfigMock = new Mock<IOptionsMonitor<CallDirectionRuleConfig>>();
            var callDirectionRuleConfigMockInstance = new CallDirectionRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                CallDirections = new List<CallDirection> { CallDirection.Incoming }
            };
            _callDirectionRuleConfigMock.Setup(x => x.CurrentValue).Returns(callDirectionRuleConfigMockInstance);
        }
        [Fact]
        public void ApplyRule_ShouldReturnMatchingRecordingIds_WhenCallDirectionCorrect()
        {
            var callDirectionRule = new CallDirectionArchivingRule(_loggerMock.Object, _callDirectionRuleConfigMock.Object);

            var callEvent = new CallEvent
            {
                CallId = 1,
                CallDirection = CallDirection.Incoming,
                Recordings = new List<Recording>
            {
                    new Recording { RecordingId = 1, MediaType = MediaType.Voice },
                    new Recording { RecordingId = 2, MediaType = MediaType.Screen }
            }
            };

            var result = callDirectionRule.ApplyRule(callEvent);
            Assert.Equal(new HashSet<int> { 1,2 }, result);
        }
        [Fact]
        public void ApplyRule_ShouldReturnMatchingRecordingIds_WhenCallDirectionNotCorrect()
        {
            var callDirectionRule = new CallDirectionArchivingRule(_loggerMock.Object, _callDirectionRuleConfigMock.Object);

            var callEvent = new CallEvent
            {
                CallId = 1,
                CallDirection = CallDirection.Unknown,
                Recordings = new List<Recording>
            {
                    new Recording { RecordingId = 1, MediaType = MediaType.Voice },
                    new Recording { RecordingId = 2, MediaType = MediaType.Screen }
            }
            };

            var result = callDirectionRule.ApplyRule(callEvent);
            Assert.Equal(new HashSet<int> { }, result);
        }
        [Fact]
        public void OnChange_ShouldRefreshCallDirections_WhenConfigurationChanges()
        {
            var initialConfig = new CallDirectionRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                CallDirections = new List<CallDirection> { CallDirection.Incoming }
            };

            var updatedConfig = new CallDirectionRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                CallDirections = new List<CallDirection> { CallDirection.Outcoming }
            };

            Action<CallDirectionRuleConfig, string> onChangeHandler = null;

            _callDirectionRuleConfigMock.Setup(x => x.CurrentValue)
                .Returns(initialConfig);

            _callDirectionRuleConfigMock.Setup(x => x.OnChange(It.IsAny<Action<CallDirectionRuleConfig, string>>()))
                .Callback<Action<CallDirectionRuleConfig, string>>(onChange => onChangeHandler = onChange);

            var callDirectionArchivingRule = new CallDirectionArchivingRule(_loggerMock.Object, _callDirectionRuleConfigMock.Object);

            onChangeHandler.Invoke(updatedConfig, null);

            _loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CallDirectionRuleConfig changed. Refreshed CallDirections.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
