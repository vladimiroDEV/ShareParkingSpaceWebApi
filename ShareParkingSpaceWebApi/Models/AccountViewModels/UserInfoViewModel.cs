using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ShareParkingSpaceWebApi.Models.AccountViewModels
{
    public class UserInfoViewModel
    {
        [Required]
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public double Credits { get; set; }

        public Auto Auto { get; set; }
    }
}
