using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiAddCarAsync.Entities.Identities
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public virtual AppUser User { get; set; }
        public virtual AppRole Role { get; set; }
    }
}
