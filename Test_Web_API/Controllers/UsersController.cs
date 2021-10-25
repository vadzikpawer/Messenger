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
        User user = new User();
        public UsersController(Models.AppDbContext context)
        {

            db = context;
            if (!db.Users.Any())
            {
                db.Users.Add(new User { Name = "test1", Online = false , Pass = "test1" });
                db.Users.Add(new User { Name = "test2", Online = false, Pass = "test2" });
                db.SaveChanges();
            }
            Console.WriteLine("User controller up");

        }

        // GET api/users/all
        [HttpGet("all")]

        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            List<User> Users = await db.Users.ToListAsync();

            for (int i = 0; i < Users.Count; i++) Users[i].Pass = null;

            return Users;
        }

        // GET api/users/
        [HttpPost("login")]
        public async Task<ActionResult<User>> Get(User user_in)
        {
            try
            {
                user = await db.Users.FirstAsync(x => (x.Name == user_in.Name && x.Pass == user_in.Pass));
                Console.WriteLine($"Get_user!!!!!");
                if (user == null)
                {
                    Console.WriteLine($"User Not found");
                    return NotFound();
                }
            }
            catch (InvalidOperationException ex)
            {
                return NotFound();
            }
            user.Online = true;
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return new ObjectResult(user);
        }
        [HttpPost("logout")]
        public async Task<ActionResult<User>> logout(User user_in)
        {

            user = await db.Users.FirstAsync(x => (x.Name == user_in.Name && x.Pass == user_in.Pass));
            Console.WriteLine($"Get_user!!!!!");
            if (user == null)
            {
                Console.WriteLine($"User Not found");
                return NotFound();
            }

            user.Online = false;
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return new ObjectResult(user);
        }

        [HttpGet("getuser/{id}")]
        public async Task<ActionResult<User>> Get(int id)
        {
            User user = await db.Users.FirstAsync(x => x.Id == id);

            User user_out = new Models.User();
            user_out.Id = user.Id;
            user_out.Name = user.Name;


            if (user == null)
            {
                return NotFound();
            }
            return new ObjectResult(user_out);
        }
        /*
                [HttpGet("finduser/{name}")]
                public async Task<ActionResult<User>> Get(string name)
                {
                    User user = await db.Users.FirstAsync(x => x.Name == name);

                    User user_out = new Models.User();
                    user_out.Id = user.Id;
                    user_out.Age = user.Age;
                    user_out.Email = user.Email;
                    user_out.Name = user.Name;
                    user_out.Friends_IDs = user.Friends_IDs;

                    if (user == null)
                    {
                        return NotFound();
                    }
                    return new ObjectResult(user_out);
                }*/

        // POST api/users
        [HttpPost("add")]
        public async Task<ActionResult<User>> Post([FromBody] User user)
        { 
            User user_in = await db.Users.FirstAsync(x => x.Name == user.Name);

            if (user_in != null) return BadRequest("User exist");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                user.Online = true;
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return Ok(user);
            }
        }

        /*[HttpPost("addfriend/id1={id1}&id2={id2}")]
        public async Task<ActionResult<User>> Post(int id1, int id2)
        {
            User user1 = await db.Users.FirstAsync<User>(x => x.Id == id1);
            User user2 = await db.Users.FirstAsync<User>(x => x.Id == id2);

            List<int> friend_list = IntListToString.StringToList(user1.Friends_IDs);
            if (friend_list == null) friend_list = new List<int>();
            friend_list.Add(id2);
            user1.Friends_IDs = IntListToString.ListToString(friend_list);

            friend_list = IntListToString.StringToList(user2.Friends_IDs);
            if (friend_list == null) friend_list = new List<int>();
            friend_list.Add(id1);
            user2.Friends_IDs = IntListToString.ListToString(friend_list);

            db.Update(user1);
            db.Update(user2);
            await db.SaveChangesAsync();
            return Ok(user1);
        }*/

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

            user.Pass = db.Users.First(x => x.Id == user.Id).Pass;

            db.Users.Update(user);
            await db.SaveChangesAsync();
            return Ok(user);
        }

        /* // DELETE api/users/5
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
         }*/

    }
}
