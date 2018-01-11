using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareParkingSpaceWebApi.Models.Helpers
{
    public enum ParkingSpaceState
    {
        Free = 1,
        Reserverd = 2
    }
    public  enum ParkingSpaceAction
    {
        Create =1,
        Modified =2,
        Used =3 ,   // quando il posto aout è stato usato 
        Deleted = 4
    }
}
