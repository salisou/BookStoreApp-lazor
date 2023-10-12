global using System;
global using Serilog;
global using AutoMapper;
global using System.Linq;
global using bookstoreApp.Api.Data;
global using System.Threading.Tasks;
global using bookstoreApp.Api.Static;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Http;
global using System.Collections.Generic;
global using Microsoft.EntityFrameworkCore;
global using bookstoreApp.Api.Models.Author;
global using bookstoreApp.Api.Configurations;
global using System.ComponentModel.DataAnnotations;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var conString = builder.Configuration.GetConnectionString("BookStoreAppDbConnection");
builder.Services.AddDbContext<BookStoreDbContext>(options => options.UseSqlServer(conString));

builder.Services.AddAutoMapper(typeof(MapperConfig));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((contextConfigutation, loggingConfiguration) => 
    loggingConfiguration.WriteTo.Console()
    .ReadFrom.Configuration(contextConfigutation.Configuration)
);

builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowAll", 
        b =>  b.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
