using Microsoft.EntityFrameworkCore;
using TenteraAPI.Application.Services;
using TenteraAPI.Domain.Interfaces.Repositories;
using TenteraAPI.Domain.Interfaces.Services;
using TenteraAPI.Infrastructure.Database;
using TenteraAPI.Infrastructure.Repositories;
using TenteraAPI.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AccountDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddScoped<IVerificationCodeStore, InMemoryVerificationCodeStore>();
builder.Services.AddScoped<AccountService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
