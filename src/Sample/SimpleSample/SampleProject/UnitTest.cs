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

        [InterceptestMock("", 1,1)]
        int Mock(int x) { return x; }


        //Act
        var result = sut.FunctionToTest("5");

        //Assert
        Assert.AreEqual(25, result);
    }
}

