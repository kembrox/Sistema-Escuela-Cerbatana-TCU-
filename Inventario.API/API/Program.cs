using System.Text.Json.Serialization;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Interfaces.Reglas;
using DA;
using DA.Repositorios;
using Flujo;
using Microsoft.Extensions.Configuration;
using Reglas;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

// ==========================================
// REGISTRO DE REPOSITORIO BASE (DAPPER)
// ==========================================
builder.Services.AddScoped<IRepositorioDapper, RepositorioDapper>();

// ==========================================
// MÓDULO DE INVENTARIO (ACTIVOS)
// ==========================================
builder.Services.AddScoped<IInventarioDA, InventarioDA>();
builder.Services.AddScoped<IInventarioFlujo, InventarioFlujo>();

// ==========================================
// MÓDULO DE CATEGORÍAS
// ==========================================
builder.Services.AddScoped<ICategoriaDA, CategoriaDA>();
builder.Services.AddScoped<ICategoriaFlujo, CategoriaFlujo>();

// ==========================================
// MÓDULO DE UBICACIONES
// ==========================================
builder.Services.AddScoped<IUbicacionDA, UbicacionDA>();
builder.Services.AddScoped<IUbicacionFlujo, UbicacionFlujo>();

// ==========================================
// MÓDULO DE REPORTES
// ==========================================
builder.Services.AddScoped<IReportesHelper, ReportesHelper>();

// ==========================================
// Configuración
// ==========================================
builder.Services.AddScoped<IConfiguracion, Configuracion>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
