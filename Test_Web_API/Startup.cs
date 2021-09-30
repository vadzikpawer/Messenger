using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Test_Web_API.Models;

namespace Test_Web_API
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string ip_db = "168.63.110.193";
            string con = $"Server={ip_db},1433;Database=TestDB;User = SA; Password = <YourStrong@Passw0rd>;";
            
            // устанавливаем контекст данных
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(con));

            services.AddControllers(); // используем контроллеры без представлений

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, AppDbContext db)
        {

            db.Database.EnsureCreated();
            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); // подключаем маршрутизацию на контроллеры
            });
        }
    }
}
