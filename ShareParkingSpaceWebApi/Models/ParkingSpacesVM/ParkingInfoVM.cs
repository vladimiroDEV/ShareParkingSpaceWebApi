﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareParkingSpaceWebApi.Models.ParkingSpacesVM
{
    public class ParkingInfoVM
    {
        public long ParkingID { get; set; }
        public Auto UserAuto { get; set; }
        public string username { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
 
    }
}
