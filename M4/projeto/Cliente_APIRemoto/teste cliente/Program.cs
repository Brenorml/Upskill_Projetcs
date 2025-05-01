using Microsoft.AspNetCore.Authentication.Cookies;
using teste_cliente.Controllers;
using teste_cliente.Services;
using teste_cliente.Services.IServices;
using Rotativa.AspNetCore;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); //adicionado pro login
builder.Services.AddHttpClient<IAuthService, AuthService>(); //adicionado pro login
builder.Services.AddHttpClient(); // Adicionado para suportar HttpClient gen�rico
builder.Services.AddScoped<IAuthService, AuthService>(); //adicionado pro login
builder.Services.AddDistributedMemoryCache(); //adicionado pro login

// Adicionar o NoticiasController ao DI
builder.Services.AddScoped<NoticiasController>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();//adicionado pro login
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7211/");
});
builder.Services.AddScoped<IAuthService, AuthService>();//adicionado pro login
builder.Services.AddDistributedMemoryCache();//adicionado pro login

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        // For�a que os cookies sejam enviados mesmo em conex�es n�o seguras
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.SlidingExpiration = true;
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
}); //adicionado pro login

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// **Rotativa**
RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

app.Run();