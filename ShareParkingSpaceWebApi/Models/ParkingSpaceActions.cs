using ShareParkingSpaceWebApi.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShareParkingSpaceWebApi.Models
{
    public class ParkingSpaceActions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ParkingSpaceActionsID { get; set; }
        public DateTime DateAction { get; set; }

        public ParkingSpaceAction Action { get; set; }

        public long ID { get; set; }
        public string UserID { get; set; }

       // public virtual Auto Auto { get; set; }
        public long AutoID { get; set; }

        //public virtual Auto ReservedAuto { get; set; }
        public long ReservedAutoID { get; set; }

        public string Lat { get; set; }
        public string Long { get; set; }
        public string Location { get; set; }
    }
}
