using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareParkingSpaceWebApi.Models.ParkingSpacesVM
{
    public class ReservedAutoVM
    {
        public Auto UserAuto { get; set; }
        public string  Username { get; set; }
    }
}
