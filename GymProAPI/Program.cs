using System;
using BCrypt.Net;
using GymProAPI.Data;
using GymProAPI.Services;
using GymProAPI.Servicios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//  Servicios
builder.Services.AddControllers();
builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  Aquí registra el servicio de correo ANTES de Build()
builder.Services.AddSingleton<EmailService>();
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

//  Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//  Activar CORS antes de Authorization
app.UseCors("AllowAngular");
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();


