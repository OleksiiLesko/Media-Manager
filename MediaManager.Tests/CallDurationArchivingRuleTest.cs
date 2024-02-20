using MediaManager.ArchivingRuleManager;
using MediaManager.Common;
using MediaManager.Common.ComparisonOperators;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MediaManager.Tests
{
    public class CallDurationArchivingRuleTest
    {
        private readonly Mock<ILogger<CallDurationArchivingRule>> _loggerMock;
        private readonly Mock<IOptionsMonitor<CallDurationRuleConfig>> _callDurationRuleConfigMock;
        private readonly Mock<IOperatorFactory> _operatorFactoryMock;
        public CallDurationArchivingRuleTest()
        {
            _loggerMock = new Mock<ILogger<CallDurationArchivingRule>>();
            _callDurationRuleConfigMock = new Mock<IOptionsMonitor<CallDurationRuleConfig>>();
            _operatorFactoryMock = new Mock<IOperatorFactory>();
            var callDurationRuleConfigMockInstance = new CallDurationRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                ComparisonOperator = ">=",
                ParsedComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                CallDurationSeconds = 2700
            };
            _callDurationRuleConfigMock.Setup(x => x.CurrentValue).Returns(callDurationRuleConfigMockInstance);
        }
        [Fact]
        public void ApplyRule_ShouldReturnMatchingRecordingIds_WhenCallDurationCorrect()
        {
            var expectedOperatorInstance = new GreaterThanOrEqualOperator(Mock.Of<ILogger<GreaterThanOrEqualOperator>>());

            _operatorFactoryMock.Setup(x => x.CreateOperator(ComparisonOperator.GreaterThanOrEqual))
                                .Returns(expectedOperatorInstance);
            var callDurationRule = new CallDurationArchivingRule(_loggerMock.Object, _operatorFactoryMock.Object, _callDurationRuleConfigMock.Object);

            var callEvent = new CallEvent
            {
                CallId = 1,
                CallEndTime = DateTime.Now.AddMinutes(55.0),
                CallStartTime = DateTime.Now,
                CallDirection = CallDirection.Incoming,
                Recordings = new List<Recording>
            {
                    new Recording { RecordingId = 1, MediaType = MediaType.Voice },
                    new Recording { RecordingId = 2, MediaType = MediaType.Screen }
            }
            };

            var result = callDurationRule.ApplyRule(callEvent);
            Assert.Equal(new HashSet<int> { 1, 2 }, result);
        }
        [Fact]
        public void ApplyRule_ShouldReturnMatchingRecordingIds_WhenCallDurationNotCorrect()
        {
            var expectedOperatorInstance = new GreaterThanOrEqualOperator(Mock.Of<ILogger<GreaterThanOrEqualOperator>>());

            _operatorFactoryMock.Setup(x => x.CreateOperator(ComparisonOperator.GreaterThanOrEqual))
                                .Returns(expectedOperatorInstance);
            var callDurationRule = new CallDurationArchivingRule(_loggerMock.Object, _operatorFactoryMock.Object, _callDurationRuleConfigMock.Object);

            var callEvent = new CallEvent
            {
                CallId = 1,
                CallEndTime = DateTime.Now.AddMinutes(5.0),
                CallStartTime = DateTime.Now,
                CallDirection = CallDirection.Incoming,
                Recordings = new List<Recording>
            {
                    new Recording { RecordingId = 1, MediaType = MediaType.Voice },
                    new Recording { RecordingId = 2, MediaType = MediaType.Screen }
            }
            };

            var result = callDurationRule.ApplyRule(callEvent);
            Assert.Equal(new HashSet<int> { }, result);
        }
        [Fact]
        public void OnChange_ShouldRefreshCallDurationAndComparisonOperator_WhenConfigurationChanges()
        {
            var initialConfig = new CallDurationRuleConfig
            {
                Enabled = true,
                StopOnFailure = false
            };

            var updatedConfig = new CallDurationRuleConfig
            {
                Enabled = true,
                StopOnFailure = false
            };

            Action<CallDurationRuleConfig, string> onChangeHandler = null;

            _callDurationRuleConfigMock.Setup(x => x.CurrentValue)
                .Returns(initialConfig);

            _callDurationRuleConfigMock.Setup(x => x.OnChange(It.IsAny<Action<CallDurationRuleConfig, string>>()))
                .Callback<Action<CallDurationRuleConfig, string>>(onChange => onChangeHandler = onChange);

            var callDurationRule = new CallDurationArchivingRule(_loggerMock.Object, _operatorFactoryMock.Object, _callDurationRuleConfigMock.Object);

            onChangeHandler.Invoke(updatedConfig, null);

            _loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("CallDurationRuleConfig changed. Refreshed CallDuration")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
