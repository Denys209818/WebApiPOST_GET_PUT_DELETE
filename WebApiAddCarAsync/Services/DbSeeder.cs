using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAddCarAsync.Entities;

namespace WebApiAddCarAsync.Services
{
    public static class DbSeeder
    {
        private static EFDbContext context { get; set; } = null;
        public static void SeedAll(this IApplicationBuilder app) 
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope()) 
            {
                context ??= scope.ServiceProvider.GetRequiredService<EFDbContext>();
                SeedCars(context);
            }
        }
        private static void SeedCars(EFDbContext context) 
        {
            if (!context.Cars.Any()) 
            {
                context.Cars.AddRange(new List<AppCar> { 
                    new AppCar 
                    {
                        Mark = "Mercedes",
                        Model = "GLA",
                        Age = 2021,
                        Capacity = 3.1F,
                        Fuel = "Бензин",
                        Image = "https://www.mercedes-benz.ua/passengercars/mercedes-benz-cars/" +
                        "models/gla/gla-h247/specifications/alternative-drive/_" +
                        "jcr_content/par/productinfotextimage/media2/slides/videoimageslide/image.MQ6.7.20201208163725.jpeg"
                    },
                    new AppCar 
                    {
                        Mark = "BMW",
                        Model = "X7",
                        Age = 2021,
                        Capacity = 3.1F,
                        Fuel = "Бензин",
                        Image = "https://www.bmw.ua/content/dam/bmw/common/all-models/x-series/x7/2018/Inspire/bmw-x7-inspire-radiating-presence-01.jpg"
                    },
                    new AppCar 
                    {
                        Mark = "Toyota",
                        Model = "RAV4",
                        Age = 2021,
                        Capacity = 3.1F,
                        Fuel = "Бензин",
                        Image = "https://www.ixbt.com/img/n1/news/2020/5/1/rav-4-prime-1280x720_large.jpg"
                    }
                });

                context.SaveChanges();
            }
        }
    }
}
