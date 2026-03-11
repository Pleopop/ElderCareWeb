# Giai đoạn 1: Build source code
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy file Solution và các file csproj để restore thư viện trước (giúp build nhanh hơn)
COPY ["ElderCareConnect.sln", "./"]
COPY ["src/ElderCare.API/ElderCare.API.csproj", "src/ElderCare.API/"]
COPY ["src/ElderCare.Application/ElderCare.Application.csproj", "src/ElderCare.Application/"]
COPY ["src/ElderCare.Domain/ElderCare.Domain.csproj", "src/ElderCare.Domain/"]
COPY ["src/ElderCare.Infrastructure/ElderCare.Infrastructure.csproj", "src/ElderCare.Infrastructure/"]
COPY ["src/ElderCare.Shared/ElderCare.Shared.csproj", "src/ElderCare.Shared/"]
RUN dotnet restore "ElderCareConnect.sln"

# Copy toàn bộ code còn lại vào và Publish
COPY . .
WORKDIR "/src/src/ElderCare.API"
RUN dotnet publish "ElderCare.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Giai đoạn 2: Chạy ứng dụng (Chỉ lấy môi trường Runtime cho nhẹ)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render thường yêu cầu chạy ở port 8080 hoặc port động, .NET 8/9 mặc định dùng 8080
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ElderCare.API.dll"]