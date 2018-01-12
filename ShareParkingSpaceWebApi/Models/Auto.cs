using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShareParkingSpaceWebApi.Models
{
    public class Auto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AutoID { get; set; }

        public string UderID { get; set; }
        public string CarBrend { get; set; }
        public string CarModel { get; set; }
        public string CarColor { get; set; }
        public string NumberPlate { get; set; }
    }
}
