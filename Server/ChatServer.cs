using System.Collections.Concurrent;
using Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Timers;
using Timer = System.Timers.Timer;


namespace Server;

/// <summary>
/// This is a very basic implementation of a chat server.
/// There are lot of things to improve...
/// </summary>
public class ChatServer
{
    /// <summary>
    /// Writes a log
    /// </summary>
    private LogWriter logWriter = new(); 
    
    /// <summary>
    /// The message history
    /// </summary>
    private readonly ConcurrentQueue<ChatMessage> messageQueue = new();

    /// <summary>
    /// All the chat clients
    /// </summary>
    private readonly ConcurrentDictionary<string, TaskCompletionSource<ChatMessage>> waitingClients = new();

    /// <summary>
    /// Default list of all available colors
    /// </summary>
    private static readonly List<string> DefaultColors = new List<string>(){
        "darkBlue", "darkGreen", "darkCyan",
        "darkRed", "darkMagenta", "darkYellow",
        "gray", "darkGray", "blue", "green", "cyan",
        "red", "magenta", "yellow"
        
    };
    
    /// <summary>
    /// List containing all still usable colors
    /// </summary>
    List<string> usedColors = new List<string>(DefaultColors);
    
    /// <summary>
    /// List containing all registered names.
    /// </summary>
    List<string> usedNames = new List<string>();

    /// <summary>
    /// The lock object for concurrency
    /// </summary>
    private readonly object lockObject = new();

    /// <summary>
    /// Configures the web services.
    /// </summary>
    /// <param name="app">The application.</param>
    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            
            // Endpoint that verifies if a name is already used or can be added
            endpoints.MapGet("/names", async context =>
            {
                context.Request.Query.TryGetValue("name", out var rawName);
                var name = rawName.ToString();
                
                this.logWriter.WriteLogLine($"!!GET /names was called by '{name}'!!");
                
                if (this.usedNames.Contains(name))
                {
                    context.Response.StatusCode = StatusCodes.Status406NotAcceptable;
                    
                    this.logWriter.WriteLogLine($"\t\t '{name}' was rejected!");
                }
                else
                {
                    // Add new user
                    this.usedNames.Add(name);
                    context.Response.StatusCode = StatusCodes.Status201Created;
                    
                    this.logWriter.WriteLogLine($"\t\t '{name}' was added!");
                }
            });
            
            // endpoint to receive all available colors
            endpoints.MapGet("/colors", async context =>
            {
                this.logWriter.WriteLogLine("!!GET /colors was called!!");

                var colors = string.Join("|", this.usedColors);
                await context.Response.WriteAsync(colors);
            });

            // endpoint to register a color
            endpoints.MapPost("/colors", async context =>
            {
                var color = await context.Request.ReadFromJsonAsync<string>();
                
                this.logWriter.WriteLogLine($"!!POST /colors was called with '{color}'!!");

                // Hier muss was unternommen werden!!!
                this.usedColors.Remove(color);
                context.Response.StatusCode = StatusCodes.Status202Accepted;
                await context.Response.WriteAsync($"'{color}' erfolgreich registriert!");
            });
            
            // endpoint to remove a user
            endpoints.MapPost("/leave", async context =>
            {
                this.logWriter.WriteLogLine("!!POST /leave was called!!");

                var message = await context.Request.ReadFromJsonAsync<ChatMessage>();

                if (message == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Message invalid.");

                    this.logWriter.WriteLogLine("\t\t Message invalid!");
                }
                
                // Check if this user exists and the color is part of default colors
                if (!this.usedNames.Contains(message.Sender) || !this.usedColors.Contains(message.Color) || !DefaultColors.Contains(message.Color))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("User invalid");

                    this.logWriter.WriteLogLine("\t\t User invalid!");
                }

                // Remove user from usedNames list
                this.usedNames.Remove(message.Sender);
                this.logWriter.WriteLogLine($"\t\t Sender '{message.Sender}' removed!");

                // Add color to the usable colors
                this.usedColors.Add(message.Color);
                this.logWriter.WriteLogLine($"\t\t Color '{message.Color}' added!");
                
                this.logWriter.WriteLogLine($"Client '{message.Sender}' with color '{message.Color}' removed");
            });
            
            // The endpoint to receive the next message
            // This endpoint utilizes the Long-Running-Requests pattern.
            endpoints.MapGet("/messages", async context =>
            {
                var tcs = new TaskCompletionSource<ChatMessage>();
                context.Request.Query.TryGetValue("Color", out var rawColor);
                string color = rawColor.ToString();

                context.Request.Query.TryGetValue("id", out var rawId);

                var id = rawId.ToString();
                
                this.logWriter.WriteLogLine($"!!GET /messages was called with '{id}' and '{color}'!!");
                
                // Check if this user exists
                if (!this.usedNames.Contains(id))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("User invalid");

                    this.logWriter.WriteLogLine($"\t\t User '{id}' invalid!");
                }
                
                this.logWriter.WriteLogLine($"\t\t Client '{id}' registered");
                
                // register a client to receive the next message
                var error = true;
                lock (this.lockObject)
                {
                    if (this.waitingClients.ContainsKey(id))
                    {
                        if (this.waitingClients.TryRemove(id, out _))
                        {
                            this.logWriter.WriteLogLine($"\t\t Client '{id}' removed from waiting clients");
                        }
                    }

                    if (this.waitingClients.TryAdd(id, tcs))
                    {
                        this.logWriter.WriteLogLine($"\t\t Client '{id}' added to waiting clients");
                        error = false;
                    }

                    // You could replace all of the above with just one line...
                    //this.waitingClients.AddOrUpdate(id.ToString(), tcs, (_, _) => tcs);
                    
                }

                // if anything went wrong send out an error message
                if (error)
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("Internal server error.");

                    this.logWriter.WriteLogLine($"\t\t Internal server error.");
                    
                }

                // otherwise wait for the next message broadcast

                
                
                Timer timer = new Timer();
                timer.Interval = 30000;
                timer.AutoReset = false;
                timer.Elapsed += TimeOut;
                
                timer.Start();
                bool timeOut = true;
                
                var message = await tcs.Task;

                void TimeOut(object sender, ElapsedEventArgs e)
                {

                    // Erzeugen Sie eine Nachricht für den Timeout

                    var timeoutMessage = new ChatMessage
                    {
                        Sender = "System",
                        Content = "AFK-Warnung!",
                        Color = "white"
                    };

                    this.logWriter.WriteLogLine("Zu lange nichts gesendet. Verschicke Warnung");

                    // Broadcast der Timeout-Nachricht an alle registrierten Clients
                    lock (lockObject)
                    {

                        foreach (var (_, client) in waitingClients)
                        {
                            // Setzen Sie das Ergebnis der TaskCompletionSource auf die Timeout-Nachricht
                            client.TrySetResult(timeoutMessage);
                        }
                    }


                }

                timer.Stop();
                timer.Close();
                
                this.logWriter.WriteLogLine($"\t\t Client '{id}' received message: {message.Content}");

                // send out the next message
                await context.Response.WriteAsJsonAsync(message);
            });

            // This endpoint is for sending messages into the chat
            endpoints.MapPost("/messages", async context =>
            {
                this.logWriter.WriteLogLine("!!POST /messages was called!!");

                var message = await context.Request.ReadFromJsonAsync<ChatMessage>();

                if (message == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Message invalid.");

                    this.logWriter.WriteLogLine($"\t\t Message invalid.");
                }

                this.logWriter.WriteLogLine($"\t\t Received message from '{message!.Sender}': {message!.Content}");

                // Check if this user exists
                if (!this.usedNames.Contains(message.Sender))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("User invalid");

                    this.logWriter.WriteLogLine($"\t\t User '{message.Sender}' invalid.");
                }
                
                // maintain the chat history
                this.messageQueue.Enqueue(message);

                // broadcast the new message to all registered clients
                lock (this.lockObject)
                {
                    foreach (var (id, client) in this.waitingClients)
                    {
                        this.logWriter.WriteLogLine($"\t\t Broadcasting to client '{id}'");

                        // possbile memory leak as the 'dead' clients are never removed from the list
                        client.TrySetResult(message);
                    }
                }

                this.logWriter.WriteLogLine($"\t\t Broadcasted message to all clients: {message.Content}");

                // confirm that the new message was successfully processed
                context.Response.StatusCode = StatusCodes.Status201Created;
                await context.Response.WriteAsync("Message received and processed.");
                this.logWriter.WriteLogLine($"\t\t Message received and processed.");
            });
        });
    }
}