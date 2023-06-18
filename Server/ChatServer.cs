using System.Collections.Concurrent;
using Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

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
    /// The lock object for concurrency
    /// </summary>
    private readonly object lockObject = new();

    private UserDict users = new UserDict();
    
    private LogWriter logWriter = new LogWriter();


    /// <summary>
    /// Configures the web services.
    /// </summary>
    /// <param name="app">The application.</param>
    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            // The endpoint to register a client to the server to subsequently receive the next message
            // This endpoint utilizes the Long-Running-Requests pattern.
            endpoints.MapGet("/messages", async context =>
            {
                this.logWriter.WriteLogLine("!!GET Messages called!!");

                var tcs = new TaskCompletionSource<ChatMessage>();

                context.Request.Query.TryGetValue("id", out var rawId);
                context.Request.Query.TryGetValue("key", out var rawKey);

                var id = rawId.ToString();
                var key = rawKey.ToString();
                

                // register a client to receive the next message
                var error = true;
                lock (this.lockObject)
                {
                    if (this.waitingClients.ContainsKey(id))
                    {
                        if (this.waitingClients.TryRemove(id, out _))
                        {
                            this.logWriter.WriteLogLine($"Client '{id}' removed from waiting clients");
                        }
                    }

                    if (this.waitingClients.TryAdd(id, tcs))
                    {
                        this.logWriter.WriteLogLine($"Client '{id}' added to waiting clients");
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
                var message = await tcs.Task;

                // send out the next message
                await context.Response.WriteAsJsonAsync(message);
                this.logWriter.WriteLogLine("!!GET Messages END!!");
            });

            // This endpoint is for sending messages into the chat
            endpoints.MapPost("/messages", async context =>
            {
                this.logWriter.WriteLogLine("!!POST Message called!!");

                var message = await context.Request.ReadFromJsonAsync<ChatMessage>();

                if (message == null)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("Message invalid.");
                }
                
                lock (this.lockObject)
                {
                    this.logWriter.WriteLogLine($"Received message from client('{message!.Sender}'): '{message.Content}' with key '{message.Key}'");
                }
                
                // maintain the chat history
                this.messageQueue.Enqueue(message);

                // broadcast the new message to all registered clients
                lock (this.lockObject)
                {
                    foreach (var (id, client) in this.waitingClients)
                    {
                        this.logWriter.WriteLogLine($"Broadcasting to client '{id}'");

                        // possbile memory leak as the 'dead' clients are never removed from the list
                        client.TrySetResult(message);
                    }
                    
                    this.logWriter.WriteLogLine($"Broadcasted message to all clients: {message.Content}");
                }
                
                // confirm that the new message was successfully processed
                context.Response.StatusCode = StatusCodes.Status201Created;
                await context.Response.WriteAsync("Message successfully processed!");
                this.logWriter.WriteLogLine("!!POST Message END!!");
            });
            
            
            endpoints.MapGet("/register", async context =>
            {

                this.logWriter.WriteLogLine("!!Register called!!");
                
                context.Request.Query.TryGetValue("id", out var rawId);
                context.Request.Query.TryGetValue("key", out var rawKey);

                    
                var name = rawId.ToString();
                var uKey = rawKey.ToString();

                bool checkKey;
                bool checkName;
                bool checkUser;
                
                lock (this.lockObject)
                {
                    checkKey = this.users.CheckKey(uKey);
                    checkName = this.users.CheckName(name);
                    checkUser = this.users.CheckUser(name, uKey);
                }

                switch (checkName)
                {
                    // Try to register a new user in the dictionary if it does not already exist.
                    case false when !checkKey:
                        lock (this.lockObject)
                        {
                            uKey = this.users.AddUser(name);
                            this.logWriter.WriteLogLine($"Client '{name}' registered with '{uKey}'!");
                        }
                        break;
                    
                    // Tell the client if the chosen name is already in use.
                    case true when !checkUser:
                        context.Response.StatusCode = StatusCodes.Status409Conflict;
                        await context.Response.WriteAsync($"User '{name}' is already in use!");
                        break;
                }
                        

                // // Check if user is registered!
                // if (!this.users.CheckUser(name, uKey))
                // {
                //     context.Response.StatusCode = StatusCodes.Status409Conflict;
                //     await context.Response.WriteAsync("User invalid!");
                // }

                
                var reg = new Registration{ Sender = name, Key = uKey};
                
                // confirm that the new message was successfully processed
                context.Response.StatusCode = StatusCodes.Status201Created;
                await context.Response.WriteAsJsonAsync(reg);
                this.logWriter.WriteLogLine("!!Register END!!");
            });
        });
    }
}