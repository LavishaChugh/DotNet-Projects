namespace DotNet_Project.Services.WeatherForcast
{
    public interface IWeatherForcastService
    {
        IEnumerable<WeatherForecast> Get();

    }
}
