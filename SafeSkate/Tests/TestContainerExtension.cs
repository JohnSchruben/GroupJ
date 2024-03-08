using SafeSkate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Unity;
using Unity.Extension;

namespace Tests
{
    internal class TestContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            // adding safe skate types.
            this.Container.AddExtension(new SafeSkateContainerExtension());

            // getting all the classes in this namespace that implement ITest
            var testTypes = Assembly.GetExecutingAssembly()
             .GetTypes()
             .Where(t => t.IsClass && !t.IsAbstract && typeof(ITest).IsAssignableFrom(t) && t.Namespace == "Tests");

            // regestering each test by name. can register more complext texts by hand.
            foreach (var type in testTypes)
            {
                this.Container.RegisterType(typeof(ITest), type, type.Name);
            }
        }
    }
}
