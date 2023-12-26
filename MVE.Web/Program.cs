using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TCP.Core.Code.LIBS;
using TCP.Repo;
using TCP.Service;
using TCP.Data.Models;
using TCP.Service.ActivityIncExc;
using TCP.Service.Banner;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Services.AddRazorPages();

builder.Services.AddOptions();
builder.Services.AddHttpContextAccessor();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddDbContext<TravelCustomPackagesContext>(options => options.UseSqlServer(
    //builder.Configuration.GetConnectionString("DBConnection")).UseLazyLoadingProxies()
    builder.Configuration.GetConnectionString("DBConnection")).UseLazyLoadingProxies().ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.DetachedLazyLoadingWarning))
);

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
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opt =>
{
    opt.LoginPath = "/home/index";
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(150);
    options.Cookie.IsEssential = true;
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

    options.Cookie.Name = "FrontEndAuthCookie";
    options.Cookie.Path = "/"; // Root path for front-end cookies
});

builder.Services.AddHttpClient();

builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.Configure<IISServerOptions>(options =>
{
    //options.MaxRequestBodySize = int.MaxValue;
    options.MaxRequestBodySize = long.MaxValue;
});

InitServices(builder.Services);

var app = builder.Build();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute("static-page", "{page_url}", new { controller = "Common", action = "StaticPages" },
              new { page_url = @"^privacy-policy|terms-condition|about-us" },
              new[] { "TCP.Web.Controllers" }
              );

//app.MapControllerRoute("visa-guide", "visa-guide/{visa_url}", new { controller = "VisaGuide", action = "Index" },
//              new { page_url = @"^{page_url}" },
//              new[] { "TCP.Web.Controllers" }
//              );
app.MapControllerRoute(
                name: "visa-guide",
                pattern: "visa-guide/{visa_url}",
                defaults: new { controller = "VisaGuide", action = "Index" });

app.MapControllerRoute(
                name: "package",
                pattern: "package/{package_url}",
                defaults: new { controller = "ListingDetail", action = "Index" });


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
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IEmailFactoryService, EmailFactoryService>();
    services.AddScoped<IEmailSenderService, EmailSenderService>();
    services.AddScoped<ICustomQuoteService, CustomQuoteService>();
    services.AddScoped<ICustomPkgService, CustomPkgService>();
    services.AddScoped<IThemeService, ThemeService>();
    services.AddScoped<IPlanHolidayService, PlanHolidayService>();
    services.AddScoped<IPackageService, PackageService>();
    services.AddScoped<ICountryService, CountryService>();
    services.AddScoped<ICheckoutService, CheckoutService>();
    services.AddScoped<IDestinationService, DestinationService>();
    services.AddScoped<IStaticService, StaticService>();
    services.AddScoped<IActivityIncExcService, ActivityIncExcService>();
    services.AddScoped<IContactUsService, ContactUsService>();
    services.AddScoped<ISettingService, SettingService>();
    services.AddScoped<IQuoteFeedbackService, QuoteFeedbackService>();
    services.AddScoped<IRegionService, RegionService>();
    services.AddScoped<IManageDestinationService, ManageDestinationService>();
    services.AddScoped<IRatingReviewService, RatingReviewService>();
    services.AddScoped<IVisaGuideService, VisaGuideService>();
    services.AddTransient<INotificationService, NotificationService>();
    services.AddTransient<IStaticContentBannerService, StaticContentBannerService>();
}