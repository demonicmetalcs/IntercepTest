using IntercepTest;
using SampleProject;

namespace SampleTestProject
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void SampleTest()
        {
            //Arrange
            var sut = new ControllerToTest(null);

            //[IntercepTestMock(typeof(ServiceToMock), nameof(ServiceToMock.FunctionToMock), typeof(ControllerToTest), nameof(ControllerToTest.FunctionToTest))]
            int Mock(int parameter)
            { return parameter; }


            //Act
            var result = sut.FunctionToTest("5");

            //Assert
            Assert.AreEqual(25, result);
        }
    }
}