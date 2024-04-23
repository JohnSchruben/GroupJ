namespace SafeSkate.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            OnStart();
            InitializeComponent();
            MainPage = new MainPage();
        }

        protected override async void OnStart()
        {
            // Perform your initialization here
            await InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                Task.Run(async () =>
                {
                    ServiceTypeProvider.ServerIp = "172.214.88.163";
                    ServiceTypeProvider.UpdatePort = 9000;
                    ServiceTypeProvider.QueryPort = 9001;

                    var model = ServiceTypeProvider.Instance.MapMarkerInfoCollectionProxy;
                }).Wait();

            }
            catch (Exception ex)
            {
                ShowErrorDialog(ex);
            }
        }

        private void ShowErrorDialog(Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            });
        }
    }
}
