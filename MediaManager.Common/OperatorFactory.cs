using MediaManager.Common.ComparisonOperators;
using Microsoft.Extensions.Logging;

namespace MediaManager.Common
{
    /// <summary>
    /// Represents a factory that initializes an operator.
    /// </summary>
    public class OperatorFactory : IOperatorFactory
    {
        private readonly ILogger<OperatorFactory> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public OperatorFactory(ILogger<OperatorFactory> logger, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Creates operator.
        /// </summary>
        /// <param name="comparisonOperator"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IComparisonOperator CreateOperator(ComparisonOperator comparisonOperator)
        {
            switch (comparisonOperator)
            {
                case ComparisonOperator.GreaterThan:
                    _logger.LogInformation($"Creating operator {comparisonOperator}");
                    return new GreaterThanOperator(_loggerFactory.CreateLogger<GreaterThanOperator>());
                case ComparisonOperator.LessThan:
                    _logger.LogInformation($"Creating operator {comparisonOperator}");
                    return new LessThanOperator(_loggerFactory.CreateLogger<LessThanOperator>());
                case ComparisonOperator.GreaterThanOrEqual:
                    _logger.LogInformation($"Creating operator {comparisonOperator}");
                    return new GreaterThanOrEqualOperator(_loggerFactory.CreateLogger<GreaterThanOrEqualOperator>());
                case ComparisonOperator.LessThanOrEqual:
                    _logger.LogInformation($"Creating operator {comparisonOperator}");
                    return new LessThanOrEqualOperator(_loggerFactory.CreateLogger<LessThanOrEqualOperator>());
                case ComparisonOperator.Equal:
                    _logger.LogInformation($"Creating operator {comparisonOperator}");
                    return new EqualOperator(_loggerFactory.CreateLogger<EqualOperator>());
                default:
                    _logger.LogError("Unsupported operator requested");
                    throw new ArgumentException("Unsupported operator");
            }
        }
    }
}
