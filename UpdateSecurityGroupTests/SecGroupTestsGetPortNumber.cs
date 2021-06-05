using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateSecurityGroup;

namespace UpdateSecurityGroupTests
{
    [TestClass]
    public class SecGroupTestsGetPortNumber
    {
        [TestMethod]
        public void GetPortNumber_WithValidArgument_ReturnPort()
        {
            //Arrange
            int expected = 3389;
            string[] args = { "3389" };

            //Act
            int actual = UpdateSecurityGroupProgram.GetPortNumber(args);

            //Assert
            Assert.AreEqual(expected, actual, "Port number returned does not equal value passed from args");
        }

        [TestMethod]
        public void GetPortNumber_WithMoreThanOneArgument_ReturnZero()
        {
            //Arrange
            int expected = 0;
            string[] args = { "3389", "443"};

            //Act
            int actual = UpdateSecurityGroupProgram.GetPortNumber(args);

            //Assert
            Assert.AreEqual(expected, actual, "Multiple strings passed; result should be zero");
        }

        [TestMethod]
        public void GetPortNumber_WithNonNumeric_ReturnZero()
        {
            //Arrange
            int expected = 0;
            string[] args = { "ThisIsNotANumber"};

            //Act
            int actual = UpdateSecurityGroupProgram.GetPortNumber(args);

            //Assert
            Assert.AreEqual(expected, actual, "NonNumeric value passed; result should be zero");
        }

        [TestMethod]
        public void GetPortNumber_NullArgs_ReturnZero()
        {
            //Arrange
            int expected = 0;
            string[] args = null;

            //Act
            int actual = UpdateSecurityGroupProgram.GetPortNumber(args);

            //Assert
            Assert.AreEqual(expected, actual, "Arg array has no elements/null; result should be zero");
        }

        [TestMethod]
        public void GetPortNumber_EmptyString_ReturnZero()
        {
            //Arrange
            int expected = 0;
            string[] args = { "" }; //This is equivalent to String.Empty

            //Act
            int actual = UpdateSecurityGroupProgram.GetPortNumber(args);

            //Assert
            Assert.AreEqual(expected, actual, "Empty String value passed; result should be zero");
        }

        [TestMethod]
        public void GetPortNumber_WithLessThanLowerBound_ReturnZero()
        {
            //Arrange
            int expected = 0;
            string[] args = { "-5",};

            //Act
            int actual = UpdateSecurityGroupProgram.GetPortNumber(args);

            //Assert
            Assert.AreEqual(expected, actual, "Value less than lower bound passed; result should be zero");
        }

        [TestMethod]
        public void GetPortNumber_WithAtLowerBound_ReturnZero()
        {
            //Arrange
            int expected = 1;
            string[] args = { "1", };

            //Act
            int actual = UpdateSecurityGroupProgram.GetPortNumber(args);

            //Assert
            Assert.AreEqual(expected, actual, "Value is at lower bound; result should be value");
        }

        [TestMethod]
        public void GetPortNumber_WithAtUpperBound_ReturnZero()
        {
            //Arrange
            int expected = 65535;
            string[] args = { "65535", };

            //Act
            int actual = UpdateSecurityGroupProgram.GetPortNumber(args);

            //Assert
            Assert.AreEqual(expected, actual, "Value is at upper bound; result should be value");
        }

        [TestMethod]
        public void GetPortNumber_WithAboveUpperBound_ReturnZero()
        {
            //Arrange
            int expected = 0;
            string[] args = { "82938", };

            //Act
            int actual = UpdateSecurityGroupProgram.GetPortNumber(args);

            //Assert
            Assert.AreEqual(expected, actual, "Value is at above upper bound; result should be zero");
        }
    }
}
