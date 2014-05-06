using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Insya.NetDash.Models
{
    public class User
    {
        public int UserId { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        public User()
        {
        }

        public User(int id, string name, string pass)
        {
            UserId = id;
            Username = name;
            Password = pass;
        }
    }

    public class UserViewModel
    {

        public int UserId { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

}
