using MediaManager.ArchivingRuleManager;
using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MediaManager.Tests
{
    public class MediaTypeArchivingRuleTest
    {
        private readonly Mock<ILogger<MediaTypeArchivingRule>> _loggerMock;
        private readonly Mock<IOptionsMonitor<MediaTypeRuleConfig>> _mediaTypeRuleConfigMock;
        public MediaTypeArchivingRuleTest()
        {
            _loggerMock = new Mock<ILogger<MediaTypeArchivingRule>>();
            _mediaTypeRuleConfigMock = new Mock<IOptionsMonitor<MediaTypeRuleConfig>>();
            var mediaTypeRuleConfigMockInstance = new MediaTypeRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                MediaTypes = new List<MediaType> { MediaType.Voice }
            };
            _mediaTypeRuleConfigMock.Setup(x => x.CurrentValue).Returns(mediaTypeRuleConfigMockInstance);
        }
        [Fact]
        public void ApplyRule_ShouldReturnMatchingRecordingIds_WhenMediaTypesCorrect()
        {
            var mediaTypeRule = new MediaTypeArchivingRule(_loggerMock.Object, _mediaTypeRuleConfigMock.Object);

            var callEvent = new CallEvent
            {
                CallId = 1,
                Recordings = new List<Recording>
            {
                    new Recording { RecordingId = 1, MediaType = MediaType.Voice },
                    new Recording { RecordingId = 2, MediaType = MediaType.Voice },
                    new Recording { RecordingId = 3, MediaType = MediaType.Screen }
            }
            };

            var result = mediaTypeRule.ApplyRule(callEvent);
            Assert.Equal(new HashSet<int> { 1, 2 }, result);
        }
        [Fact]
        public void ApplyRule_ShouldReturnMatchingRecordingIds_WhenMediaTypesNotCorrect()
        {
            var mediaTypeRule = new MediaTypeArchivingRule(_loggerMock.Object, _mediaTypeRuleConfigMock.Object);

            var callEvent = new CallEvent
            {
                CallId = 1,
                Recordings = new List<Recording>
            {
                    new Recording { RecordingId = 1, MediaType = MediaType.Screen }
            }
            };

            var result = mediaTypeRule.ApplyRule(callEvent);
            Assert.Equal(new HashSet<int> { }, result);
        }
        [Fact]
        public void OnChange_ShouldRefreshMediaTypes_WhenConfigurationChanges()
        {
            var initialConfig = new MediaTypeRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                MediaTypes = new List<MediaType> { MediaType.Voice }
            };

            var updatedConfig = new MediaTypeRuleConfig
            {
                Enabled = true,
                StopOnFailure = false,
                MediaTypes = new List<MediaType> { MediaType.Screen }
            };

            Action<MediaTypeRuleConfig, string> onChangeHandler = null;

            _mediaTypeRuleConfigMock.Setup(x => x.CurrentValue)
                .Returns(initialConfig);

            _mediaTypeRuleConfigMock.Setup(x => x.OnChange(It.IsAny<Action<MediaTypeRuleConfig, string>>()))
                .Callback<Action<MediaTypeRuleConfig, string>>(onChange => onChangeHandler = onChange);

            var mediaTypeArchivingRule = new MediaTypeArchivingRule(_loggerMock.Object, _mediaTypeRuleConfigMock.Object);

            onChangeHandler.Invoke(updatedConfig, null);

            _loggerMock.Verify(
                x => x.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("MediaTypeRulConfig changed. Refreshed MediaTypes")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
