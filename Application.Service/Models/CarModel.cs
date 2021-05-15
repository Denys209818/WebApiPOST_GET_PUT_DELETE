using System;
using System.Collections.Generic;
using System.Text;

namespace AppService.Models
{
    public class CarModel
    {
        public int Id { get; set; } = 0;
        public string Mark { get; set; }
        public string Model { get; set; }
        public int Age { get; set; }
        public string Fuel { get; set; }
        public float Capacity { get; set; }
        public string Image { get; set; }
        public bool IsNew { get; set; } = false;
    }
}
