using MediaManager.Common.ComparisonOperators;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaManager.Tests
{
    public class ComparisonOperatorTest
    {
        [Fact]
        public void EqualOperator_Compare_ReturnsTrueForEqualValues()
        {
            var loggerMock = new Mock<ILogger<EqualOperator>>();
            var equalOperator = new EqualOperator(loggerMock.Object);

            var result = equalOperator.Compare(42, 42);

            Assert.True(result);
        }

        [Fact]
        public void EqualOperator_Compare_ReturnsFalseForDifferentValues()
        {
            var loggerMock = new Mock<ILogger<EqualOperator>>();
            var equalOperator = new EqualOperator(loggerMock.Object);

            var result = equalOperator.Compare(42, 24);

            Assert.False(result);
        }

        [Fact]
        public void GreaterThanOperator_Compare_ReturnsTrueForGreaterThanValues()
        {
            var loggerMock = new Mock<ILogger<GreaterThanOperator>>();
            var greaterThanOperator = new GreaterThanOperator(loggerMock.Object);

            var result = greaterThanOperator.Compare(42, 24);

            Assert.True(result);
        }

        [Fact]
        public void GreaterThanOperator_Compare_ReturnsFalseForLessThanValues()
        {
            var loggerMock = new Mock<ILogger<GreaterThanOperator>>();
            var greaterThanOperator = new GreaterThanOperator(loggerMock.Object);


            var result = greaterThanOperator.Compare(24, 42);

            Assert.False(result);
        }

        [Fact]
        public void GreaterThanOrEqualOperator_Compare_ReturnsTrueForGreaterThanOrEqualValues()
        {
            var loggerMock = new Mock<ILogger<GreaterThanOrEqualOperator>>();
            var greaterThanOrEqualOperator = new GreaterThanOrEqualOperator(loggerMock.Object);

            var result1 = greaterThanOrEqualOperator.Compare(42, 24);
            var result2 = greaterThanOrEqualOperator.Compare(42, 42);

            Assert.True(result1);
            Assert.True(result2);
        }

        [Fact]
        public void GreaterThanOrEqualOperator_Compare_ReturnsFalseForLessThanValues()
        {
            var loggerMock = new Mock<ILogger<GreaterThanOrEqualOperator>>();
            var greaterThanOrEqualOperator = new GreaterThanOrEqualOperator(loggerMock.Object);


            var result = greaterThanOrEqualOperator.Compare(24, 42);

            Assert.False(result);
        }

        [Fact]
        public void LessThanOperator_Compare_ReturnsTrueForLessThanValues()
        {
            var loggerMock = new Mock<ILogger<LessThanOperator>>();
            var lessThanOperator = new LessThanOperator(loggerMock.Object);

            var result = lessThanOperator.Compare(24, 42);

            Assert.True(result);
        }

        [Fact]
        public void LessThanOperator_Compare_ReturnsFalseForGreaterThanValues()
        {
            var loggerMock = new Mock<ILogger<LessThanOperator>>();
            var lessThanOperator = new LessThanOperator(loggerMock.Object);

            var result = lessThanOperator.Compare(42, 24);

            Assert.False(result);
        }

        [Fact]
        public void LessThanOrEqualOperator_Compare_ReturnsTrueForLessThanOrEqualValues()
        {
            var loggerMock = new Mock<ILogger<LessThanOrEqualOperator>>();
            var lessThanOrEqualOperator = new LessThanOrEqualOperator(loggerMock.Object);

            var result1 = lessThanOrEqualOperator.Compare(24, 42);
            var result2 = lessThanOrEqualOperator.Compare(42, 42);

            Assert.True(result1);
            Assert.True(result2);
        }

        [Fact]
        public void LessThanOrEqualOperator_Compare_ReturnsFalseForGreaterThanValues()
        {
            var loggerMock = new Mock<ILogger<LessThanOrEqualOperator>>();
            var lessThanOrEqualOperator = new LessThanOrEqualOperator(loggerMock.Object);

            var result = lessThanOrEqualOperator.Compare(42, 24);

            Assert.False(result);
        }
    }
}
