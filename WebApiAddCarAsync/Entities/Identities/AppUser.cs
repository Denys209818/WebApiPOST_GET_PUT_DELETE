using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAddCarAsync.Entities.Identities
{
    public class AppUser : IdentityUser<int>
    { 
        public ICollection<AppUserRole> UserRoles { get; set; }

    }
}
