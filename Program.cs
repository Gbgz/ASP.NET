using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;          // UseSqlite ����
using Microsoft.Extensions.Configuration;      // Configuration.GetConnectionString ����
using Microsoft.Extensions.DependencyInjection; // AddDbContext ����



namespace AspNetCoreTodo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //�������ڸ���Ϊ Scoped ��Singleton-->Scoped
            builder.Services.AddScoped<ITodoItemService, TodoItemService>();

            // 注册 Identity 服务，使用 ApplicationUser
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            //数据库
            // 配置数据库连接选项
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                // 使用 SQLite，适合开发环境
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

                // 如使用 SQL Server，生产环境推荐
                // options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));

                // 如使用 PostgreSQL
                // options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));

                // 开发环境启用详细日志记录
                if (builder.Environment.IsDevelopment())
                {
                    options.LogTo(Console.WriteLine, LogLevel.Information)
                           .EnableSensitiveDataLogging();
                }
            });



            var app = builder.Build();

            InitializeDatabase(app);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages();

            app.Run();
        }
        private static void InitializeDatabase(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    SeedData.InitializeAsync(services).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services
                        .GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Error occurred seeding the DB.");
                }
            }
        }
    }
}
