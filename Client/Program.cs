

namespace Client;

/// <summary>
/// A most basic chat client for the console
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var ts = new TextSnippets();
        var cs = new ColorSettings();
        
        const string defaultColor = "white";
        
        var serverUri = new Uri("http://localhost:5000");
        
        // create a new client
        var client = new ChatClient(serverUri);
        
        // Press any key to start the application
        Console.WriteLine("Zum starten <Enter> drücken");
        Console.ReadKey();
        Console.Clear();
        
        // Creates a main menu. User ist requested to enter name. Displaying: Title, Guiding Text, Input box
        ts.WriteText(1,ts.WelcomeText,defaultColor,false);
        ts.WriteText(8, ts.LoginText, defaultColor, false);
        ts.WriteText(13, ts.NameField, defaultColor, false);
        
        // User can insert name in created input box. Checking if user name is valid
        Console.SetCursorPosition(Console.WindowWidth/2-19,14);
        var currentSender = await client.ChooseName();
        
        // writing name in an array for later display
        string[] sender = { $"< {currentSender} >" };
        
        // Deletes Guiding Text and input box
        ts.DeleteText(8,ts.LoginText, 1);
        ts.DeleteText(13, ts.NameField,3 );
        
        // Starts the process of choosing a color. Creates: Input Box, Guiding Text, Available Colors
        var color = await client.ChooseColor();
        
        // Displays a new main menu using the name and the chosen color by the user.
        ts.WriteText(1, ts.WelcomeText, color, false);
        ts.WriteText(8, sender, color, false);
        ts.WriteText(10, ts.StartText, defaultColor, false);
        
        // Final key input before logging in
        Console.ReadKey();
        
        // Creating the chat Interface
        ts.DeleteText(10, ts.StartText, 1);
        Console.SetCursorPosition(0, 11);
        Console.WriteLine($"Chat vom: {DateTime.Now:dd.MM.yyyy}");
        Console.SetCursorPosition(0, 12);
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
            var content = Console.ReadLine() ?? string.Empty;
            while (string.IsNullOrWhiteSpace(content))
            {
                var left = Console.GetCursorPosition().Left;
                var top = Console.GetCursorPosition().Top;
                Console.SetCursorPosition(left, top-1);
                content = Console.ReadLine() ?? string.Empty; 
            }
            //Moves the cursor up by one, in order to remove the entered Text   
            var cursor = Console.GetCursorPosition();
            Console.SetCursorPosition(0,cursor.Top - 1);

            // cancel the listening task and exit the loop
            if (content.ToLower() == "exit")
            {
                client.CancelListeningForMessages();
                break;
            }

            // send the message and display the result
            if (!await client.SendMessage(content))
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
        var time = DateTime.Now.ToString("HH:mm");
        Console.Write($"[{time}] ");

        cs.SetColor(e.Color);
        Console.Write(e.Sender);
        cs.SetColor("White");
        Console.WriteLine($": {e.Message}");
    }
}