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
    /// The message history
    /// </summary>
    private readonly ConcurrentQueue<ChatMessage> messageQueue = new();

    /// <summary>
    /// All the chat clients
    /// </summary>
    private readonly ConcurrentDictionary<string, TaskCompletionSource<ChatMessage>> waitingClients = new();

    /// <summary>
    /// List containing all still usable colors
    /// </summary>
    List<string> usedColors = new List<string>();
    
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
            
            endpoints.MapGet("/usedNames", async context =>
            {
                context.Request.Query.TryGetValue("name", out var rawName);
                var name = rawName.ToString();
                
                if (this.usedNames.Contains(name))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
                else
                {
                    // Add new user
                    this.usedNames.Add(name);
                    context.Response.StatusCode = StatusCodes.Status201Created;
                }
            });
            
            // The endpoint to register a client to the server to subsequently receive the next message
            // This endpoint utilizes the Long-Running-Requests pattern.
            endpoints.MapGet("/messages", async context =>
            {
                var tcs = new TaskCompletionSource<ChatMessage>();
                context.Request.Query.TryGetValue("Color", out var rawColor);
                string color = rawColor.ToString();

                context.Request.Query.TryGetValue("id", out var rawId);

                var id = rawId.ToString();
                
                // Check if this user exists
                if (!this.usedNames.Contains(id))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("User invalid");
                }
                
                Console.WriteLine($"Client '{id}' registered");
                
                // register a client to receive the next message
                var error = true;
                lock (this.lockObject)
                {
                    if (this.waitingClients.ContainsKey(id))
                    {
                        if (this.waitingClients.TryRemove(id, out _))
                        {
                            Console.WriteLine($"Client '{id}' removed from waiting clients");
                        }
                    }

                    if (this.waitingClients.TryAdd(id, tcs))
                    {
                        Console.WriteLine($"Client '{id}' added to waiting clients");
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

                    Console.WriteLine("Zu lange nichts gesendet. Verschicke Warnung");

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
                
                Console.WriteLine($"Client '{id}' received message: {message.Content}");

                // send out the next message
                await context.Response.WriteAsJsonAsync(message);
            });

            // This endpoint is for sending messages into the chat
            endpoints.MapPost("/messages", async context =>
            {
                var message = await context.Request.ReadFromJsonAsync<ChatMessage>();

                if (message == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Message invalid.");
                }

                Console.WriteLine($"Received message from client: {message!.Content}");

                // Check if this user exists
                if (!this.usedNames.Contains(message.Sender))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("User invalid");
                }
                
                // maintain the chat history
                this.messageQueue.Enqueue(message);

                // broadcast the new message to all registered clients
                lock (this.lockObject)
                {
                    foreach (var (id, client) in this.waitingClients)
                    {
                        Console.WriteLine($"Broadcasting to client '{id}'");

                        // possbile memory leak as the 'dead' clients are never removed from the list
                        client.TrySetResult(message);
                    }
                }

                Console.WriteLine($"Broadcasted message to all clients: {message.Content}");

                // confirm that the new message was successfully processed
                context.Response.StatusCode = StatusCodes.Status201Created;
                await context.Response.WriteAsync("Message received and processed.");
            });
        });
    }
}