using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SI_MicroServicos.Model;

namespace SI_Microservicos.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles="SuperAdministrator")]
    public class UserProfilesController : ControllerBase
    {
        private readonly _DbContext _context;

        public UserProfilesController(_DbContext context)
        {
            _context = context;
        }

        // GET: api/UserProfiles
       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/UserProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfile>> GetUserProfile(Guid id)
        {
            
            var userProfile = await _context.Users.FindAsync(id);

            if (userProfile == null)
            {
                return NotFound();
            }

            return userProfile;
        }

        // PUT: api/UserProfiles/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserProfile(Guid id, UserProfile userProfile)
        {
            if (id != userProfile.ID)
            {
                return BadRequest();
            }

            _context.Entry(userProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserProfileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserProfiles
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<UserProfile>> PostUserProfile(UserProfile userProfile)
        {
        
            _context.Users.Add(userProfile);
            await _context.SaveChangesAsync();

            _context.LogAuditorias.Add(
                new LogAuditoria{
                User = userProfile.Username,
                DetalhesAuditoria = string.Concat("Efetuou Login no sistema: ", userProfile.Sistema,"Data do login : ", DateTime.Now.ToLongDateString())

            });
             await _context.SaveChangesAsync();
             

            return CreatedAtAction("GetUserProfile", new { id = userProfile.ID }, userProfile);
        }

        // DELETE: api/UserProfiles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserProfile>> DeleteUserProfile(Guid id)
        {
            var userProfile = await _context.Users.FindAsync(id);
            if (userProfile == null)
            {
                return NotFound();
            }

            _context.Users.Remove(userProfile);
            await _context.SaveChangesAsync();

            return userProfile;
        }

        private bool UserProfileExists(Guid id)
        {
            return _context.Users.Any(e => e.ID == id);
        }
}
}
