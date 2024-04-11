using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SafeSkate.Mobile
{
    public partial class App : Xamarin.Forms.Application
    {
        public App()
        {
            Task.Run(async () =>
            {
                ServiceTypeProvider.ServerIp = "172.214.88.163";
                //ServiceTypeProvider.ServerIp = "127.0.0.1";
                ServiceTypeProvider.UpdatePort = 9000;
                ServiceTypeProvider.QueryPort = 9001;

                var model = ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy;
            }).Wait();
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
