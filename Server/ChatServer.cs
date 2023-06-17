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
                var tcs = new TaskCompletionSource<ChatMessage>();

                context.Request.Query.TryGetValue("id", out var rawId);
                context.Request.Query.TryGetValue("key", out var rawKey);

                var id = rawId.ToString();
                var key = rawKey.ToString();
                var checkKey = this.users.CheckKey(key);
                var checkName = this.users.CheckName(id);
                var checkUser = this.users.CheckUser(key, id);
                
                // Check if user exists
                switch (checkUser)
                {
                    // Tell the client if the chosen name is already in use.
                    case false when !checkKey && checkName:
                        context.Response.StatusCode = StatusCodes.Status409Conflict;
                        await context.Response.WriteAsync($"User '{id}' is already in use!");
                        break;
                    
                    // Try to register a new user in the dictionary if it does not already exist.
                    case false when !checkKey && !checkName:
                        key = this.users.AddUser(id);
                        break;
                    
                    // Verify existing user
                    case true:
                        lock (this.lockObject)
                        {
                            this.logWriter.WriteLogLine($"'{id}' successfully verified!");
                        }
                        break;
                }

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

                lock (this.lockObject)
                {
                    // save the key in message
                    this.logWriter.WriteLogLine("VORHER: " + message.Key);
                    message.Key = key;
                    this.logWriter.WriteLogLine("NACHHER: " + message.Key);
                    this.logWriter.WriteLogLine($"Client '{id}' received message: {message.Content}");
                }



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
                
                var sender = message?.Sender;
                var key = message?.Key;
                
                // // Check if user is registered!
                // if (!this.users.CheckUser(key, sender))
                // {
                //     context.Response.StatusCode = StatusCodes.Status409Conflict;
                //     await context.Response.WriteAsync("User invalid!");
                // }
                lock (this.lockObject)
                {
                    this.logWriter.WriteLogLine($"Received message from client('{sender}'): '{message!.Content}' with key '{key}'");
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
            });
        });
    }
}