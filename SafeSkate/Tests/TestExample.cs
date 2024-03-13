using SafeSkate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal class TestExample : ITest
    {
        private Coordinate coordinate;
        public TestExample(Coordinate coordinate)
        {
            this.coordinate = coordinate;
        }

        public string RunTest()
        {
            // check something 
            return $"the test was inconclusive...X:{coordinate.Latitude},Y:{coordinate.Longitude}";
        }
    }
}
