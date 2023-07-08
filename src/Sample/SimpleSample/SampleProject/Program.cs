using System.Runtime.CompilerServices;

namespace SampleProject;

public class Program
{
    static void Main(string[] args)
    {
        new Test();
    }

    
}
public class Test
{
    public Test()
    {
        HelloFrom("World");
    }

    public void HelloFrom(string name)
    {
        Console.WriteLine(name);
    }
}