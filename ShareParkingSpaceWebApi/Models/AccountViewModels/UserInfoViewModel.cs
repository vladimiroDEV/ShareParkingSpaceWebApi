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
        public string Email { get; set; }
        public string UserId { get; set; }

        public double Credits { get; set; }

        public Auto Auto { get; set; }
    }
    public class UserProfileViewModel
    {
        [Required]
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
    public class UserAutoViewModel
    {
        public string CarBrend { get; set; }
        public string CarModel { get; set; }
        public string CarColor { get; set; }
        public string NumberPlate { get; set; }
    }

    public class UserCreditViewModel
    {
        public double Credit { get; set; }
        public CreditAction Action { get; set; }
    }
}
