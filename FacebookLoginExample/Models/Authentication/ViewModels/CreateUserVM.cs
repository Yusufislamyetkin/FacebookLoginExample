using FacebookLoginExample.Models.Authentication.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookLoginExample.Models.Authentication.ViewModels
{
    public class CreateUserVM
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public static implicit operator AppUser(CreateUserVM createUserVM)
        {
            return new AppUser
            {
                UserName = createUserVM.Username,
                Email = createUserVM.Email
            };
        }
    }
}
