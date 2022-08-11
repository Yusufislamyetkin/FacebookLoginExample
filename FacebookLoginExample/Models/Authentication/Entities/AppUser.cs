using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookLoginExample.Models.Authentication.Entities
{
    public class AppUser : IdentityUser<int>
    {
       
    }
}
