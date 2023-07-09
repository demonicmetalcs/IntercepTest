namespace SampleProject
{
    public class ControllerToTest
    {
        private ServiceToMock _serviceToMock;
        public ControllerToTest(ServiceToMock serviceToMock)
        {
            _serviceToMock = serviceToMock;
        }


        public int FunctionToTest(string parameter)
        {
            int parse = int.Parse(parameter);
            return _serviceToMock.FunctionToMock(parse)*5;
        }

        public int FunctionToTestMultiple2(string parameter)
        {
            int parse = int.Parse(parameter);
            return _serviceToMock.FunctionToMockMultiple2(parse) * _serviceToMock.FunctionToMockMultiple3(parse);
        }
    }
}
