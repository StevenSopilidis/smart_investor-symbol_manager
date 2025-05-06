using API;
using API.Migrations;
using API.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationExtensions(builder.Configuration);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

var app = builder.Build();

DBMigrator.MigrateDB(app);

// Configure the HTTP request pipeline.
app.MapGet("/", () => "gRPC API is running....");
app.MapGrpcService<ApiService>();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.Run();
