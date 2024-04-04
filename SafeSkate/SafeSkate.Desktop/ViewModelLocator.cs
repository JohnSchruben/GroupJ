using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace SafeSkate.Desktop
{
    internal class ViewModelLocator
    {
        IUnityContainer container;
        
        private static ViewModelLocator instance;
        public static ViewModelLocator Instance => instance ?? (instance = new ViewModelLocator());
        public ViewModelLocator() 
        {
            this.container = new UnityContainer();
            this.container.RegisterType<MainWindowViewModel>();
            this.container.AddExtension(new SafeSkateContainerExtension());
            this.container.AddExtension(new ServiceContainerExtension("127.0.0.1", 9000, 9001));
        }   

        public MainWindowViewModel MainWindowViewModel => this.container.Resolve<MainWindowViewModel>();
    }
}
