using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafeSkate;


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
            return false; //Checks current coordinate with list of other coordinates. Returns true if a duplicate is found
        }
        static bool isStreet(Coordinate c1)
        {
            return true; //Checks if the current coordinate is on a street object on the global map. Returns true if the coordinate is on a street
        }
        static bool isNearStreet(Coordinate c1)
        {
            return true; //Checks if the current coordinate is within a certain distance from a street object on the global map. Returns true if the coordinate is near a street
        }
    }
}
