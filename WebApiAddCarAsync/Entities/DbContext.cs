using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAddCarAsync.Entities.Identities;

namespace WebApiAddCarAsync.Entities
{
    public class EFDbContext : IdentityDbContext<AppUser, AppRole, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, 
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public EFDbContext(DbContextOptions<EFDbContext> options) : base(options)
        {

        }

        public DbSet<AppCar> Cars { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUserRole>(userRoles => {
                userRoles.HasKey(primaryKeys => new { primaryKeys.RoleId, primaryKeys.UserId });

                userRoles.HasOne(virtualElementWithEntityToVirtualCollection => virtualElementWithEntityToVirtualCollection.Role)
                .WithMany(virtualCollectionWithEntityToVirtualElement => virtualCollectionWithEntityToVirtualElement.UserRoles)
                .HasForeignKey(intParamWithForeignKeySettings => intParamWithForeignKeySettings.RoleId)
                .IsRequired();

                userRoles.HasOne(virtualElementWithEntityToVirtualCollection => virtualElementWithEntityToVirtualCollection.User)
                .WithMany(virtualCollectionWithEntityToVirtualElement => virtualCollectionWithEntityToVirtualElement.UserRoles)
                .HasForeignKey(intParamWithForeignKeySettings => intParamWithForeignKeySettings.UserId)
                .IsRequired();
            });
        }
    }
}
