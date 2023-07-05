using Takasbu.Models;
using Takasbu.Services;
using Takasbu.Middleware;
using Microsoft.AspNetCore.Antiforgery;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<UserStoreDatabaseSettings>(
    builder.Configuration.GetSection("UserStoreDatabaseSettings"));
 builder.Services.Configure<ProductStoreDatabaseSettings>(
    builder.Configuration.GetSection("ProductStoreDatabaseSettings"));   
builder.Services.AddScoped<TokenAuthenticationMiddleware>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<UsersService>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<TokenAuthenticationMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllers();

app.Run();
