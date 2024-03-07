using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class VerifyLocation
    {
        static bool testVerifyLocation(Coordinate c1)
        {
            if (isDuplicateCoord(c1) == false)
            {
                if (isStreet(c1) == true)
                    return true;
                else if (isNearStreet(c1) == true)
                    return true;
                else return false;
            }
            else return false;
        }

        static bool isDuplicateCoord(Coordinate c1)
        {
            return false;
        }
        static bool isStreet(Coordinate c1)
        {
            return true;
        }
        static bool isNearStreet(Coordinate c1)
        {
            return true;
        }
    }
}
