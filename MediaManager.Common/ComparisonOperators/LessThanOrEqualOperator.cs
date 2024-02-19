using Microsoft.Extensions.Logging;

namespace MediaManager.Common.ComparisonOperators
{
    /// <summary>
    ///Represents a comparison operator for comparing less than or equal to a value.
    /// </summary>
    public class LessThanOrEqualOperator : IComparisonOperator
    {
        private readonly ILogger<LessThanOrEqualOperator> _logger;

        public LessThanOrEqualOperator(ILogger<LessThanOrEqualOperator> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Compares two numbers using the comparison operator.
        /// </summary>
        /// <param name="callDuration"></param>
        /// <param name="callDurationConfig"></param>
        /// <returns></returns>
        public bool Compare(int callDuration, int callDurationConfig)
        {
            _logger.LogInformation($"Comparing {callDuration} with {callDurationConfig} using LessThanOrEqualOperator");
            return callDuration.CompareTo(callDurationConfig) <= 0;
        }
    }
}