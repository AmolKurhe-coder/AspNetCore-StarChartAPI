using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            CelestialObject celestialObject = _context.CelestialObjects.Find(id);

            if (celestialObject == null)
                return NotFound();

            celestialObject.Satellites.Add(celestialObject);

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            IEnumerable<CelestialObject> celestialObject = _context.CelestialObjects.Where(s => s.Name == name);

            if (!celestialObject.Any())
                return NotFound();

            foreach (var obj in celestialObject)
            {
                obj.Satellites.Add(obj);
            }

            return Ok(celestialObject);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<CelestialObject> celestialObjects = _context.CelestialObjects.ToList();

            foreach (var obj in celestialObjects)
            {
                obj.Satellites.Add(obj);
            }

            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {

            var createdObj = _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = createdObj.Entity.Id }, createdObj.Entity);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            CelestialObject celestialObjectToUpdate = _context.CelestialObjects.Find(id);

            if (celestialObjectToUpdate == null)
                return NotFound();

            celestialObjectToUpdate.Name = celestialObject.Name;
            celestialObjectToUpdate.OrbitedObjectId = celestialObject.OrbitedObjectId;

            var updatedObj = _context.Update(celestialObjectToUpdate);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            CelestialObject celestialObjectToUpdate = _context.CelestialObjects.Find(id);

            if (celestialObjectToUpdate == null)
                return NotFound();

            celestialObjectToUpdate.Name = name;

            var updatedObj = _context.Update(celestialObjectToUpdate);

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            IEnumerable<CelestialObject> celestialObjects = _context.CelestialObjects.Where(s => s.Id == id || s.OrbitedObjectId == id);

            if (!celestialObjects.Any())
                return NotFound();

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
