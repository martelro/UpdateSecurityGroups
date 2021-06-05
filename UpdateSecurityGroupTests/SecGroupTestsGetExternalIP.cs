using Microsoft.VisualStudio.TestTools.UnitTesting;
using UpdateSecurityGroup;

namespace UpdateSecurityGroupTests
{
    [TestClass]
    public class SecGroupTestsGetExternalIP
    {
        [TestMethod]
        public void GetExternalIP_WithEmptyString_ReturnEmptyString()
        {
            //Arrange
            string expected = "";
            
             //Act
            string actual = UpdateSecurityGroupProgram.GetExternalIP(string.Empty);

            //Assert
            Assert.AreEqual(expected, actual, "Empty Value Sent; Empty String Expected");
        }

        [TestMethod]
        public void GetExternalIP_WithBadURL_ReturnEmptyString()
        {
            //Arrange
            string expected = "";

            //Act
            string actual = UpdateSecurityGroupProgram.GetExternalIP(UpdateSecurityGroupProgram.IPUrl + "a;sdfkja");

            //Assert
            Assert.AreEqual(expected, actual, "Bad URL sent; Empty String Expected");
        }

        //[TestMethod]
        //public void ValidateIP_WithValidIP_ReturnTrue()
        //{
        //    //Arrange
        //    bool expected = true;

        //    //Act
        //    bool actual = UpdateSecurityGroupProgram.ValidateIPv4("1.1.1.1");

        //    //Assert
        //    Assert.AreEqual(expected, actual);
        //}


        //[TestMethod]
        //public void ValidateIP_WithMalformedIP()
        //{
        //    //Arrange
        //    bool expected = false;

        //    //Act
        //    bool actual = UpdateSecurityGroupProgram.ValidateIPv4("1.1.");

        //    //Assert
        //    Assert.AreEqual(expected, actual);
        //}

        //[TestMethod]
        //public void ValidateIP_PlainTextIP()
        //{
        //    //Arrange
        //    bool expected = false;

        //    //Act
        //    bool actual = UpdateSecurityGroupProgram.ValidateIPv4("Thisisjustsometext.");

        //    //Assert
        //    Assert.AreEqual(expected, actual);
        //}

        //[TestMethod]
        //public void ValidateIP_NonNumericIP()
        //{
        //    //Arrange
        //    bool expected = false;

        //    //Act
        //    bool actual = UpdateSecurityGroupProgram.ValidateIPv4("A.B.C.D");

        //    //Assert
        //    Assert.AreEqual(expected, actual);
        //}

        //[TestMethod]
        //public void ValidateIP_EmptyStringIP()
        //{
        //    //Arrange
        //    bool expected = false;

        //    //Act
        //    bool actual = UpdateSecurityGroupProgram.ValidateIPv4("");

        //    //Assert
        //    Assert.AreEqual(expected, actual);
        //}

    }
}
