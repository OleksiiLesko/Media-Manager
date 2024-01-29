﻿using MediaManager.ArchivingRuleManager;
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
        private readonly Mock<IOptionsMonitor<RulesSettings>> _rulesSettingsMock;
        private readonly Mock<ILoggerFactory> _loggerFactoryMock;
        private  ArchivingRuleManagerService _service;

        public ArchivingRuleManagerServiceTest()
        {
            _loggerMock = new Mock<ILogger<ArchivingRuleManagerService>>();
            _rulesSettingsMock = new Mock<IOptionsMonitor<RulesSettings>>();
            _loggerFactoryMock = new Mock<ILoggerFactory>();

            var rulesSettingsInstance = new RulesSettings
            {
                MediaTypeArchivingRule = new MediaTypeArchivingRule
                {
                    Enabled = true,
                    StopOnFailure = false,
                    MediaTypes = new List<MediaType> { MediaType.Voice }
                },
                CallDirectionArchivingRule = new CallDirectionArchivingRule
                {
                    Enabled = true,
                    StopOnFailure = false,
                    CallDirections = new List<CallDirection> { CallDirection.Incoming }
                }
            };
            _rulesSettingsMock.Setup(x => x.CurrentValue).Returns(rulesSettingsInstance);
            _service = new ArchivingRuleManagerService(_loggerMock.Object,
                _rulesSettingsMock.Object,
                _loggerFactoryMock.Object);
        }
        [Fact]
        public void ApplyRules_ReturnsFilteredCall()
        {
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
            var mediaTypeRule = new MediaTypeArchivingRule
            {
                Enabled = false,
                StopOnFailure = false,
                MediaTypes = new List<MediaType> { MediaType.Voice }
            };

            var callDirectionRule = new CallDirectionArchivingRule
            {
                Enabled = true,
                StopOnFailure = false,
                CallDirections = new List<CallDirection> { CallDirection.Incoming }
            };

            SetRulesSettings(mediaTypeRule, callDirectionRule);

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
            var mediaTypeRule = new MediaTypeArchivingRule
            {
                Enabled = true,
                StopOnFailure = true,
                MediaTypes = new List<MediaType> { MediaType.Voice }
            };

            var callDirectionRule = new CallDirectionArchivingRule
            {
                Enabled = true,
                StopOnFailure = true,
                CallDirections = new List<CallDirection> { CallDirection.Incoming }
            };

            SetRulesSettings(mediaTypeRule, callDirectionRule);

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
        private void SetRulesSettings(MediaTypeArchivingRule mediaTypeRule, CallDirectionArchivingRule callDirectionRule)
        {
            var rulesSettingsInstance = new RulesSettings
            {
                MediaTypeArchivingRule = mediaTypeRule,
                CallDirectionArchivingRule = callDirectionRule
            };

            _rulesSettingsMock.Setup(x => x.CurrentValue).Returns(rulesSettingsInstance);
            _service = new ArchivingRuleManagerService(_loggerMock.Object,
                _rulesSettingsMock.Object,
                _loggerFactoryMock.Object);
        }
    }
}
