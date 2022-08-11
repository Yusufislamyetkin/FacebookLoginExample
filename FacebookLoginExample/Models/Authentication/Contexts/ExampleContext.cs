using FacebookLoginExample.Models.Authentication.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookLoginExample.Models.Authentication.Contexts
{
    public class ExampleContext : IdentityDbContext<AppUser, AppRole, int>
    {
        public ExampleContext(DbContextOptions options) : base(options)
        {
        }
    }
}
