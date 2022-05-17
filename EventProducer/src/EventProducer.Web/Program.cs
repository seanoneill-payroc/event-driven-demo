using Common.Events;
using EventProducer.Web.Features.HealthCheck;
using EventProducer.Web.Services.Events;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, loggerConfiguration) =>
{
    loggerConfiguration
        .ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddHttpClient();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEventSystems(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck<SeqHealthCheck>("Seq connectivity");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseHealthChecks(
    "/health", 
    new HealthCheckOptions() { ResponseWriter = ResponseWriters.JsonReportWriter }
);

app.MapPost("/manualevent",
    async (
        IEventEmitter emitter,
        EventGenerator generator,
        string eventName,
        object payload) =>
    {
        var @event = await generator.NewEvent(eventName, payload);
        await emitter.Emit<object>(@event);
    }).WithName("EmitEvent");


app.MapGet("/randomevent", (RandomEventGenerator eventGenerator) => eventGenerator.RandomEvent());

app.Run();

