using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WebApplication3.Model;
using WebApplication3.ViewModels;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
})
.AddPasswordValidator<CustomPasswordValidator>() 
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
	options.LogoutPath = "/Logout";
	options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.Cookie.MaxAge = TimeSpan.FromMinutes(5);
    options.SlidingExpiration = true;
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache(); //save session in memory

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDataProtection();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseMiddleware<Prevent400LoginMiddleware>(); 
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseStatusCodePagesWithRedirects("/errors/{0}");

/*app.Use(async (context, next) =>
{
	// Check if the user is authenticated
	if (context.User.Identity.IsAuthenticated)
	{
		// Redirect to the desired page (e.g., Index)
		context.Response.Redirect("/Index");
		return;
	}

	await next();
});*/

app.MapRazorPages();

app.Run();

public class Prevent400LoginMiddleware
{
    private readonly RequestDelegate _next;

    public Prevent400LoginMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Check if the response status is 400 (Bad Request)
        if (context.Response.StatusCode == 400)
        {
            // Do not proceed with authentication
            return;
        }

        // Continue processing the request
        await _next(context);
    }
}
