using Microsoft.AspNetCore.Authentication.Cookies;
using BBLiftClient.Application;
using BBLiftClient.MVC.Handlers; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options => 
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => 
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
        options.SlidingExpiration = false;
    });

builder.Services.AddSession(options => 
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var apiBaseUrl = builder.Configuration["API:BaseUrl"]
                 ?? throw new InvalidOperationException("API Base URL is not defined.");

builder.Services.AddTransient<AuthTokenHandler>();

builder.Services.AddHttpClient("BBLiftApi", options => 
    {
        options.BaseAddress = new Uri(apiBaseUrl);
        options.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .AddHttpMessageHandler<AuthTokenHandler>(); 

builder.Services.AddHttpContextAccessor();

builder.Services.AddApplication();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name:"areas",
    pattern:"{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

app.MapDefaultControllerRoute();

app.Run();