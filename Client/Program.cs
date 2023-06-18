using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;
using System;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Client;

/// <summary>
/// A most basic chat client for the console
/// </summary>
public class Program{

    public static async Task Main(string[] args)
    {
        TextSnipplets ts = new TextSnipplets();
        ColorSettings cs = new ColorSettings();

        var serverUri = new Uri("http://localhost:5000");
        
        // create a new client
        var client = new ChatClient(serverUri);
        
        // query the user for a name
        ts.Welcome();
        var currentSender = await client.ChooseName();
        ts.DotLine();
        var color = cs.ColorSelection();
        
        // connect the event handler for the received messages
        client.MessageReceived += MessageReceivedHandler;
        
        
        // connect to the server and start listening for messages
        var connectTask = await client.Connect();
        
        ts.HaveFun(color, currentSender);
        Console.WriteLine();

        
        var listenTask = client.ListenForMessages();

        // query the user for messages to send or the exit command
        while (true)
        {
            //Console.Write("Geben Sie Ihre Nachricht ein (oder 'exit' zum Beenden): ");
            var content = Console.ReadLine() ?? string.Empty;
            
            //Moves the curser up by one, in order to remove the entered Text   
            var cursor = Console.GetCursorPosition();
            Console.SetCursorPosition(0,cursor.Top - 1);

            // cancel the listening task and exit the loop
            if (content.ToLower() == "exit")
            {
                client.CancelListeningForMessages();
                break;
            }

            //Console.WriteLine($"Sending message: {content}");

            // send the message and display the result
            if (await client.SendMessage(content))
            {
                //Console.WriteLine("Message sent successfully.");
            }
            else
            {
                Console.WriteLine("Failed to send message.");
            }
        }

        // wait for the listening for new messages to end
        await Task.WhenAll(listenTask);

        Console.WriteLine("\nGood bye...");
    }

    /// <summary>
    /// Helper method to display the newly received messages.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="MessageReceivedEventArgs"/> instance containing the event data.</param>
    static void MessageReceivedHandler(object? sender, MessageReceivedEventArgs e)
    {
        var cs = new ColorSettings();
        var time = DateTime.Now.ToString().Remove(0,11).Remove(5,3);
        Console.Write($"[{time}] ");
        
        cs.setColor(e.Color);
        Console.Write(e.Sender);
        cs.setColor("White");
        Console.WriteLine($": {e.Message}");
    }
}