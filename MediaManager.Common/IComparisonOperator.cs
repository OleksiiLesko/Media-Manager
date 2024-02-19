namespace MediaManager.Common
{
    /// <summary>
    /// Represents a comparison operator.
    /// </summary>
    public interface IComparisonOperator
    {
        /// <summary>
        /// Compares two numbers using the comparison operator.
        /// </summary>
        /// <param name="callDuration"></param>
        /// <param name="callDurationConfig"></param>
        /// <returns></returns>
        bool Compare(int callDuration, int callDurationConfig);
    }
}
