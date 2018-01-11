using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareParkingSpaceWebApi.Models
{
    public class UsersAuto
    {
        public long ID { get; set; }
        public string CarBrend { get; set; }
        public string CarModel { get; set; }
        public string CarColor { get; set; }
        public string NumberPlate { get; set; }
    }
}
