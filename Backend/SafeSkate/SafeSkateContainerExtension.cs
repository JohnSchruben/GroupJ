using Unity;
using Unity.Extension;

namespace SafeSkate
{
    public class SafeSkateContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            this.Container.RegisterType<Coordinate>();
        }
    }
}