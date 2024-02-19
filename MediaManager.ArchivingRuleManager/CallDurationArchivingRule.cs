using MediaManager.Common;
using MediaManager.Domain.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MediaManager.ArchivingRuleManager
{
    /// <summary>
    /// Represents an archiving rule for call by call duration.
    /// </summary>
    public class CallDurationArchivingRule : IArchivingRule
    {
        private readonly ILogger<CallDurationArchivingRule> _logger;
        private readonly IOperatorFactory _operatorFactory;
        private readonly CallDurationRuleConfig _callDurationRuleConfig;
        private int _callDurationConfig;
        private ComparisonOperator _comparisonOperatorConfig;
        public int Priority { get; set; }
        public bool StopOnFailure { get; set; }

        public CallDurationArchivingRule(ILogger<CallDurationArchivingRule> logger,
            IOperatorFactory operatorFactory,
            IOptionsMonitor<CallDurationRuleConfig> callDurationRuleConfig)
        {
            _logger = logger;

            _operatorFactory = operatorFactory;
            _callDurationRuleConfig = callDurationRuleConfig.CurrentValue;
            Priority = _callDurationRuleConfig.Priority;
            StopOnFailure = _callDurationRuleConfig.StopOnFailure;

            _logger.LogInformation($"CallDurationArchivingRule initialized with " +
               $"Priority: {Priority}, StopOnFailure: {StopOnFailure}");

            _callDurationConfig = GetCallDurationFromConfig();
            _comparisonOperatorConfig = GetComparisonOperatorFromConfig();

            callDurationRuleConfig.OnChange(settings =>
            {
                _callDurationConfig = GetCallDurationFromConfig();
                _comparisonOperatorConfig = GetComparisonOperatorFromConfig();
                _logger.LogInformation("CallDurationRuleConfig changed. Refreshed CallDuration");
            });
        }

        /// <summary>
        /// Applies rule for call recordings by call duration.
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public HashSet<int> ApplyRule(CallEvent callEvent)
        {
            TimeSpan callDuration = callEvent.CallEndTime - callEvent.CallStartTime;
            var callDurationSeconds = (int)callDuration.TotalSeconds;

            var comparisedCallDuration = CompareCallDuration(callDurationSeconds);

            if (comparisedCallDuration)
            {
                _logger.LogInformation($"Applying rule for call recordings by call duration: {callEvent.CallId}");
                return new HashSet<int>(callEvent.Recordings.Select(recording => recording.RecordingId));
            }
            else
            {
                _logger.LogInformation($"Rule not applied for call event with ID: {callEvent.CallId}");
                return new HashSet<int>();
            }
        }
        /// <summary>
        /// Compares call durations.
        /// </summary>
        /// <param name="callDurationSeconds"></param>
        /// <returns></returns>
        private bool CompareCallDuration(int callDurationSeconds)
        {
            var createdOperator = _operatorFactory.CreateOperator(_comparisonOperatorConfig);
            _logger.LogInformation($"Comparing call duration: {callDurationSeconds} seconds");
            return createdOperator.Compare(callDurationSeconds, _callDurationConfig);
        }
        /// <summary>
        /// Gets call duration from configuration.
        /// </summary>
        /// <returns></returns>
        private int GetCallDurationFromConfig()
        {
            var callDuration = _callDurationRuleConfig.CallDurationSeconds;
            _logger.LogInformation($"Call duration retrieved from config: {callDuration} seconds");
            return callDuration;
        }

        /// <summary>
        /// Gets comparison operator for call duration.
        /// </summary>
        /// <returns></returns>
        private ComparisonOperator GetComparisonOperatorFromConfig()
        {
            var comparisonOperator = _callDurationRuleConfig.ParsedComparisonOperator;
            _logger.LogInformation($"Comparison operator retrieved from config: {comparisonOperator}");
            return comparisonOperator;
        }
    }
}
