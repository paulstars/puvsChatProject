using Microsoft.AspNetCore.Builder;

namespace Server;

/// <summary>
/// The runner for the web services
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        // Initialise logWriter to clear the log-file
        LogWriter logWriter = new();
        logWriter.ClearLogFile();
        logWriter.WriteLogLine("!SERVER START!\n");

        // Create the web application
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        var server = new ChatServer();
        server.Configure(app);

        // Run the web application
        app.Run();
    }
}