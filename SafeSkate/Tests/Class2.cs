using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class Class2
    {
        static bool Main(string[] args)
        {
            def testVerifyLocation(coordinate c1)
            {
                if (isDuplicateCoord(c1) == false​)
                {
                    if (isStreet(c1) == true)
                        return true;
                    else if (isNearStreet(c1) == true​)
                        return true​;
                    else return false​;
                }
                else return false;
            }
        }
    }
}
