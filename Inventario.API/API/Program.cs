using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.DA.Seguridad;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Interfaces.Flujo.Seguridad;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Interfaces.Reglas.Seguridad;
using Abstracciones.Modelos.Seguridad;
using DA;
using DA.Repositorios;
using DA.Seguridad;
using Flujo;
using Flujo.Seguridad;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Reglas;
using Reglas.Seguridad;

var builder = WebApplication.CreateBuilder(args);

var tokenConfiguration = builder.Configuration.GetSection("Token").Get<TokenConfiguracion>();
var jwtIssuer = tokenConfiguration.Issuer;
var jwtAudience = tokenConfiguration.Audience;
var jwtKey = tokenConfiguration.key;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer=true,
            ValidateAudience=true,
            ValidateLifetime=true,
            ValidateIssuerSigningKey=true,
            ValidIssuer=jwtIssuer,
            ValidAudience=jwtAudience,
            IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    }
    );

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
builder.Services.AddScoped<IImportadorExcelHelper, ImportadorExcelHelper>();
// ==========================================
// Configuración
// ==========================================
builder.Services.AddScoped<IConfiguracion, Configuracion>();
// ==========================================
// Seguridad
// ==========================================
builder.Services.AddScoped<ISeguridadDA, SeguridadDA>();
builder.Services.AddScoped<ISeguridadFlujo, SeguridadFlujo>();
builder.Services.AddScoped<IUsuarioDA, UsuarioDA>();
builder.Services.AddScoped<IUsuarioFlujo, UsuarioFlujo>();
builder.Services.AddScoped<IAutenticacionFlujo, AutenticacionFlujo>();
builder.Services.AddScoped<IAutenticacionReglas, AutenticacionReglas>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<Autorizacion.Middleware.ClaimsPerfiles>();

app.MapControllers();

app.Run();
