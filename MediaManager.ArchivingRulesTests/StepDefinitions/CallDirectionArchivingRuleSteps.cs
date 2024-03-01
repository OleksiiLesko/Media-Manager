using MediaManager.ArchivingRuleManager;
using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MediaManager.ArchivingRulesTests.StepDefinitions
{
    [Binding, Scope(Tag = "CallDirectionArchivingRule")]
    public sealed class CallDirectionArchivingRuleSteps
    {
        private readonly Mock<ILogger<CallDirectionArchivingRule>> _loggerMock;
        private readonly Mock<IOptionsMonitor<CallDirectionRuleConfig>> _callDirectionRuleConfigMock;
        private CallDirectionArchivingRule _callDirectionArchivingRule;
        private CallEvent _callEvent;
        private HashSet<int> _result;

        public CallDirectionArchivingRuleSteps()
        {
            _loggerMock = new Mock<ILogger<CallDirectionArchivingRule>>();
            _callDirectionRuleConfigMock = new Mock<IOptionsMonitor<CallDirectionRuleConfig>>();
        }

        [Given(@"the call direction archiving rule is initialized")]
        public void GivenTheCallDirectionArchivingRuleIsInitialized()
        {
            var callDirectionRuleConfigMockInstance = new CallDirectionRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                CallDirections = new List<CallDirection> { CallDirection.Incoming }
            };
            _callDirectionRuleConfigMock.Setup(x => x.CurrentValue).Returns(callDirectionRuleConfigMockInstance);

            _callDirectionArchivingRule = new CallDirectionArchivingRule(_loggerMock.Object, _callDirectionRuleConfigMock.Object);
        }

        [Given(@"a call event with special CallId (.*) and CallDirection (.*)")]
        public void GivenACallEventWithRecordings(int callId, CallDirection callDirection)
        {
            _callEvent = new CallEvent
            {
                CallId = callId,
                CallDirection = callDirection,
                Recordings = new List<Recording>
                {
                    new Recording { RecordingId = 1, MediaType = MediaType.Voice },
                    new Recording { RecordingId = 2, MediaType = MediaType.Screen }
                }
            };
        }

        [When(@"the call direction archiving rule is applied")]
        public void WhenTheCallDirectionArchivingRuleIsApplied()
        {
            _result = _callDirectionArchivingRule.ApplyRule(_callEvent);
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

        [Given(@"an initial call direction configuration")]
        public void GivenAnInitialCallDirectionConfiguration()
        {
            var initialConfig = new CallDirectionRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                CallDirections = new List<CallDirection> { CallDirection.Incoming }
            };

            _callDirectionRuleConfigMock.Setup(x => x.CurrentValue).Returns(initialConfig);

            _callDirectionArchivingRule = new CallDirectionArchivingRule(_loggerMock.Object, _callDirectionRuleConfigMock.Object);
        }

        [When(@"the configuration changes to updated call direction (.*)")]
        public void WhenTheConfigurationChanges(CallDirection updatedCallDirection)
        {
            var updatedConfig = new CallDirectionRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                CallDirections = new List<CallDirection> { updatedCallDirection }
            };
            Action<CallDirectionRuleConfig, string> onChangeHandler = null;

            _callDirectionRuleConfigMock.Setup(x => x.CurrentValue).Returns(updatedConfig);

            _callDirectionRuleConfigMock.Setup(x => x.OnChange(It.IsAny<Action<CallDirectionRuleConfig, string>>()))
                .Callback<Action<CallDirectionRuleConfig, string>>(onChange => onChangeHandler = onChange);

            var callDirectionArchivingRule = new CallDirectionArchivingRule(_loggerMock.Object, _callDirectionRuleConfigMock.Object);

            onChangeHandler.Invoke(updatedConfig, null);
        }

        [Then(@"it should refresh call directions and log the change with message (.*)")]
        public void ThenItShouldRefreshCallDirectionsAndLogTheChange(string expectedLogMessage)
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
