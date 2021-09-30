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

    public class MessagesController : ControllerBase
    {
        AppDbContext db;
        bool debug;
        public MessagesController(AppDbContext context)
        {
            Console.WriteLine("message controller up");
            debug = true;
            db = context;
            if (debug)
            {
                if (!db.Messages.Any())
                {
                    db.Messages.Add(new Message { From = "test", To = "test", Text = "test" });
                    db.Messages.Add(new Message { From = "test", To = "test", Text = "test1" });
                    db.Messages.Add(new Message { From = "test", To = "test", Text = "test2" });
                    db.Messages.Add(new Message { From = "test1", To = "test1", Text = "test1" });
                    db.SaveChanges();
                }

                Console.WriteLine("message controller up");
            }
        }

        [HttpPost("get")]
        public async Task<ActionResult<Message>> Get(Message message_in)
        {
            if (debug)
            {
                Console.WriteLine(message_in);
            }

            List<Message> messages = await db.Messages.Where(x => x.From == message_in.From && x.To == message_in.To).ToListAsync();

            if (messages == null)
            {
                return NotFound();
            }

            return new ObjectResult(messages);
        }

        // POST api/messages
        [HttpPost("add")]
        public async Task<ActionResult<Message>> Post(Message message)
        {
            Console.WriteLine(message.ToString());
            if (message == null)
            {
                return BadRequest();
            }

            message.dateStapm = new DateTime();
            message.dateStapm = DateTime.UtcNow;
            db.Messages.Add(message);
            await db.SaveChangesAsync();
            return Ok(message);
        }

        // PUT api/messages
        [HttpPut]
        public async Task<ActionResult<Message>> Put(Message user)
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

    }
}
