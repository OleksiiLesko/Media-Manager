using MediaManager.Common.ComparisonOperators;
using MediaManager.Common;
using Microsoft.Extensions.Logging;
using Moq;

namespace MediaManager.Tests
{
    public class OperatorFactoryTest
    {
        private readonly Mock<ILoggerFactory> loggerFactoryMock;
        private readonly Mock<ILogger<IOperatorFactory>> loggerMock;
        private readonly Mock<IOperatorFactory> operatorFactoryMock;

        public OperatorFactoryTest()
        {
            loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerMock = new Mock<ILogger<IOperatorFactory>>();
            loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);
            operatorFactoryMock = new Mock<IOperatorFactory>();
        }

        [Fact]
        public void CreateOperator_GreaterThan_ReturnsInstanceOfGreaterThanOperator()
        {
            var factory = operatorFactoryMock.Object;

            operatorFactoryMock.Setup(x => x.CreateOperator(It.IsAny<ComparisonOperator>()))
                .Returns(new GreaterThanOperator(loggerFactoryMock.Object.CreateLogger<GreaterThanOperator>()));

            var result = factory.CreateOperator(ComparisonOperator.GreaterThan);

            Assert.IsType<GreaterThanOperator>(result);
        }

        [Fact]
        public void CreateOperator_LessThan_ReturnsInstanceOfLessThanOperator()
        {
            var factory = operatorFactoryMock.Object;

            operatorFactoryMock.Setup(x => x.CreateOperator(It.IsAny<ComparisonOperator>()))
                .Returns(new LessThanOperator(loggerFactoryMock.Object.CreateLogger<LessThanOperator>()));

            var result = factory.CreateOperator(ComparisonOperator.LessThan);

            Assert.IsType<LessThanOperator>(result);
        }

        [Fact]
        public void CreateOperator_GreaterThanOrEqual_ReturnsInstanceOfGreaterThanOrEqualOperator()
        {
            var factory = operatorFactoryMock.Object;

            operatorFactoryMock.Setup(x => x.CreateOperator(It.IsAny<ComparisonOperator>()))
                .Returns(new GreaterThanOrEqualOperator(loggerFactoryMock.Object.CreateLogger<GreaterThanOrEqualOperator>()));

            var result = factory.CreateOperator(ComparisonOperator.GreaterThanOrEqual);

            Assert.IsType<GreaterThanOrEqualOperator>(result);
        }

        [Fact]
        public void CreateOperator_LessThanOrEqual_ReturnsInstanceOfLessThanOrEqualOperator()
        {
            var factory = operatorFactoryMock.Object;

            operatorFactoryMock.Setup(x => x.CreateOperator(It.IsAny<ComparisonOperator>()))
                .Returns(new LessThanOrEqualOperator(loggerFactoryMock.Object.CreateLogger<LessThanOrEqualOperator>()));

            var result = factory.CreateOperator(ComparisonOperator.LessThanOrEqual);

            Assert.IsType<LessThanOrEqualOperator>(result);
        }

        [Fact]
        public void CreateOperator_Equal_ReturnsInstanceOfEqualOperator()
        {
            var factory = operatorFactoryMock.Object;

            operatorFactoryMock.Setup(x => x.CreateOperator(It.IsAny<ComparisonOperator>()))
                .Returns(new EqualOperator(loggerFactoryMock.Object.CreateLogger<EqualOperator>()));

            var result = factory.CreateOperator(ComparisonOperator.Equal);

            Assert.IsType<EqualOperator>(result);
        }

        [Fact]
        public void CreateOperator_UnsupportedOperator_ThrowsArgumentException()
        {
            var factory = operatorFactoryMock.Object;

            operatorFactoryMock.Setup(x => x.CreateOperator(It.IsAny<ComparisonOperator>()))
                .Throws<ArgumentException>();

            Assert.Throws<ArgumentException>(() => factory.CreateOperator((ComparisonOperator)100));
        }
    }
}
