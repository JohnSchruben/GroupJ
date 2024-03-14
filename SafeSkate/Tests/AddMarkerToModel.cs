using SafeSkate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class AddMarkerToModel : ITest
    {
        private MapMarkerInfoCollectionProxy model;
        public AddMarkerToModel(MapMarkerInfoCollectionProxy model)
        {
            this.model = model;
        }
        public string RunTest()
        {
            model.AddMapMarkerInfo(new MapMarkerInfo(new Coordinate(40.7128, -74.0060, 10), "test class", DateTime.Now, Severity.Morphine));

            // count should be one more than default.
            if (model.MapMarkerInfos.Count > 1)
            {
                return "Add marker to model test passed";
            }

            return "Add marker to model test failed.";
        }
    }
}
