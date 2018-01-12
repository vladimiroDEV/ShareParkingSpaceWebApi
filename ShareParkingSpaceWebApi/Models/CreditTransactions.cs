using ShareParkingSpaceWebApi.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ShareParkingSpaceWebApi.Models
{
    public class CreditTransactions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CreditTransactionsID { get; set; }

        public string UserSrc { get; set; }
        public string UserDist { get; set; }

        public DateTime Date { get; set; }

        public CreditAction  Action  { get; set; }

        public double Amount { get; set; }

    }
   
}
