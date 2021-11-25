using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Test_Web_API.Controllers;

namespace Test_Web_API.Models
{
    [Authorize]
    public class ChatHub : Hub
    {

        AppDbContext db;
        Random rnd = new Random();

        public ChatHub(AppDbContext context)
        {
            db = context;
            if (!db.Users.Any())
            {
                string Salt1 = ComputeSalt();
                string Salt2 = ComputeSalt();
                db.Users.Add(new User { Name = "test1", Online = false, Pass = ComputeSha512Hash(ComputeSha512Hash("test1") + Salt1), Color = UsersController.Colors[rnd.Next(UsersController.Colors.Count())], Salt = Salt1 });
                db.Users.Add(new User { Name = "test2", Online = false, Pass = ComputeSha512Hash(ComputeSha512Hash("test2") + Salt2), Color = UsersController.Colors[rnd.Next(UsersController.Colors.Count())], Salt = Salt2 });
                db.SaveChanges();
            }
        }
        public async Task Send(Message message)
        {
            await Clients.Caller.SendAsync("SendMessage", message);
            Console.WriteLine("WEEEEEEEEEEEEEEEBBBBB SOCKEEEEEEEEEEEEEEEEEEEEEEET");
            message.dateStapm = new DateTime();
            message.dateStapm = DateTime.UtcNow;
            message.IsSeen = false;
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
                int unseen;
                List<Message> Messages = await db.Messages.Where(x => (x.From == message.From && x.To == message.To) || (x.To == message.From && x.From == message.To)).ToListAsync();
                try
                {
                    unseen = (await db.Messages.Where(x => x.To == message.To && x.From == message.From && x.IsSeen == false).ToListAsync()).Count;
                }
                catch (Exception ex)
                {
                    unseen = 0;
                }
                await Clients.Caller.SendAsync("RecieveLastMessageFromAll", Messages.Last().dateStapm, message.From, unseen);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Clients.Caller.SendAsync("NoMessage", message.From);
            }
        }

        public async Task GetMessages(Message message)
        {
            Console.WriteLine("GetMessages");
            try
            {
                List<Message> Messages = await db.Messages.Where(x => (x.To == message.From && x.From == message.To) || (x.From == message.From && x.To == message.To)).ToListAsync();

                for (int i = 0; i < Messages.Count; i++)
                {
                    if (Messages[i].To == message.From)
                    {
                        Messages[i].IsSeen = true;
                        db.Messages.Update(Messages[i]);
                    }
                }
                await Clients.Caller.SendAsync("ReceiveMessages", Messages);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Clients.Caller.SendAsync("NoMessage", message.From);
            }
        }

        public async Task SeenAllMessages(Message message)
        {
            Console.WriteLine("SeenAllMessages");
            try
            {
                List<Message> Messages = await db.Messages.Where(x => x.To == message.From && x.From == message.To).ToListAsync();
                for (int i = 0; i < Messages.Count; i++)
                {
                    Messages[i].IsSeen = true;
                    db.Messages.Update(Messages[i]);
                }
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Clients.Caller.SendAsync("NoMessage", message.From);
            }
        }

        public async Task Login(User user)
        {
            Console.WriteLine("Login");
            try
            {
                User usertemp = await db.Users.FirstAsync(x => x.Name == user.Name);
                if (usertemp != null)
                {
                    if (ComputeSha512Hash(user.Pass + usertemp.Salt) == usertemp.Pass)
                    {
                        string temp = usertemp.Salt;

                        usertemp.Pass = null;
                        usertemp.Salt = null;
                        await Clients.Caller.SendAsync("LoginSuccess", usertemp);

                        usertemp.Salt = temp;
                        usertemp.Pass = ComputeSha512Hash(user.Pass + usertemp.Salt);
                        usertemp.ConnectionID = Context.ConnectionId;
                        usertemp.Online = true;
                        db.Users.Update(usertemp);
                        db.SaveChanges();

                        List<User> Users = await db.Users.ToListAsync();
                        for (int i = 0; i < Users.Count; i++)
                        {
                            Users[i].Pass = null;
                            Users[i].ConnectionID = null;
                            Users[i].Salt = null;
                        }
                        await Clients.All.SendAsync("UpdateUser", Users);
                    }
                    else
                    {
                        Console.WriteLine("NoUser");
                        await Clients.Caller.SendAsync("LoginError", "NoUser");
                    }
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
            await Clients.AllExcept(Context.ConnectionId).SendAsync("UpdateUser", Users);

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
                _user.Salt = ComputeSalt();
                _user.Online = true;
                _user.Pass = ComputeSha512Hash(_user.Pass + _user.Salt);
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
                    Users[i].Salt = null;
                }
                await Clients.All.SendAsync("UpdateUser", Users);

            }
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"CONNECTED {Context.ConnectionId}");
            var name = Context.User.Claims.First().Value;
            User user = db.Users.Where(x => x.Name == name).First();
            string temp = user.Pass;

            user.Pass = null;
            await Clients.Caller.SendAsync("LoginSuccess", user);

            user.Pass = temp;
            user.ConnectionID = Context.ConnectionId;
            user.Online = true;
            db.Users.Update(user);
            db.SaveChanges();

            List<User> Users = await db.Users.ToListAsync();
            for (int i = 0; i < Users.Count; i++)
            {
                Users[i].Pass = null;
                Users[i].ConnectionID = null;
                Users[i].Salt = null;
                Users[i].Token = null;
            }
            await Clients.All.SendAsync("UpdateUser", Users);
            await base.OnConnectedAsync();
        }

        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        static string ComputeSha512Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA512 sha256Hash = SHA512.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        static string ComputeSalt()
        {
            // Определите минимальный и максимальный размеры соли.
            int minSaltSize = 4;
            int maxSaltSize = 8;

            // Создать случайное число для размера соли.
            Random random = new Random();
            int saltSize = random.Next(minSaltSize, maxSaltSize);

            // Выделите массив байтов, который будет содержать соль.
            byte[] saltBytes = new byte[saltSize];

            // Инициализируйте генератор случайных чисел.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            // Заполните соль криптографически сильными значениями байт.
            rng.GetNonZeroBytes(saltBytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < saltBytes.Length; i++)
            {
                builder.Append(saltBytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
