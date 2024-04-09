using Unity;

namespace SafeSkate.Mobile
{
    internal class ViewModelLocator
    {
        private IUnityContainer container;

        private static ViewModelLocator instance;
        public static ViewModelLocator Instance => instance ?? (instance = new ViewModelLocator());

        public ViewModelLocator()
        {
            this.container = new UnityContainer();
            this.container.RegisterType<MainPageViewModel>();
            this.container.AddExtension(new SafeSkateContainerExtension());
            //this.container.AddExtension(new ServiceContainerExtension("172.214.88.163", 9000, 9001));
            this.container.AddExtension(new ServiceContainerExtension("127.0.0.1", 9000, 9001));
        }

        public MainPageViewModel MainWindowViewModel => this.container.Resolve<MainPageViewModel>();
    }
}