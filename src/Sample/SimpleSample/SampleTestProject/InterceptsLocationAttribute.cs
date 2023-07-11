namespace IntercepTest;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class IntercepTestMockAttribute : Attribute
{
    public IntercepTestMockAttribute(System.Type typeToMock, string functionToMock, System.Type callingType, string callingFunction)
    {
    }
}