using SafeSkate;
using System.Runtime.CompilerServices;
using Unity;

namespace Tests
{
    class Program
    {
        public static void Main(string[] args)
        {
            // using the container to register tests. we do this so that 
            // you dont have to call new to constuct objects
            IUnityContainer container = new UnityContainer();
            container.AddExtension(new TestContainerExtension());

            // create all tests
            var tests = container.ResolveAll<ITest>();

            // run each test
            foreach(var test in tests)
            {
                // print test result
                Console.WriteLine(test.RunTest());
            }
        }
    }
}
