using MediaManager.ArchivingRuleManager;
using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MediaManager.ArchivingRulesTests.StepDefinitions
{
    [Binding, Scope(Tag = "MediaTypeArchivingRule")]
    public sealed class MediaTypeArchivingRuleSteps
    {
        private readonly Mock<ILogger<MediaTypeArchivingRule>> _loggerMock;
        private readonly Mock<IOptionsMonitor<MediaTypeRuleConfig>> _mediaTypeRuleConfigMock;
        private MediaTypeArchivingRule _mediaTypeArchivingRule;
        private CallEvent _callEvent;
        private HashSet<int> _result;

        public MediaTypeArchivingRuleSteps()
        {
            _loggerMock = new Mock<ILogger<MediaTypeArchivingRule>>();
            _mediaTypeRuleConfigMock = new Mock<IOptionsMonitor<MediaTypeRuleConfig>>();
        }

        [Given(@"the media type archiving rule is initialized")]
        public void GivenTheMediaTypeArchivingRuleIsInitialized()
        {
            var mediaTypeRuleConfigMockInstance = new MediaTypeRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                MediaTypes = new List<MediaType> { MediaType.Voice }
            };
            _mediaTypeRuleConfigMock.Setup(x => x.CurrentValue).Returns(mediaTypeRuleConfigMockInstance);

            _mediaTypeArchivingRule = new MediaTypeArchivingRule(_loggerMock.Object, _mediaTypeRuleConfigMock.Object);
        }

        [Given(@"a call event with special CallId (.*) and MediaType (.*)")]
        public void GivenACallEventWithRecordings(int callId,string mediaTypes)
        {
            List<MediaType> mediaTypeList = mediaTypes.Split(',').Select(m => Enum.Parse<MediaType>(m.Trim())).ToList();
            _callEvent = new CallEvent
            {
                CallId = callId,
                Recordings = mediaTypeList.Select((mediaType, index) => new Recording { RecordingId = index + 1, MediaType = mediaType }).ToList()
            };
        }

        [When(@"the media type archiving rule is applied")]
        public void WhenTheMediaTypeArchivingRuleIsApplied()
        {
            _result = _mediaTypeArchivingRule.ApplyRule(_callEvent);
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

        [Given(@"an initial media type configuration")]
        public void GivenAnInitialMediaTypeConfiguration()
        {
            var initialConfig = new MediaTypeRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                MediaTypes = new List<MediaType> { MediaType.Voice }
            };

            _mediaTypeRuleConfigMock.Setup(x => x.CurrentValue).Returns(initialConfig);

            _mediaTypeArchivingRule = new MediaTypeArchivingRule(_loggerMock.Object, _mediaTypeRuleConfigMock.Object);
        }

        [When(@"the configuration changes to updated media type (.*)")]
        public void WhenTheConfigurationChanges(MediaType updatedMediaType)
        {
            var updatedConfig = new MediaTypeRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                MediaTypes = new List<MediaType> { updatedMediaType }
            };
            Action<MediaTypeRuleConfig, string> onChangeHandler = null;
            _mediaTypeRuleConfigMock.Setup(x => x.CurrentValue).Returns(updatedConfig);

            _mediaTypeRuleConfigMock.Setup(x => x.OnChange(It.IsAny<Action<MediaTypeRuleConfig, string>>()))
                .Callback<Action<MediaTypeRuleConfig, string>>(onChange => onChangeHandler = onChange);

            var mediaTypeArchivingRule = new MediaTypeArchivingRule(_loggerMock.Object, _mediaTypeRuleConfigMock.Object);

            onChangeHandler.Invoke(updatedConfig, null);
        }

        [Then(@"it should refresh media types and log the change with message (.*)")]
        public void ThenItShouldRefreshMediaTypesAndLogTheChange(string expectedLogMessage)
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
