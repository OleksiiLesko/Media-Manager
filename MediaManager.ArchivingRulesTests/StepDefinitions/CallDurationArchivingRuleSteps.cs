using MediaManager.ArchivingRuleManager;
using MediaManager.Common;
using MediaManager.Common.ComparisonOperators;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MediaManager.ArchivingRulesTests.StepDefinitions
{
    [Binding, Scope(Tag = "CallDurationArchivingRule")]
    public sealed class CallDurationArchivingRuleSteps
    {
        private readonly Mock<ILogger<CallDurationArchivingRule>> _loggerMock;
        private readonly Mock<IOptionsMonitor<CallDurationRuleConfig>> _callDurationRuleConfigMock;
        private readonly Mock<IOperatorFactory> _operatorFactoryMock;
        private CallDurationArchivingRule _callDurationArchivingRule;
        private CallEvent _callEvent;
        private HashSet<int> _result;
        public CallDurationArchivingRuleSteps()
        {
            _loggerMock = new Mock<ILogger<CallDurationArchivingRule>>();
            _callDurationRuleConfigMock = new Mock<IOptionsMonitor<CallDurationRuleConfig>>();
            _operatorFactoryMock = new Mock<IOperatorFactory>();
        }

        [Given(@"the call duration archiving rule is initialized with сomparison operator (.*) and call duration (.*)")]
        public void GivenTheCallDurationArchivingRuleIsInitialized(string comparisonOperator, int callDuration)
        {
            var callDurationRuleConfigMockInstance = new CallDurationRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                ComparisonOperator = comparisonOperator,
                ParsedComparisonOperator = ComparisonOperator.GreaterThanOrEqual,
                CallDurationSeconds = callDuration
            };
            _callDurationRuleConfigMock.Setup(x => x.CurrentValue).Returns(callDurationRuleConfigMockInstance);
            var expectedOperatorInstance = new GreaterThanOrEqualOperator(Mock.Of<ILogger<GreaterThanOrEqualOperator>>());

            _operatorFactoryMock.Setup(x => x.CreateOperator(ComparisonOperator.GreaterThanOrEqual))
                                .Returns(expectedOperatorInstance);

            _callDurationArchivingRule = new CallDurationArchivingRule(_loggerMock.Object, _operatorFactoryMock.Object, _callDurationRuleConfigMock.Object);
        }

        [Given(@"a call event with special CallId (.*) and CallStartTime (.*), CallEndTime (.*)")]
        public void GivenACallEventWithRecordings(int callId, DateTime callStartTime, DateTime callEndTime)
        {
            _callEvent = new CallEvent
            {
                CallId = callId,
                CallEndTime = callEndTime,
                CallStartTime = callStartTime,
                CallDirection = CallDirection.Incoming,
                Recordings = new List<Recording>
                {
                    new Recording { RecordingId = 1, MediaType = MediaType.Voice },
                    new Recording { RecordingId = 2, MediaType = MediaType.Screen }
                }
            };
        }

        [When(@"the call duration archiving rule is applied")]
        public void WhenTheCallDurationArchivingRuleIsApplied()
        {
            _result = _callDurationArchivingRule.ApplyRule(_callEvent);
        }

        [Then(@"it should return expected result (.*) set of recording ids")]
        public void ThenItShouldReturnMatchingRecordingIds(string expectedRecordingIds)
        {
            HashSet<int> expectedSet;

            if (string.IsNullOrEmpty(expectedRecordingIds))
            {
                expectedSet = new HashSet<int>();
            }
            else
            {
                expectedSet = new HashSet<int>(Array.ConvertAll(expectedRecordingIds.Split(','), int.Parse));
            }

            Assert.Equal(expectedSet, _result);
        }

        [Given(@"an initial call duration configuration")]
        public void GivenAnInitialCallDurationConfiguration()
        {
            var initialConfig = new CallDurationRuleConfig
            {
                Enabled = true,
                StopOnFailure = false
            };

            _callDurationRuleConfigMock.Setup(x => x.CurrentValue).Returns(initialConfig);

            _callDurationArchivingRule = new CallDurationArchivingRule(_loggerMock.Object, _operatorFactoryMock.Object, _callDurationRuleConfigMock.Object);
        }
        [When(@"the configuration changes to updated call duration (.*)")]
        public void WhenTheConfigurationChanges(int updatedCallDuration)
        {
            var updatedConfig = new CallDurationRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                CallDurationSeconds = updatedCallDuration
            };

            Action<CallDurationRuleConfig, string> onChangeHandler = null;

            _callDurationRuleConfigMock.Setup(x => x.CurrentValue).Returns(updatedConfig);

            _callDurationRuleConfigMock.Setup(x => x.OnChange(It.IsAny<Action<CallDurationRuleConfig, string>>()))
                .Callback<Action<CallDurationRuleConfig, string>>(onChange => onChangeHandler = onChange);

            var callDurationArchivingRule = new CallDurationArchivingRule(_loggerMock.Object, _operatorFactoryMock.Object, _callDurationRuleConfigMock.Object);

            onChangeHandler.Invoke(updatedConfig, null);
        }

        [Then(@"it should refresh call directions and log the change with message (.*)")]
        public void ThenItShouldRefreshCallDurationAndLogTheChange(string expectedLogMessage)
        {
            _loggerMock.Verify(
               x => x.Log(
                   It.IsAny<LogLevel>(),
                   It.IsAny<EventId>(),
                   It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(expectedLogMessage)),
                   It.IsAny<Exception>(),
                   It.IsAny<Func<It.IsAnyType, Exception, string>>()),
               Times.Once);
        }
    }
}
