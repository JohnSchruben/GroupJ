using SafeSkate;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity;

namespace Tests
{
    class Program
    {
        public static void Main(string[] args)
        {
            // starting the service in the background.
            StartServce();

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

            // killing the service.
            KillService();
        }

        private static void KillService()
        {
            try
            {
                foreach (var process in Process.GetProcessesByName("SafeSkate.Service"))
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void StartServce()
        {
            string baseDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string serviceExecutablePath = Path.Combine(baseDirectory, "SafeSkate.Service.exe");

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = serviceExecutablePath,
                Arguments = "",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Process serviceProcess = new Process { StartInfo = startInfo };
            serviceProcess.Start();
        }
    }
}
