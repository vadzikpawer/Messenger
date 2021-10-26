using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test_Web_API.Controllers;

namespace Test_Web_API.Models
{
    public class ChatHub : Hub
    {

        AppDbContext db;
        Random rnd = new Random();

        public ChatHub(AppDbContext context)
        {
            db = context;
            db = context;
            if (!db.Users.Any())
            {
                db.Users.Add(new User { Name = "test1", Online = false, Pass = "test1", Color = UsersController.Colors[rnd.Next(UsersController.Colors.Count())] });
                db.Users.Add(new User { Name = "test2", Online = false, Pass = "test2", Color = UsersController.Colors[rnd.Next(UsersController.Colors.Count())] });
                db.SaveChanges();
            }
        }
        public async Task Send(Message message)
        {
            await Clients.Caller.SendAsync("SendMessage", message);
            Console.WriteLine("WEEEEEEEEEEEEEEEBBBBB SOCKEEEEEEEEEEEEEEEEEEEEEEET");
            message.dateStapm = new DateTime();
            message.dateStapm = DateTime.UtcNow;
            db.Messages.Add(message);
            await db.SaveChangesAsync();
            string _connectionID = db.Users.First(x => x.Id == message.To).ConnectionID;
            if (_connectionID != null)
            {
                Console.WriteLine("New Message");
                await Clients.Clients(_connectionID).SendAsync("NewMessage", message);
            }
        }

        public async Task GetLastMessage(Message message)
        {
            Console.WriteLine("GetLastMessage");
            try
            {
                List<Message> Messages = await db.Messages.Where(x => (x.From == message.From && x.To == message.To) || (x.To == message.From && x.From == message.To)).ToListAsync();
                Message LastMessage = Messages.Last();
                DateTime DateStamp = LastMessage.dateStapm;
                await Clients.Caller.SendAsync("RecieveLastMessage", DateStamp, message.To);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Clients.Caller.SendAsync("NoMessage", "");
            }
        }

        public async Task GetMessages(Message message)
        {
            Console.WriteLine("GetMessages");
            try
            {
                List<Message> Messages = await db.Messages.Where(x => (x.From == message.From && x.To == message.To) || (x.To == message.From && x.From == message.To)).ToListAsync();
                await Clients.Caller.SendAsync("ReceiveMessages", Messages);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Clients.Caller.SendAsync("NoMessage", "");
            }
        }

        public async Task Login(User user)
        {
            Console.WriteLine("Login");
            try
            {
                User usertemp = await db.Users.FirstAsync(x => x.Name == user.Name && user.Pass == x.Pass);
                if (usertemp != null)
                {

                    usertemp.Pass = null;
                    await Clients.Caller.SendAsync("LoginSuccess", usertemp);

                    usertemp.Pass = user.Pass;
                    usertemp.ConnectionID = Context.ConnectionId;
                    usertemp.Online = true;
                    db.Users.Update(usertemp);
                    db.SaveChanges();

                    List<User> Users = await db.Users.ToListAsync();
                    for (int i = 0; i < Users.Count; i++)
                    {
                        Users[i].Pass = null;
                        Users[i].ConnectionID = null;
                    }
                    await Clients.All.SendAsync("UpdateUser", Users);
                }
                else
                {
                    Console.WriteLine("NoUser");
                    await Clients.Caller.SendAsync("LoginError", "NoUser");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Clients.Caller.SendAsync("LoginError", "NoUser");
            }
        }

        public async Task LogOut(User _user)
        {
            Console.WriteLine($"LogOUUT!!!!!");
            try
            {
                User user = await db.Users.FirstAsync(x => (x.Id == _user.Id));
                user.Online = false;
                user.ConnectionID = null;
                db.Users.Update(user);
                db.SaveChanges();
            }
            catch (Exception ex) { };
            List<User> Users = await db.Users.ToListAsync();
            for (int i = 0; i < Users.Count; i++)
            {
                Users[i].Pass = null;
                Users[i].ConnectionID = null;
            }
            await Clients.All.SendAsync("UpdateUser", Users);

        }

        public async Task Register(User _user)
        {
            try
            {
                User user = await db.Users.FirstAsync(x => x.Name == _user.Name);
                Console.WriteLine("UserExist");
                await Clients.Caller.SendAsync("UserExist", "");
            }
            catch
            {

                _user.Online = true;
                _user.ConnectionID = Context.ConnectionId;
                _user.Color = UsersController.Colors[rnd.Next(UsersController.Colors.Count())];
                db.Users.Add(_user);
                await Clients.Caller.SendAsync("RegisterSuccess", _user);
                await db.SaveChangesAsync();

                List<User> Users = await db.Users.ToListAsync();
                for (int i = 0; i < Users.Count; i++)
                {
                    Users[i].Pass = null;
                    Users[i].ConnectionID = null;
                }
                await Clients.All.SendAsync("UpdateUser", Users);
                
            }  
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("GetConnectionId", Context.ConnectionId);
            await base.OnConnectedAsync();
        }
    }
}
