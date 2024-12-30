using System.Text.Json.Serialization;
using IotControlService;
using IotControlService.Filters;
using IotControlService.Helpers;
using IotControlService.Repositories.Implementations;
using IotControlService.Repositories.Interfaces;
using IotControlService.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers(options => { options.Filters.Add<RequestResponseLoggingFilter>(); }).AddJsonOptions(
    opts =>
    {
        opts.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.SetUpSwagger();
builder.Services.SetUpIdentity(config);
builder.Services.SetUpJobs();

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

app.UseAuthentication();
app.UseAuthorization();

app.SetUpSwaggerUI();

app.MapGet("/", async context => context.Response.Redirect("/docs/index.html", false));

app.MapControllers();

app.Run();