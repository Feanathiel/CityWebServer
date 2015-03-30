namespace CityWebServer.Extensibility
{
    public interface IWebServer
    {
        IRequestHandler[] RequestHandlers { get; }
    }
}