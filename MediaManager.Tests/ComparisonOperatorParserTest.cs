using MediaManager.Common;

namespace MediaManager.Tests
{
    public class ComparisonOperatorParserTest
    {
        [Fact]
        public void ParseComparisonOperator_ValidOperator_ReturnsCorrectEnumValue()
        {
            var comparisonOperatorString = ">";
            var expectedComparisonOperator = ComparisonOperator.GreaterThan;

            var actualComparisonOperator = ComparisonOperatorParser.ParseComparisonOperator(comparisonOperatorString);

            Assert.Equal(expectedComparisonOperator, actualComparisonOperator);
        }

        [Fact]
        public void ParseComparisonOperator_InvalidOperator_ThrowsArgumentException()
        {
            var invalidComparisonOperatorString = ">?";

            Assert.Throws<ArgumentException>(() => ComparisonOperatorParser.ParseComparisonOperator(invalidComparisonOperatorString));
        }
    }
}
