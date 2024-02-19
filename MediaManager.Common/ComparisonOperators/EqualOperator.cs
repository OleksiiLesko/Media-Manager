using Microsoft.Extensions.Logging;

namespace MediaManager.Common.ComparisonOperators
{
    /// <summary>
    /// Represents a comparison operator for comparing equal to a value.
    /// </summary>
    public class EqualOperator : IComparisonOperator
    {
        private readonly ILogger<EqualOperator> _logger;

        public EqualOperator(ILogger<EqualOperator> logger)
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
            _logger.LogInformation($"Comparing {callDuration} with {callDurationConfig} using EqualOperator");
            return callDuration.Equals(callDurationConfig);
        }
    }
}