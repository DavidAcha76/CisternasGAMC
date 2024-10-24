using Microsoft.EntityFrameworkCore;
using CisternasGAMC.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor.
builder.Services.AddRazorPages(); // Habilitar Razor Pages en el proyecto.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") // Usar la cadena de conexión definida en la configuración.
    )
);
builder.Services.AddIdentity<IdentityUser, IdentityRole>() // Agregar Identity
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.Name = "UserLoginCookie";
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/AccessDenied";
});

var app = builder.Build();

// Configurar el pipeline de manejo de solicitudes HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error"); // Usar la página de manejo de errores en producción.
    app.UseHsts(); // Habilitar HSTS
}

app.UseHttpsRedirection(); // Redirigir automáticamente a HTTPS.
app.UseStaticFiles();      // Habilitar el uso de archivos estáticos.

app.UseRouting();          // Habilitar el enrutamiento.

app.UseAuthentication();   // Habilitar la autenticación.
app.UseAuthorization();    // Habilitar la autorización.

app.MapRazorPages();       // Mapear las Razor Pages para que respondan a las solicitudes.

app.Run();                 // Ejecutar la aplicación.
