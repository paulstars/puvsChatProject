

namespace Client;

/// <summary>
/// A most basic chat client for the console
/// </summary>
public class Program{

    public static async Task Main(string[] args)
    {
        TextSnipplets ts = new TextSnipplets();
        ColorSettings cs = new ColorSettings();
        const string defaultColor = "white";

        var serverUri = new Uri("http://localhost:5000");
        
        // create a new client
        var client = new ChatClient(serverUri);
        
        // query the user for a name
        Console.ReadKey();
        ts.WriteText(1,ts.WelcomeBackText,defaultColor);
        ts.WriteText(8, ts.LoginText, defaultColor);
        ts.WriteText(13, ts.NameField, defaultColor);
        
        Console.SetCursorPosition(Console.WindowWidth/2-19,14);
        var currentSender = await client.ChooseName();
        string[] sender = { $"< {currentSender} >" };
        ts.DeleteText(8,ts.LoginText, 1);
        ts.DeleteText(13, ts.NameField,3 );
        
        var color = await client.ChooseColor();
        
        ts.WriteText(1, ts.WelcomeBackText, color);
        ts.WriteText(8, sender, color);
        ts.WriteText(10, ts.StartText, defaultColor);
        Console.ReadKey();
        ts.DeleteText(10, ts.StartText, 1);
        Console.SetCursorPosition(0,11);
        Console.WriteLine($"Chat vom: {DateTime.Now.ToString().Remove(11,8)}");
        Console.SetCursorPosition(0,12);
        ts.CreateChatInterface();
        
        // connect the event handler for the received messages
        client = new ChatClient(currentSender, color, serverUri);
        client.MessageReceived += MessageReceivedHandler;
        
        
        // connect to the server and start listening for messages
        var connectTask = await client.Connect();
       
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
        
        cs.SetColor(e.Color);
        Console.Write(e.Sender);
        cs.SetColor("White");
        Console.WriteLine($": {e.Message}");
    }
}