using Unity;
using Unity.Extension;

namespace SafeSkate.Desktop
{
    public class DesktopContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            this.Container.RegisterType<MainWindowViewModel>();
        }
    }
}