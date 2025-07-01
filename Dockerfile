# 使用 .NET 9.0 SDK 镜像
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# 设置工作目录并复制 csproj 文件
COPY AspNetCoreTodo/*.csproj ./AspNetCoreTodo/
WORKDIR /AspNetCoreTodo

# 恢复 NuGet 包
RUN dotnet restore

# 复制剩余的文件并发布应用
COPY AspNetCoreTodo/. ./
RUN dotnet publish -c Release -o /app/out /p:PublishWithAspNetCoreTargetManifest="false"

# 使用 .NET 9.0 运行时镜像
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# 设置环境变量
ENV ASPNETCORE_URLS=http://+:80

# 设置工作目录
WORKDIR /app

# 从 build 镜像复制已发布的应用
COPY --from=build /app/out ./

# 创建数据库目录（在复制文件之后）
RUN mkdir -p /app/data

# 设置数据库目录权限
RUN chmod 755 /app/data

# 启动应用
ENTRYPOINT ["dotnet", "AspNetCoreTodo.dll"]
