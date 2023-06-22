﻿using System.Net;
using System.Net.Http.Json;
using Data;

namespace Client;

/// <summary>
/// A client for the simple web server
/// </summary>
public class ChatClient
{
    /// <summary>
    /// The HTTP client to be used throughout
    /// </summary>
    private readonly HttpClient httpClient;

    /// <summary>
    /// The alias of the user
    /// </summary>
    private string alias;

    /// <summary>
    /// The color of the user
    /// </summary>
    private string color;

    /// <summary>
    /// The cancellation token source for the listening task
    /// </summary>
    readonly CancellationTokenSource cancellationTokenSource = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatClient"/> class.
    /// </summary>
    /// <param name="alias">The alias of the user.</param>
    /// <param name="color">The color of the user</param>
    /// <param name="serverUri">The server URI.</param>
    public ChatClient(string alias, string color, Uri serverUri)
    {
        this.alias = alias;
        //***neu***
        this.color = color;
        this.httpClient = new HttpClient();
        this.httpClient.BaseAddress = serverUri;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatClient"/> class.
    /// </summary>
    /// <param name="serverUri">The server URI.</param>
    public ChatClient(Uri serverUri)
    {
        this.alias = "-1";
        this.color = "-1";
        this.httpClient = new HttpClient();
        this.httpClient.BaseAddress = serverUri;
    }

    /// <summary>
    /// Lets the user choose a name and verifies it
    /// </summary>
    /// <returns>alias - Name of the user.</returns>
    public async Task<string> ChooseName()
    {
        var ts = new TextSnippets();
        const string defaultColor = "white";

        // Get cursor position
        var left = Console.GetCursorPosition().Left;
        var top = Console.GetCursorPosition().Top;

        this.alias = Console.ReadLine() ?? Guid.NewGuid().ToString();

        // Renders wrong-name screen 
        while (string.IsNullOrWhiteSpace(this.alias) || this.alias.Length > 38 || this.alias.Contains('\t'))
        {
            ts.EmptyLine(14);

            // Writes error-name box
            ts.WriteText(13, ts.NameField, "red", true);

            if (this.alias.Contains('\t'))
            {
                ts.DeleteText(8, ts.NameToLong, 1);
                ts.WriteText(8, ts.NameNoTab, defaultColor, true);
            }

            if (string.IsNullOrWhiteSpace(this.alias))
            {
                ts.DeleteText(8, ts.NameToLong, 1);
                ts.WriteText(8, ts.NameEmpty, defaultColor, true);
            }

            if (this.alias.Length > 38)
            {
                ts.DeleteText(8, ts.NameEmpty, 1);
                ts.WriteText(8, ts.NameToLong, defaultColor, true);
            }

            Console.SetCursorPosition(left, top);
            this.alias = Console.ReadLine() ?? Guid.NewGuid().ToString();
        }

        // Ask server if this name is available
        var listResponse = await this.httpClient.GetAsync($"/names?name={this.alias}");

        // Tell the user to pick another name
        while (!listResponse.IsSuccessStatusCode)
        {
            ts.WriteText(8, ts.NameError, defaultColor, false);
            ts.WriteText(13, ts.NameField, "red", true);

            Console.SetCursorPosition(Console.WindowWidth / 2 - 19, 14);
            this.alias = Console.ReadLine() ?? Guid.NewGuid().ToString();
            listResponse = await this.httpClient.GetAsync($"/names?name={this.alias}");
        }

        Console.Clear();

        return this.alias;
    }

    /// <summary>
    /// Lets the user choose a color
    /// </summary>
    /// <returns></returns>
    public async Task<string> ChooseColor()
    {
        var colorSettings = new ColorSettings();
        var responseSuccess = false;

        do
        {
            // get the usable colors from server
            var responseColors = await this.httpClient.GetStringAsync($"/colors");

            // make response string to string array and use it for the color selection
            var colorRange = responseColors.Split('|');
            this.color = colorSettings.ColorSelection(colorRange);

            // post the chosen color to the server
            var response = await this.httpClient.PostAsJsonAsync("/colors", this.color);
            responseSuccess = response.IsSuccessStatusCode;
        } while (!responseSuccess);

        return this.color;
    }

    /// <summary>
    /// Connects this client to the server.
    /// </summary>
    /// <returns>True if the connection could be established; otherwise False</returns>
    public async Task<bool> Connect()
    {
        // create and send a welcome message
        var message = new ChatMessage { Sender = this.alias, Content = $"Hi, I joined the chat!", Color = this.color };
        var response = await this.httpClient.PostAsJsonAsync("/messages", message);


        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Sends a new message into the chat.
    /// </summary>
    /// <param name="content">The message content as text.</param>
    /// <returns>True if the message could be send; otherwise False</returns>
    public async Task<bool> SendMessage(string content)
    {
        // creates the message and sends it to the server
        var message = new ChatMessage { Sender = this.alias, Content = content, Color = this.color };
        var response = await this.httpClient.PostAsJsonAsync("/messages", message);

        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Listens for messages until this process is cancelled by the user.
    /// </summary>
    public async Task ListenForMessages()
    {
        var cancellationToken = this.cancellationTokenSource.Token;

        // run until the user request the cancellation
        while (true)
        {
            try
            {
                string url = $"/messages?id={this.alias}&color={this.color}";

                // listening for messages. possibly waits for a long time.
                var message = await this.httpClient.GetFromJsonAsync<ChatMessage>(url, cancellationToken);

                // if a new message was received notify the user
                if (message != null)
                {
                    this.OnMessageReceived(message.Sender, message.Content, message.Color);
                }
            }
            catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // catch the cancellation 
                this.OnMessageReceived(this.alias, "[Hat den Chat verlassen!]", "White");
                
                // Leaving message
                var message = new ChatMessage
                {
                    Sender = this.alias,
                    Content = "[Hat den Chat verlassen!]",
                    Color = this.color
                };
                
                // send leaving-message to all clients
                var leaving = await this.httpClient.PostAsJsonAsync("/messages", message);
                
                // request leave
                var response = await this.httpClient.PostAsJsonAsync("/leave", message);
                
                break;
            }
        }
    }

    /// <summary>
    /// Cancels the loop for listening for messages.
    /// </summary>
    public void CancelListeningForMessages()
    {
        // signal the cancellation request
        this.cancellationTokenSource.Cancel();
    }

    // Enabled the user to receive new messages. The assigned delegated is called when a new message is received.
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    /// Called when a message was received and signal this to the user using the MessageReceived event.
    /// </summary>
    /// <param name="sender">The alias of the sender.</param>
    /// <param name="message">The containing message as text.</param>
    /// <param name="color">The color of the sender.</param>
    protected virtual void OnMessageReceived(string sender, string message, string color)
    {
        this.MessageReceived?.Invoke(this,
            new MessageReceivedEventArgs { Sender = sender, Message = message, Color = color });
    }
}