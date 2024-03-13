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
    public class QueryMapItems : ITest
    {
        private MapMarkerInfoCollectionProxy model;
        public QueryMapItems(MapMarkerInfoCollectionProxy model) 
        {
            this.model = model;
        }
        public string RunTest()
        {
            var mapItems = this.model.MapMarkerInfos;
            if (mapItems.Count > 0)
            {
                return "query test passed";
            }

            return "query test failed";
        }
    }
}
