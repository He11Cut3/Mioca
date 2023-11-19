using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Miocas.Models
{
    public class User : IdentityUser
    {
        [DataType(DataType.Password)]
        public string Miocas_Password { get; set; }

        public ICollection<ItemCart> ItemCarts { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
