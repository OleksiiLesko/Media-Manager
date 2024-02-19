using Microsoft.Extensions.Logging;

namespace MediaManager.Common.ComparisonOperators
{
    /// <summary>
    /// Represents a comparison operator for comparing greater than a value.
    /// </summary>
    public class GreaterThanOperator : IComparisonOperator
    {
        private readonly ILogger<GreaterThanOperator> _logger;

        public GreaterThanOperator(ILogger<GreaterThanOperator> logger)
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
            _logger.LogInformation($"Comparing {callDuration} with {callDurationConfig} using GreaterThanOperator");
            return callDuration.CompareTo(callDurationConfig) > 0;
        }
    }
}
