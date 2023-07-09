//todo this should be in a different assembly
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Interceptest;

namespace SampleProject;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        //Arrange
        var sut = new ControllerToTest(null);

        [InterceptestMock(typeof(ServiceToMock),nameof(ServiceToMock.FunctionToMock), typeof(ControllerToTest), nameof(ControllerToTest.FunctionToTest))]
        int Mock(int parameter) { return parameter; }


        //Act
        var result = sut.FunctionToTest("5");

        //Assert
        Assert.AreEqual(25, result);
    }
}

