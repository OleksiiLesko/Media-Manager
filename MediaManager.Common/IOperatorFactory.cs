namespace MediaManager.Common
{
    /// <summary>
    /// Represents a factory that initializes an operator.
    /// </summary>
    public interface IOperatorFactory
    {
        /// <summary>
        /// Creates special comparison operator.
        /// </summary>
        /// <param name="comparisonOperator"></param>
        /// <returns></returns>
        IComparisonOperator CreateOperator(ComparisonOperator comparisonOperator);
    }
}
