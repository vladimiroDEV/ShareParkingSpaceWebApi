using ShareParkingSpaceWebApi.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareParkingSpaceWebApi.Models
{
    public class ParkingSpaces
    {
        public long ID { get; set; }
        public string UserID { get; set; }

        public virtual UsersAuto UsersAuto { get; set; }
        public long UsersAutoID { get; set; }

        public long? ReservedUsersAutoID { get; set; }

        public string Lat { get; set; }
        public string Long { get; set; }


        public ParkingSpaceState State { get; set; }

    }
}
