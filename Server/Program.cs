using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;

namespace Server;

/// <summary>
/// The runner for the web services
/// </summary>
public class Program
{
    
    public static void Main(string[] args)
    {
        // Initialises the LogWriter and clears the log-file
        var logWriter = new LogWriter();
        logWriter.ClearLogFile();
        logWriter.WriteLogLine("\t !!START Server!!");
        
        // Create the web application
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        var server = new ChatServer();
        server.Configure(app);

        // Run the web application
        app.Run();
    }
}