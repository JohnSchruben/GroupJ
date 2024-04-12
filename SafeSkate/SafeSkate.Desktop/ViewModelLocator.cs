using Unity;

namespace SafeSkate.Desktop
{
    internal class ViewModelLocator
    {
        private IUnityContainer container;

        private static ViewModelLocator instance;
        public static ViewModelLocator Instance => instance ?? (instance = new ViewModelLocator());

        public ViewModelLocator()
        {
            this.container = new UnityContainer();
            this.container.RegisterType<MainWindowViewModel>();
            this.container.AddExtension(new SafeSkateContainerExtension());
            this.container.AddExtension(new ServiceContainerExtension("172.214.88.163", 9000, 9001));
        }

        public MainWindowViewModel MainWindowViewModel => this.container.Resolve<MainWindowViewModel>();
        public MapMarkerInfo test => this.container.Resolve<MapMarkerInfo>();
    }
}