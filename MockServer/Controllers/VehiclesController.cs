using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using MockServer.Model;

namespace MockServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        // GET api/vehicles
        [HttpGet]
        public ActionResult<IEnumerable<Vehicle>> Get([FromQuery] string status = "all")
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get customer collection
                var vehicles = db.GetCollection<Vehicle>("vehicles");

                switch (status)
                {
                    case "available":
                        return Ok(vehicles.Find(x => x.IsAvailable == true));
                    case "not-available":
                        return Ok(vehicles.Find(x => x.IsAvailable == false));
                    default:
                        return Ok(vehicles.FindAll());
                }                
            }
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<Vehicle>> GetById( string id)
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get customer collection
                var vehicles = db.GetCollection<Vehicle>("vehicles");
                var vehicle = vehicles.FindById(id);

                if (vehicle != null)
                    return Ok(vehicle);
                else
                    return NotFound();                
            }
        }

        [HttpGet("nearest")]
        public ActionResult<Vehicle> GetClosestVehicle([FromQuery]string lattitude, [FromQuery]string longitude)
        {
            if (String.IsNullOrWhiteSpace(lattitude) || String.IsNullOrWhiteSpace(longitude))
                return BadRequest(new { Error = "Must provide lattitude and longitude" });

            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get customer collection
                var vehicles = db.GetCollection<Vehicle>("vehicles");
                var availableVehicles = vehicles.Find(x => x.IsAvailable == true);

                Vehicle vehicleWithMininumDistance = null;
                double distance = float.MaxValue;
                foreach(var vehicle in availableVehicles)
                {
                    var currentDistance = Location.GetDistanceBetween(vehicle.CurrentLocation,
                                    new Location()
                                    {
                                        lattitude = float.Parse(lattitude),
                                        longitude = float.Parse(longitude)
                                    });

                    if(currentDistance < distance)
                    {
                        vehicleWithMininumDistance = vehicle;
                        distance = currentDistance;
                    }
                }

                if (vehicleWithMininumDistance != null)
                    return Ok(vehicleWithMininumDistance);
                else
                    return NotFound();
            }
        }

        // GET api/vehicles
        [HttpPost("{id}/book")]
        public ActionResult<IEnumerable<Vehicle>> Book(string id)
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get customer collection
                var vehicles = db.GetCollection<Vehicle>("vehicles");
                var vehicle = vehicles.FindOne(x => x.ID == id);
                if (vehicle == null)
                    return NotFound();
                if (!vehicle.IsAvailable)
                    return Forbid();
                vehicle.IsAvailable = false;
                vehicles.Update(vehicle);
                return Ok();                
            }
        }

        // GET api/vehicles
        [HttpPost("{id}/release")]
        public ActionResult<IEnumerable<Vehicle>> Release(string id)
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get customer collection
                var vehicles = db.GetCollection<Vehicle>("vehicles");
                var vehicle = vehicles.FindOne(x => x.ID == id);
                if (vehicle == null)
                    return NotFound();
                if (vehicle.IsAvailable)
                    return Forbid();
                vehicle.IsAvailable = true;
                vehicles.Update(vehicle);
                return Ok();
            }
        }

        // POST api/vehicles
        [HttpPost]
        public void Post([FromBody] Vehicle vehicle)
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get customer collection
                var vehicles = db.GetCollection<Vehicle>("vehicles");
                vehicles.Insert(vehicle);
            }
        }

        // PUT api/vehicles/5
        [HttpPut("{id}")]
        public void Put(string id, [FromBody] Vehicle vehicle)
        {
            if(vehicle.ID != id)
            {
                throw new InvalidOperationException();
            }
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get customer collection
                var vehicles = db.GetCollection<Vehicle>("vehicles");
                vehicles.Update(vehicle);
            }
        }

        // DELETE api/vehicles/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            using (var db = new LiteDatabase(@"MyData.db"))
            {
                // Get customer collection
                var vehicles = db.GetCollection<Vehicle>("vehicles");
                vehicles.Delete(x=> x.ID == id);
            }
        }
    }
}
