using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAddCarAsync.Entities;

namespace WebApiAddCarAsync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private EFDbContext _context;
        public CarController(EFDbContext context)
        {
            _context = context;
        }

        [HttpGet, Route("getall")]
        public async Task<IActionResult> GetAllCars() 
        {
            return await Task.Run(() => { 
                return Ok(_context.Cars.AsQueryable());
            });
        }

        [HttpPost, Route("add")]
        public async Task<IActionResult> AddCarToCollection([FromBody] AppCar Car) 
        {
           return await Task.Run(() => {
               var appcar = new AppCar
               {
                   Model = Car.Model,
                   Mark = Car.Mark,
                   Age = Car.Age,
                   Capacity = Car.Capacity,
                   Fuel = Car.Fuel,
                   Image = Car.Image
               };
               _context.Cars.Add(appcar);

               _context.SaveChanges();
               return Ok(appcar.Id.ToString());
               });
        }

        [HttpPut, Route("update")]
        public async Task<IActionResult> UpdateCarData([FromBody] AppCar Car) 
        {
            return await Task.Run(() => {
                var car = _context.Cars.FirstOrDefault(x => x.Id == Car.Id);
                if (car != null) 
                {
                car.Mark = Car.Mark;
                car.Model = Car.Model;
                car.Image = Car.Image;
                car.Fuel = Car.Fuel;
                car.Capacity = Car.Capacity;
                car.Age = Car.Age;

                _context.SaveChanges();
                }

                return Ok(new { result = "Дані відредаговано!" });
            });
        }

        [HttpDelete, Route("delete")]
        public async Task<IActionResult> DeleteCar([FromBody] int Id) 
        {
            return await Task.Run(() => {
                AppCar car =_context.Cars.FirstOrDefault(x => x.Id == Id);
                if (car != null) 
                {
                    _context.Cars.Remove(car);
                    _context.SaveChanges();
                }
                return Ok("Успішно видалено елемент!");
            });
        }
    }
}
