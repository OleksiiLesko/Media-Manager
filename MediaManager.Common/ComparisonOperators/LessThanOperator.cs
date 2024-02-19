using Microsoft.Extensions.Logging;

namespace MediaManager.Common.ComparisonOperators
{
    /// <summary>
    /// Represents a comparison operator for comparing less a value.
    /// </summary>
    public class LessThanOperator : IComparisonOperator
    {
        private readonly ILogger<LessThanOperator> _logger;

        public LessThanOperator(ILogger<LessThanOperator> logger)
        {
            _logger = logger;
        }
        /// <summary>
        ///  Compares two numbers using the comparison operator.
        /// </summary>
        /// <param name="callDuration"></param>
        /// <param name="callDurationConfig"></param>
        /// <returns></returns>
        public bool Compare(int callDuration, int callDurationConfig)
        {
            _logger.LogInformation($"Comparing {callDuration} with {callDurationConfig} using LessThanOperator");
            return callDuration.CompareTo(callDurationConfig) < 0;
        }
    }
}