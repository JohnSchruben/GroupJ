namespace SafeSkate.Mobile
{
    public partial class App : Application
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

            MainPage = new AppShell();

        }
    }
}
