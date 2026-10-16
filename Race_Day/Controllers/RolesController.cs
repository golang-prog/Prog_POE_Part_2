using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Race_Day.Data;
using Race_Day.Models;

namespace Race_Day.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        public readonly ApplicationDbcontext dbcontext;

        public RolesController(ApplicationDbcontext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        // GET: api/roles
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var roles = dbcontext.roles.ToList();
            return Ok(roles);
        }

        // GET: api/roles/5
        [HttpGet("{id}")]
        public IActionResult GetRoleById(int id)
        {
            var role = dbcontext.roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
                return NotFound($"Role with Id {id} not found");

            return Ok(role);
        }

        // POST: api/roles
        [HttpPost]
        public IActionResult AddNewRole(NewRoleDTO nrDTO)
        {
            if (nrDTO == null)
                return BadRequest("Role data is null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newRole = new Role
            {
                Name = nrDTO.Name,
                Description = nrDTO.Description,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            dbcontext.roles.Add(newRole);
            dbcontext.SaveChanges();

            return CreatedAtAction(nameof(GetRoleById), new { id = newRole.Id }, newRole);
        }

        // PUT: api/roles/5
        [HttpPut("{id}")]
        public IActionResult UpdateRole(int id, NewRoleDTO nrDTO)
        {
            if (nrDTO == null)
                return BadRequest("Role data is null");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingRole = dbcontext.roles.FirstOrDefault(r => r.Id == id);
            if (existingRole == null)
                return NotFound($"Role with Id {id} not found");

            existingRole.Name = nrDTO.Name;
            existingRole.Description = nrDTO.Description;
            existingRole.UpdatedAt = DateTime.UtcNow;

            dbcontext.roles.Update(existingRole);
            dbcontext.SaveChanges();

            return Ok(existingRole);
        }

        // DELETE: api/roles/5
        [HttpDelete("{id}")]
        public IActionResult DeleteRole(int id)
        {
            var role = dbcontext.roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
                return NotFound($"Role with Id {id} not found");

            dbcontext.roles.Remove(role);
            dbcontext.SaveChanges();

            return Ok(new { message = $"Role with Id {id} deleted successfully" });
        }

        // DELETE: api/roles/soft-delete/5
        [HttpDelete("soft-delete/{id}")]
        public IActionResult SoftDeleteRole(int id)
        {
            var role = dbcontext.roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
                return NotFound($"Role with Id {id} not found");

            role.IsActive = false;
            role.UpdatedAt = DateTime.UtcNow;

            dbcontext.roles.Update(role);
            dbcontext.SaveChanges();

            return Ok(new { message = $"Role with Id {id} deactivated successfully" });
        }
    }
}
