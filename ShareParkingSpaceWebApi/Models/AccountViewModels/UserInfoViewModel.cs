using ShareParkingSpaceWebApi.Models.Helpers;
using System.ComponentModel.DataAnnotations;

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

    public class UpdateUserCreditViewModel
    {
        public double Credit { get; set; }
        public CreditAction Action { get; set; }
    }
}
