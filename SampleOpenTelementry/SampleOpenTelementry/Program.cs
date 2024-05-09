using OpenTelemetry.Trace;
using SampleOpenTelementry;
using static SampleOpenTelementry.TelementryConstants;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.ConfigureServices();
builder.Services.AddTelementryRequirement(builder,builder.Configuration);
builder.Services.AddSingleton(TracerProvider.Default.GetTracer(AppSource));
await builder.Services.AddRedisConnectionProvider(builder.Configuration);

Console.WriteLine(builder.Environment.EnvironmentName);

var app = builder.Build();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapControllers();
app.Run();
