using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockServer.Model
{
    public class Vehicle
    {
        public string ID { get; set; }
        public bool IsAvailable { get; set; }
        public Location CurrentLocation { get; set; }
    }
}
