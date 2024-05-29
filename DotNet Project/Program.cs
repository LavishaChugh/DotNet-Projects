using DotNet_Project.Services.WeatherForcast;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IWeatherForcastService, WeatherForcastService>();
builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration()
    //.MinimumLevel.Information()
    //.WriteTo.Console()       // writes to console can also be written to file.
    //.WriteTo.File("logger-.txt", rollingInterval: RollingInterval.Day)
    //.CreateLogger();
    .ReadFrom.Configuration(builder.Configuration).CreateLogger();  


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
