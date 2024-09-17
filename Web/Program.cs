using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Dal;
using Shcheduler.Web.Areas.Identity.Pages.Account;
using Shcheduler.Core.DAL;
using Shcheduler.Core.DAL.Interfaces;
using Shcheduler.Core.DAL.Repositories;
using Shcheduler.Core.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using Shcheduler.Core.Validations;
using System.Reflection;
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddDbContextFactory<UserDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("UserDbConnection")));
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ShchedulerContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Shcheduler.Core")));

builder.Services.AddDefaultIdentity<UserAddititonForIdentity>(options => options.SignIn.RequireConfirmedAccount = true).
    AddEntityFrameworkStores<UserDbContext>();
builder.Services.AddTransient<ITaskRepository, TaskRepository>();
builder.Services.AddTransient<ITaskResponseRepository, TaskResponseRepository>();
builder.Services.AddTransient<IRawResponseRepository, RawResponseRepository>();
builder.Services.AddTransient<IConverter, Converter>();
builder.Services.AddTransient<IAgentRepository, AgentRepository>();
builder.Services.AddSignalR();
builder.Services.AddTransient<SignalRHub>();
builder.Services.Configure<HubOptions>(options =>
{
    options.MaximumReceiveMessageSize = null;
});
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 0;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "WebSchedulingSystem API",
        Description = "API for create and edit cron tasks in WebSchedulingSystem",
        TermsOfService = new Uri("https://github.com/RUSALITAcademy/WebSchedulingSystem/tree/main"),
        Contact = new OpenApiContact
        {
            Name = "Kaunov Artem",
            Email = "yzenfer@mail.ru",
            Url = new Uri("https://vk.com/ness90")
        },
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=AboutUs}/{id?}");
    endpoints.MapRazorPages();
});
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebSchedulingSystem API v1");
    c.RoutePrefix = "swagger"; 
    c.InjectStylesheet("/swagger/custom.css");
    c.InjectJavascript("/swagger/custom.js", "text/javascript");
});
app.MapHub<SignalRHub>("/SignalRHub");
app.UseCookiePolicy();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
await app.RunAsync();
