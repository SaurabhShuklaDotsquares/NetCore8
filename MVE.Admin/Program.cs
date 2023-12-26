using MVE.Core.Code.LIBS;
using MVE.Data.Models;
using MVE.Repo;
using MVE.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions.Common;
using Microsoft.Extensions.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MVE.Service.Banner;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
//builder.Services.AddControllersWithViews();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Services.AddRazorPages();


builder.Services.AddOptions();
builder.Services.AddHttpContextAccessor();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddDbContext<MultiVendorEcomDbContext>(options => options.UseSqlServer(
    //builder.Configuration.GetConnectionString("DBConnection")).UseLazyLoadingProxies(), ServiceLifetime.Transient);
    builder.Configuration.GetConnectionString("DBConnection")).UseLazyLoadingProxies());

//services.AddDbContext<DBContext>(options => options.UseSqlServer(configuration.GetConnectionString("DBContext")), ServiceLifetime.Transient);

SiteKeys.Configure(builder.Configuration.GetSection("AppSetting"));

builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt =>
//{
//    opt.LoginPath = "/account/index";
//});

builder.Services.AddAuthentication("AdminScheme").AddCookie("AdminScheme", opt =>
{
    opt.LoginPath = "/account/index";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(150);
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});
builder.Services.Configure<CookieTempDataProviderOptions>(options =>
{
    options.Cookie.Expiration = TimeSpan.FromMinutes(150);
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;

    options.Cookie.Name = "AdminAuthCookie";
    options.Cookie.Path = "/admin"; // Path for admin-specific cookies
});

//builder.Services.AddRouting(options =>
//{
//    options.LowercaseUrls = true; // URLs will be treated as case-insensitive
//    //options.LowercaseQueryStrings = true; // Query strings will also be treated as case-insensitive
//});

builder.Services.AddHttpClient();

builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = long.MaxValue;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue; // Set your desired max size here
    //options.Limits.MaxRequestLineSize = 8192; // Adjust as needed
    //options.Limits.MaxRequestHeadersTotalSize = 8192; // Adjust as needed
});


InitServices(builder.Services);



var app = builder.Build();


IHostEnvironment env = app.Environment;

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseExceptionHandler(
             options =>
             {
                 options.Run(
                 async context =>
                 {
                     context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                     context.Response.ContentType = "text/html";
                     var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                     if (ex != null)
                     {
                         var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.StackTrace}";
                         await context.Response.WriteAsync(err).ConfigureAwait(false);
                     }
                 });
             }
            );

app.UseHttpsRedirection();
app.UseStatusCodePages();
app.UseAuthentication();
app.UseSession();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();
app.UseRequestLocalization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");


app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = context =>
    {
        context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
        context.Context.Response.Headers.Add("Expires", "-1");
    }
});
app.UseCookiePolicy();

ContextProvider.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

app.Run();



void InitServices(IServiceCollection services)
{
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    services.AddScoped<IAdminUserService, AdminUserService>();
    services.AddScoped<IEmailFactoryService, EmailFactoryService>();
    services.AddScoped<IEmailSenderService, EmailSenderService>();
    services.AddScoped<IUserRoleService, UserRoleService>();
    services.AddScoped<IPermissionService, PermissionService>();
    services.AddScoped<IAccomodationTypeService, AccomodationTypeService>();
    services.AddScoped<IThemeService, ThemeService>();
    services.AddScoped<ICountryService, CountryService>();
    services.AddScoped<IRegionService, RegionService>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IStaticService, StaticService>();
    services.AddScoped<IRatingReviewService, RatingReviewService>();
    services.AddScoped<ISettingService, SettingService>();
    services.AddScoped<IFrontUserService, FrontUserService>();
    services.AddScoped<IVisaGuideService, VisaGuideService>();
    //services.AddScoped<INotificationService, NotificationService>();
    //services.AddScoped<IConfiguration, Configuration>();
    services.AddTransient<INotificationService, NotificationService>();
    services.AddTransient<IStaticContentBannerService, StaticContentBannerService>();
    services.AddScoped<ICategoryService, CategoryService>();

}

