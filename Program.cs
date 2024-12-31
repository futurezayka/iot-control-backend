using System.Text.Json.Serialization;
using IotControlService;
using IotControlService.Filters;
using IotControlService.Helpers;
using IotControlService.Repositories.Implementations;
using IotControlService.Repositories.Interfaces;
using IotControlService.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers(options => { options.Filters.Add<RequestResponseLoggingFilter>(); }).AddJsonOptions(
    opts =>
    {
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.Name = "UserLoginCookie";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = new TimeSpan(24, 0, 0);
        options.Events.OnRedirectToLogin = (context) =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        
        options.Cookie.HttpOnly = false;
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
    });

builder.Services.SetUpSwagger();
builder.Services.SetUpIdentity(config);
builder.Services.SetUpJobs();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<JobHelper>();
builder.Services.AddScoped<RabbitMqService>(provider => RabbitMqService.CreateAsync(
    provider.GetRequiredService<IUnitOfWork>(),
    config["RabbitMq:HostName"]!,
    config["RabbitMq:UserName"]!,
    config["RabbitMq:Password"]!
).GetAwaiter().GetResult());

var app = builder.Build();

app.Services.DatabaseMigrate();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();

app.SetUpSwaggerUI();

app.MapGet("/", async context => context.Response.Redirect("/docs/index.html", false));

app.MapControllers();

app.Run();