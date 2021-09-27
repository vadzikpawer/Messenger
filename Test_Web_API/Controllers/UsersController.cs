using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test_Web_API.Models;

namespace Test_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        Models.AppDbContext db;
        public UsersController(Models.AppDbContext context)
        {

            db = context;
            if (!db.Users.Any())
            {
                db.Users.Add(new User { Name = "test", Pass = "test", Age = 12 });
                db.Users.Add(new User { Name = "test1", Pass = "test1", Age = 2 });
                db.SaveChanges();
            }
            Console.WriteLine("User controller up");

        }

        [HttpGet("all")]

        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            return await db.Users.ToListAsync();
        }

        // GET api/users/
        [HttpPost("login")]
        public async Task<ActionResult<User>> Get(User user_in)
        {

            User user = await db.Users.FirstAsync(x => (x.Name == user_in.Name || x.Email == user_in.Email) && x.Pass == user_in.Pass); ;

            if (user == null)
            {
                return NotFound();
            }
            return new ObjectResult(user);
        }

        [HttpPost("getuser/{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            User user = await db.Users.FirstAsync(x => x.Id == id);

            User user_out = new Models.User();
            user_out.Id = user.Id;
            user_out.Age = user.Age;
            user_out.Email = user.Email;
            user_out.Name = user.Name;

            if (user == null)
            {
                return NotFound();
            }
            return new ObjectResult(user_out);
        }

        // POST api/users
        [HttpPost("adduser")]
        public async Task<ActionResult<User>> Post(User user)
        {
            User user_check = await db.Users.FirstAsync<User>(x => x.Name == user.Name || x.Email == user.Email && user.Email != null);

            if (user_check.Name == user.Name)
            {
                ModelState.AddModelError("Name", "Пользователь с таким именем уже существует");
            }
            else if (user_check.Email == user.Email)
            {
                ModelState.AddModelError("Email", "Пользователь с таким Email уже существует");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return Ok(user);
            }
        }

        // PUT api/users/
        [HttpPut]
        public async Task<ActionResult<User>> Put(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (!db.Users.Any(x => x.Id == user.Id))
            {
                return NotFound();
            }

            db.Update(user);
            await db.SaveChangesAsync();
            return Ok(user);
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> Delete(int id)
        {
            User user = db.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return Ok(user);
        }

    }
}
