using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;
using System.Linq;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (celestialObject == null)
                return NotFound();

            celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id).ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name)
                .ToList();
            if (celestialObjects == null || celestialObjects.Count() == 0)
                return NotFound();
            foreach (var co in celestialObjects)
            {
                co.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == co.Id)
                    .ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects
                .ToList();
            foreach (var co in celestialObjects)
            {
                co.Satellites = celestialObjects.Where(c => c.OrbitedObjectId == co.Id).ToList();
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var co = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (co == null)
                return NotFound();
            co.Name = celestialObject.Name;
            co.OrbitalPeriod = celestialObject.OrbitalPeriod;
            co.OrbitedObjectId = celestialObject.Id;
            _context.CelestialObjects.Update(co);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var co = _context.CelestialObjects.FirstOrDefault(c => c.Id == id);
            if (co == null)
                return NotFound();
            co.Name = name;
            _context.CelestialObjects.Update(co);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var co = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id)
                .ToList();
            if (co.Count() == 0)
                return NotFound();

            _context.CelestialObjects.RemoveRange(co);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
