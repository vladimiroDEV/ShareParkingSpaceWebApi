using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareParkingSpaceWebApi.Models.ParkingSpacesVM
{
    public class MyPakingVm
    {
        public Auto UserAuto { get; set; }
        public string  Username { get; set; }

        public string Lng { get; set; }
        public string  Lat { get; set; }

    }
}
