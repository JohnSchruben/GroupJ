using SafeSkate;
using System.Runtime.CompilerServices;
using Unity;

namespace Tests
{
    class Program
    {
        public static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();
            container.AddExtension(new TestContainerExtension());

            var tests = container.ResolveAll<ITest>();

            foreach(var test in tests)
            {
                Console.WriteLine(test.RunTest());
            }

            Console.ReadKey();
        }
    }
}
