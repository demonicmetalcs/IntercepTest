//todo this should be in a different assembly
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Interceptest;

namespace SampleProject;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void SampleTest()
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

    [TestMethod]
    public void MultipleMock()
    {
        //Arrange
        var sut = new ControllerToTest(null);

        [InterceptestMock(typeof(ServiceToMock), nameof(ServiceToMock.FunctionToMockMultiple2), typeof(ControllerToTest), nameof(ControllerToTest.FunctionToTest2))]
        int Mock2(int parameter) { return parameter; }

        [InterceptestMock(typeof(ServiceToMock), nameof(ServiceToMock.FunctionToMockMultiple3), typeof(ControllerToTest), nameof(ControllerToTest.FunctionToTest2))]
        int Mock3(int parameter) { return parameter +5; }


        //Act
        var result = sut.FunctionToTest2("5");

        //Assert
        Assert.AreEqual(50, result);
    }
}

