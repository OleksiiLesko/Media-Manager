using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Core;

namespace MediaManager.ArchivingRuleManager
{
    /// <summary>
    /// Represents a factory for archiving rules.
    /// </summary>
    public class ArchivingRuleFactory : IArchivingRuleFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<CallDirectionRuleConfig> _callDirectionRuleConfig;
        private readonly IOptionsMonitor<CallDurationRuleConfig> _callDurationRuleConfig;
        private readonly IOptionsMonitor<MediaTypeRuleConfig> _mediaTypeRuleConfig;
        private readonly IOperatorFactory _operatorFactory;

        public ArchivingRuleFactory(ILoggerFactory loggerFactory,
            IOptionsMonitor<CallDirectionRuleConfig> callDirectionRuleConfig,
            IOptionsMonitor<CallDurationRuleConfig> callDurationRuleConfig,
            IOptionsMonitor<MediaTypeRuleConfig> mediaTypeRuleConfig,
            IOperatorFactory operatorFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<ArchivingRuleFactory>();
            _callDirectionRuleConfig = callDirectionRuleConfig;
            _callDurationRuleConfig = callDurationRuleConfig;
            _mediaTypeRuleConfig = mediaTypeRuleConfig;
            _operatorFactory = operatorFactory;
        }
        /// <summary>
        /// Creates all enebled archiving rules.
        /// </summary>
        /// <returns></returns>
        public List<IArchivingRule> CreateAllEnabledArchivingRules()
        {
            _logger.LogInformation("Creating all enabled archiving rules.");

            List<IArchivingRule> enabledRules = new List<IArchivingRule>();

            foreach (RuleType ruleType in Enum.GetValues(typeof(RuleType)))
            {

                bool isRuleEnabled = DetermineRuleEnabledStatus(ruleType);

                if (isRuleEnabled)
                {

                    IArchivingRule? rule = CreateRule(ruleType);

                    enabledRules.Add(rule);
                }
            }

            _logger.LogInformation($"Created {enabledRules.Count} enabled archiving rules.");

            return enabledRules;
        }
        /// <summary>
        /// Determines if rule has an enabled status.
        /// </summary>
        /// <param name="ruleType"></param>
        /// <returns></returns>
        private bool DetermineRuleEnabledStatus(RuleType ruleType)
        {
            switch (ruleType)
            {
                case RuleType.CallDirectionArchivingRule:
                    var isEnabled = _callDirectionRuleConfig.CurrentValue.Enabled;
                    _logger.LogInformation($"Determining enabled status for {ruleType}: {isEnabled}");
                    return isEnabled;

                case RuleType.CallDurationArchivingRule:
                    isEnabled = _callDurationRuleConfig.CurrentValue.Enabled;
                    _logger.LogInformation($"Determining enabled status for {ruleType}: {isEnabled}");
                    return isEnabled;

                case RuleType.MediaTypeArchivingRule:
                    isEnabled = _mediaTypeRuleConfig.CurrentValue.Enabled;
                    _logger.LogInformation($"Determining enabled status for {ruleType}: {isEnabled}");
                    return isEnabled;

                default:
                    _logger.LogInformation($"Unknown rule type: {ruleType}. Returning false.");
                    return false;
            }
        }
        /// <summary>
        /// Creates special rule.
        /// </summary>
        /// <param name="ruleType"></param>
        /// <returns></returns>
        private IArchivingRule? CreateRule(RuleType ruleType)
        {
            _logger.LogInformation($"Creating {ruleType} rule.");

            switch (ruleType)
            {
                case RuleType.CallDirectionArchivingRule:
                    return new CallDirectionArchivingRule(_loggerFactory.CreateLogger<CallDirectionArchivingRule>(), _callDirectionRuleConfig);

                case RuleType.CallDurationArchivingRule:
                    return new CallDurationArchivingRule(_loggerFactory.CreateLogger<CallDurationArchivingRule>(), _operatorFactory, _callDurationRuleConfig);

                case RuleType.MediaTypeArchivingRule:
                    return new MediaTypeArchivingRule(_loggerFactory.CreateLogger<MediaTypeArchivingRule>(), _mediaTypeRuleConfig);

                default:
                    return null;
            }
        }
    }
}
