using Serilog;
using System.ComponentModel;
namespace MediaManager.Common
{
    /// <summary>
    /// Represents a parser comparison operator.
    /// </summary>
    public static class ComparisonOperatorParser
    {
        private static readonly ILogger _logger = Log.ForContext(typeof(ComparisonOperatorParser));

        /// <summary>
        /// Parses comparison operator from configuration string format to enum.
        /// </summary>
        /// <param name="comparisonOperator"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static ComparisonOperator ParseComparisonOperator(string comparisonOperator)
        {
            var enumType = typeof(ComparisonOperator);
            var fields = enumType.GetFields();

            foreach (var field in fields)
            {
                var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                if (descriptionAttribute != null && descriptionAttribute.Description == comparisonOperator)
                {
                    var parsedOperator = (ComparisonOperator)Enum.Parse(enumType, field.Name);
                    _logger.Information("Comparison operator parsed to : {ComparisonOperator}", parsedOperator);
                    return parsedOperator;
                }
            }

            throw new ArgumentException($"Comparison operator not found: {comparisonOperator}");
        }
    }
}
