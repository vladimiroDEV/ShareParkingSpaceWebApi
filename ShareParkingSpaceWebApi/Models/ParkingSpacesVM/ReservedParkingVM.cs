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

        public string lng { get; set; }
        public string  lat { get; set; }

    }
}
