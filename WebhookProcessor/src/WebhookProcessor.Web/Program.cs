using Serilog;
using WebhookProducer.Web.Services.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddHttpClient();

// Add services to the container.
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddHostedService<WebhookProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.Run();
