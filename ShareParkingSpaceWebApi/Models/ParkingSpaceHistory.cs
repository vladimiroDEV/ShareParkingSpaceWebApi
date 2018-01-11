using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareParkingSpaceWebApi.Models
{
    public class ParkingSpaceHistory: ParkingSpaces
    {

        public int MyProperty { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ReservationDate { get; set; }
    }
}
