using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using AspNetCoreTodo.Data;

namespace AspNetCoreTodo.IntegrationTests
{
    public class TestFixture : IDisposable
    {
        public HttpClient Client { get; }
        private readonly WebApplicationFactory<Program> _factory;

        public TestFixture()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    // 配置测试环境
                    builder.UseEnvironment("Test");
                    
                    // 替换数据库为内存数据库（用于测试）
                    builder.ConfigureServices(services =>
                    {
                        // 移除原有的数据库上下文
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        // 添加内存数据库
                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("TestDatabase");
                        });
                    });
                });

            Client = _factory.CreateClient();
        }

        public void Dispose()
        {
            Client.Dispose();
            _factory.Dispose();
        }

        // 获取服务作用域（用于测试中的服务访问）
        public IServiceScope CreateScope()
        {
            return _factory.Services.CreateScope();
        }

        // 获取数据库上下文（用于测试中的数据库操作）
        public ApplicationDbContext GetDbContext()
        {
            var scope = CreateScope();
            return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }
    }
}