using MediaManager.ArchivingRuleManager;
using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaManager.Tests
{
    public class MediaTypeRuleTest
    {
        private readonly Mock<ILogger<MediaTypeRule>> _loggerMock;
        private readonly MediaTypeArchivingRule _mediaTypeArchivingRule;
        public MediaTypeRuleTest()
        {
            _loggerMock = new Mock<ILogger<MediaTypeRule>>();
            _mediaTypeArchivingRule = new MediaTypeArchivingRule
            {
                Enabled = true,
                MediaTypes = new List<MediaType> { MediaType.Voice }
            };
        }
        [Fact]
        public void ApplyRule_ShouldReturnMatchingRecordingIds_WhenMediaTypesMatch()
        {

            var mediaTypeRule = new MediaTypeRule(_loggerMock.Object, _mediaTypeArchivingRule);

            var recordings = new List<Recording>
        {
            new Recording { RecordingId = 1, MediaType = MediaType.Voice },
            new Recording { RecordingId = 2, MediaType = MediaType.Screen },
            new Recording { RecordingId = 3, MediaType = MediaType.Voice }
        };
            var result = mediaTypeRule.ApplyRule(recordings);

            Assert.Equal(new List<int> { 1, 3 }, result);
        }
    }
}
