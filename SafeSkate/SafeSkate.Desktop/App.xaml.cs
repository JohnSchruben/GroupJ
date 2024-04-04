using System.Configuration;
using System.Data;
using System.Runtime.ConstrainedExecution;
using System.Windows;

namespace SafeSkate.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Task.Run(async () =>
            {
                ServiceTypeProvider.ServerIp = "localhost";
                ServiceTypeProvider.UpdatePort = 9000;
                ServiceTypeProvider.QueryPort = 9001;

                var model = ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy;
            }).Wait();
            base.OnStartup(e);
        }
    }
}
