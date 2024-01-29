﻿using MediaManager.ArchivingRuleManager;
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
                StopOnFailure = false,
                MediaTypes = new List<MediaType> { MediaType.Voice }
            };
        }
        [Fact]
        public void ApplyRule_ShouldReturnMatchingRecordingIds_WhenMediaTypesCorrect()
        {
            var mediaTypeRule = new MediaTypeRule(_loggerMock.Object, _mediaTypeArchivingRule);

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
            var mediaTypeRule = new MediaTypeRule(_loggerMock.Object, _mediaTypeArchivingRule);

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
    }
}
