using MediaManager.ArchivingRuleManager;
using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaManager.Tests
{
    public class CallDirectionRuleTest
    {
        private readonly Mock<ILogger<CallDirectionRule>> _loggerMock;
        private readonly CallDirectionArchivingRule _callDirectionArchivingRule;
        public CallDirectionRuleTest()
        {
            _loggerMock = new Mock<ILogger<CallDirectionRule>>();
            _callDirectionArchivingRule = new CallDirectionArchivingRule
            {
                Enabled = true,
                StopOnFailure = false,
                CallDirections = new List<CallDirection> { CallDirection.Incoming }
            };
        }
        [Fact]
        public void ApplyRule_ShouldReturnMatchingRecordingIds_WhenCallDirectionCorrect()
        {
            var callDirectionRule = new CallDirectionRule(_loggerMock.Object, _callDirectionArchivingRule);

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
            var callDirectionRule = new CallDirectionRule(_loggerMock.Object, _callDirectionArchivingRule);

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
    }
}
