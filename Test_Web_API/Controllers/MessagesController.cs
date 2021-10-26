using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<ChatHub> _hubContext;
        public MessagesController(AppDbContext context, IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            Console.WriteLine("message controller up");
            debug = true;
            db = context;
            if (debug)
            {
                if (!db.Messages.Any())
                {
                    db.Messages.Add(new Message { From = 1, FromName = "test1", To = 2, Text = "test", dateStapm = DateTime.UtcNow });
                    db.Messages.Add(new Message { From = 2, FromName = "test2", To = 1, Text = "test1", dateStapm = DateTime.UtcNow });
                    db.Messages.Add(new Message { From = 1, FromName = "test1", To = 2, Text = "test2", dateStapm = DateTime.UtcNow });
                    db.Messages.Add(new Message { From = 2, FromName = "test2", To = 1, Text = "test3", dateStapm = DateTime.UtcNow });
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

            List<Message> messages = await db.Messages.Where(x => (x.From == message_in.From && x.To == message_in.To)
                                                            || (x.To == message_in.From && x.From == message_in.To))
                                                            .ToListAsync();

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
