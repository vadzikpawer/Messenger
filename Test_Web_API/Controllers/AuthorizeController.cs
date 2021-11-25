using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Test_Web_API.Models;

namespace Test_Web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : Controller
    {
        AppDbContext db;
        User user = new User();
        Random rnd = new Random();
        private readonly IHubContext<ChatHub> _hubContext;

        public AuthorizeController(AppDbContext context, IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            db = context;
            Console.WriteLine("User controller up");
            if (!db.Users.Any())
            {
                string Salt1 = ComputeSalt();
                string Salt2 = ComputeSalt();
                db.Users.Add(new User { Name = "test1", Online = false, Pass = ComputeSha512Hash(ComputeSha512Hash("test1") + Salt1), Color = UsersController.Colors[rnd.Next(UsersController.Colors.Count())], Salt = Salt1 });
                db.Users.Add(new User { Name = "test2", Online = false, Pass = ComputeSha512Hash(ComputeSha512Hash("test2") + Salt2), Color = UsersController.Colors[rnd.Next(UsersController.Colors.Count())], Salt = Salt2 });
                db.SaveChanges();
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(User user_in)
        {
            Console.WriteLine(user_in.Name);
            Console.WriteLine(user_in.Pass);
            var identity = await GetIdentity(user_in);
            if (identity == null)
            {
                return NotFound("Invalid username or password.");
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            User response = new User
            {
                Name = identity.Name,
                Token = encodedJwt
            };
            return Ok(response);
        }

        private async Task<ClaimsIdentity> GetIdentity(User user_in)
        {
            Console.WriteLine(user_in.Name);
            Console.WriteLine(user_in.Pass);
            User user = await db.Users.FirstAsync(x => x.Name == user_in.Name);
            if (user != null)
            {
                Console.WriteLine(user.Pass);
                Console.WriteLine(ComputeSha512Hash(user_in.Pass + user.Salt));

                if (user.Pass == ComputeSha512Hash(user_in.Pass + user.Salt))
                {
                    var claims = new List<Claim>
                    {
                        new Claim("Name", user.Name),
                        new Claim("Color", user.Color)
                    };
                    ClaimsIdentity claimsIdentity =
                    new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, "Color");
                    return claimsIdentity;
                }
                else return null;
            }
            // если пользователь не найден
            return null;
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
#pragma warning disable SYSLIB0023 // Type or member is obsolete
            var rng = new RNGCryptoServiceProvider();
#pragma warning restore SYSLIB0023 // Type or member is obsolete

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
