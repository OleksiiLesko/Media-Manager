using Microsoft.Extensions.Logging;

namespace MediaManager.Common.ComparisonOperators
{
    /// <summary>
    /// Represents a comparison operator for comparing greater than or equal to a value.
    /// </summary>
    public class GreaterThanOrEqualOperator : IComparisonOperator
    {
        private readonly ILogger<GreaterThanOrEqualOperator> _logger;

        public GreaterThanOrEqualOperator(ILogger<GreaterThanOrEqualOperator> logger)
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
            _logger.LogInformation($"Comparing {callDuration} with {callDurationConfig} using GreaterThanOrEqualOperator");
            return callDuration.CompareTo(callDurationConfig) >= 0;
        }
    }
}