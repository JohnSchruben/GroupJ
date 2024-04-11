
namespace SafeSkate.Mobile
{
    /// <summary>
    ///   Ticket #8 The Mobile App
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Task.Run(async () =>
            {
                ServiceTypeProvider.ServerIp = "172.214.88.163";
                ServiceTypeProvider.UpdatePort = 9000;
                ServiceTypeProvider.QueryPort = 9001;

                var model = ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy;
            }).Wait();

            InitializeComponent();

            MainPage = new MainPage();
        }
    }
}
