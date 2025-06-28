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
        
            builder.Services.AddSingleton<ITodoItemService, FakeTodoItemService>();

            // ע�� Identity ����ʹ�� ApplicationUser��
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //���ݿ�
            // �������ݿ�����ѡ������
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                // ʹ�� SQLite���������ʺϿ�����
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

                // ��ʹ�� SQL Server�����������Ƽ���
                // options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection"));

                // ��ʹ�� PostgreSQL
                // options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));

                // ��������������־��������������
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
