using Abstracciones.Interfaces.Reglas;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Reglas;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/Cuenta/Login"; // Redirige aquí si intentan entrar sin sesión
        opciones.LogoutPath = "/Cuenta/Logout";
        opciones.AccessDeniedPath = "/Cuenta/AccesoDenegado"; // Redirige aquí si no tienen permiso
        opciones.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Tiempo de la sesión (ej. 60 minutos)
    });
builder.Services.AddScoped<IReportesHelper, ReportesHelper>();
builder.Services.AddHttpClient("InventarioAPI", client =>
{
    // Leemos la URL de la API desde tu appsettings.json
    client.BaseAddress = new Uri(builder.Configuration["ApiEndPoints:UrlBase"]);
});
builder.Services.AddScoped<IConfiguracion, Configuracion>();

// 4. Servicios del contenedor
builder.Services.AddSession(); // Agregar soporte para sesiones

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapRazorPages();

app.Run();
