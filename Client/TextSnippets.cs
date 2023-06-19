using System.Runtime.CompilerServices;

namespace Client
{
    /// <summary>
    /// A place for several Texts which will eventually displayed in the console.<br></br>
    /// Every Text is written in an Array in order to have a universal Methods:<br></br>
    /// <see cref="WriteText"/><br></br>
    /// <see cref="DeleteText"/>
    /// </summary>
    internal class TextSnippets
    {
        
        /// <summary>
        /// Text: A big Heading for the title Screen. <br></br>
        /// To display use <see cref="WriteText"/>
        /// </summary>
        public readonly string[] WelcomeText =
        {
            @" _    _ _ _ _ _                                          _             _____ _           _   ",
            @"| |  | (_) | | |                                        (_)           /  __ \ |         | |  ",
            @"| |  | |_| | | | _____  _ __ ___  _ __ ___   ___ _ __    _ _ __ ___   | /  \/ |__   __ _| |_ ",
            @"| |/\| | | | | |/ / _ \| '_ ` _ \| '_ ` _ \ / _ \ '_ \  | | '_ ` _ \  | |   | '_ \ / _` | __|",
            @"\  /\  / | | |   < (_) | | | | | | | | | | |  __/ | | | | | | | | | | | \__/\ | | | (_| | |_ ",
            @" \/  \/|_|_|_|_|\_\___/|_| |_| |_|_| |_| |_|\___|_| |_| |_|_| |_| |_|  \____/_| |_|\__,_|\__|"
            
        };
        
        /// <summary>
        ///  Text: Tell the user to login with his name. <br></br>
        /// To display use <see cref="WriteText"/>
        /// </summary>
        public readonly string[] LoginText =
        {
            @"Bitte Melde dich an."
        };
        
        /// <summary>
        /// Text: Tell the user to choose a color.<br></br>
        /// To display use <see cref="WriteText"/>
        /// </summary>
        public readonly string[] ColorText =
        {
            @"Bitte wähle eine Farbe aus."
        };
        
        /// <summary>
        /// Text: A final confirmation before the Chat starts.<br></br>
        /// To display use <see cref="WriteText"/>
        /// </summary>
        public readonly string[] StartText =
        {
            @"Drücke beliebige Taste, um fortzufahren..."
        };
        
        /// <summary>
        /// Displays a big Box. <br></br>
        /// To display use <see cref="WriteText"/>
        /// </summary>
        public readonly string[] NameField =
        {
            @"+─────────[Gib deinen Namen ein]─────────+",
            @"│                                        │",
            @"+────────────────────────────────────────+"
        };
        
        /// <summary>
        /// Displays a small Box.<br></br>
        /// To display use <see cref="WriteText"/>
        /// </summary>
        public readonly string[] ColorField =
        {
            @"+────+",
            @"│    │",
            @"+────+"
        };
        
        /// <summary>
        /// Error message for: Entered name already exists. <br></br>
        /// To display use <see cref="WriteText"/>
        /// </summary>
        public readonly string[] NameError =
        {
            @"Dieser Name ist bereits vergeben!"
        };
        
        /// <summary>
        /// Error message for: Entered name is empty.<br></br>
        /// To display use <see cref="WriteText"/>
        /// </summary>
        public readonly string[] NameEmpty =
        {
            @"Der Name muss mind. ein Buchstaben enthalten!"
        };
        
        /// <summary>
        /// Error message for: Entered name is longer than the set value in <see cref="ChatClient.ChooseName"/><br></br>
        /// To display use <see cref="WriteText"/>
        /// </summary>
        public readonly string[] NameToLong =
        {
            @"Was meinst du wofür die Box da ist? Dein Name darf sie nicht überschreiten!"
        };
        
        /// <summary>
        /// Error message for: Entered name consists of /t.<br></br>
        /// To display use <see cref="WriteText"/>
        /// </summary>
        public readonly string[] NameNoTab =
        {
            @"Nutze kein Tabulator in deinem Namen"
        };
        
        /// <summary>
        /// Error message for: There was no value entered which correlates with a color from an array given by the Server.<br></br>
        /// To display use <see cref="WriteText"/>
        /// </summary>        
        public readonly string[] ColorError =
        {
            @"Bitte nutze zur Auswahl nur die abgebildeten Zahlen."
        };
        
        /// <summary>
        /// Method for a Text Pattern. Creates a line across the whole console.
        /// </summary>
        public void CreateChatInterface()
        {
            int width = Console.WindowWidth;
            for (int i = 0; i < width; i++)
            {
                Console.Write("─");
            }
        }
        
        /// <summary>
        /// Method to clear one line, by replacing every possible letter with " ".
        /// </summary>
        /// <param name="line"> int - Value to define the line which will get emptied.</param>
        public void EmptyLine(int line)
        {
            Console.SetCursorPosition(0, line);
            int width = Console.WindowWidth;
            for (int i = 0; i < width; i++)
            {
                Console.Write(" ");
            }
        }
        
        /// <summary>
        /// A Method used to choose a snippet from <see cref="TextSnippets"/> to display a specific Text in the console.
        /// </summary>
        /// <param name="startHeight"> int - Value to set the line where delete Method should start. Should be the same as when this snippet was written. </param>
        /// <param name="nameOfText"> string - Value to choose a snippet from <see cref="TextSnippets"/>. </param>
        /// <param name="color"> string - Value to define in which color the text will be displayed </param>
        /// <param name="speed"> bool - Value to decide if the text should be displayed with a delay of 25ms between each line (false) or with no delay (true).</param>
        public void WriteText(int startHeight, string[] nameOfText, string color, bool speed)
        {
            Enum.TryParse(color, true, out ConsoleColor theColor);
            Console.ForegroundColor = theColor;
        
            int startWidth = (Console.WindowWidth-nameOfText[0].Length)/2;
            for (int i = 0; i < nameOfText.Length; i++)
            {
                Console.SetCursorPosition(startWidth,startHeight + i);
                Console.WriteLine(nameOfText[i]);
                if (!speed)
                {
                  Thread.Sleep(50);  
                }
                
            }
        
        }
        
        /// <summary>
        /// A Method used to choose a snippet from <see cref="TextSnippets"/> to Delete a specific Text from the screen.  
        /// </summary>
        /// <param name="startHeight"> int - Value to set the line where delete Method should start. Should be the same as when this snippet was written. </param>
        /// <param name="nameOfText"> string - Value to choose a snippet from <see cref="TextSnippets"/>. </param>
        /// <param name="lines"> int - Value to define how many line should be deleted. Should be snippet.Length</param>
        public void DeleteText(int startHeight, string[] nameOfText, int lines)
        {
            int startWidth = (Console.WindowWidth-nameOfText[0].Length)/2;

            for (int i = 0; i < lines; i++)
            {
                for (int j = 0; j < nameOfText[0].Length; j++)
                {
                    Console.SetCursorPosition(startWidth+j, startHeight+i);
                    Console.WriteLine(" ");
                }
            }
        
        }
        
    }
    
    
}