using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce_app.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name { get; set; } 
        [Required]
        public string Surname { get; set; }
        public string Adress { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }

        [NotMapped]
        public string Role { get; set; }
    }
}
