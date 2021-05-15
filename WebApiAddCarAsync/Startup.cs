using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAddCarAsync.Entities;
using WebApiAddCarAsync.Entities.Identities;
using WebApiAddCarAsync.Services;

namespace WebApiAddCarAsync
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();

            services.AddDbContext<EFDbContext>((DbContextOptionsBuilder builder) => {
                builder.UseNpgsql(Configuration.GetConnectionString("DefaultCollection"));
            });

            services.AddIdentity<AppUser, AppRole>((IdentityOptions opt) => {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 5;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<EFDbContext>()
                .AddDefaultTokenProviders();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI((SwaggerUIOptions opt) => {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Add Cars Project");
            });

            app.SeedAll();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
