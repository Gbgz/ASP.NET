using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;          // UseSqlite 方法
using Microsoft.Extensions.Configuration;      // Configuration.GetConnectionString 方法
using Microsoft.Extensions.DependencyInjection; // AddDbContext 方法



namespace AspNetCoreTodo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
        
            builder.Services.AddSingleton<ITodoItemService, FakeTodoItemService>();

            // 注册 Identity 服务（使用 ApplicationUser）
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //数据库
            // 根据数据库类型选择配置
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                // 使用 SQLite（轻量，适合开发）
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

                // 或使用 SQL Server（生产环境推荐）
                // options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));

                // 或使用 PostgreSQL
                // options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));

                // 启用敏感数据日志（仅开发环境）
                if (builder.Environment.IsDevelopment())
                {
                    options.LogTo(Console.WriteLine, LogLevel.Information)
                           .EnableSensitiveDataLogging();
                }
            });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
