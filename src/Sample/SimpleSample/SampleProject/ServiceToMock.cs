using System.Runtime.CompilerServices;

[assembly: IntercepTestProject("D:\\Github\\Interceptest\\src\\Sample\\SimpleSample\\SampleTestProject\\SampleTestProject.csproj")]
namespace SampleProject
{
    public class ServiceToMock
    {
        public int FunctionToMock(int parameter)
        {
            throw new NotImplementedException();
        }

        public int FunctionToMockMultiple2(int parameter)
        {
            throw new NotImplementedException();
        }
        public int FunctionToMockMultiple3(int parameter)
        {
            throw new NotImplementedException();
        }

        public int FunctionToMockMultipleInjection(int parameter)
        {
            throw new NotImplementedException();
        }
    }
}
