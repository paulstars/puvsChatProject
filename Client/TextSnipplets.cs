using System.Runtime.CompilerServices;

namespace Client
{
    internal class TextSnipplets
    {
        
        
        public string[] WelcomeText =
        {
            @" _    _ _ _ _ _                                          _             _____ _           _   ",
            @"| |  | (_) | | |                                        (_)           /  __ \ |         | |  ",
            @"| |  | |_| | | | _____  _ __ ___  _ __ ___   ___ _ __    _ _ __ ___   | /  \/ |__   __ _| |_ ",
            @"| |/\| | | | | |/ / _ \| '_ ` _ \| '_ ` _ \ / _ \ '_ \  | | '_ ` _ \  | |   | '_ \ / _` | __|",
            @"\  /\  / | | |   < (_) | | | | | | | | | | |  __/ | | | | | | | | | | | \__/\ | | | (_| | |_ ",
            @" \/  \/|_|_|_|_|\_\___/|_| |_| |_|_| |_| |_|\___|_| |_| |_|_| |_| |_|  \____/_| |_|\__,_|\__|"
            
        };
        
        public string[] LoginText =
        {
            @"Bitte Melde dich an."
        };
        
        public string[] ColorText =
        {
            @"Bitte wähle eine Farbe aus."
        };
        
        public string[] StartText =
        {
            @"Drücke beliebige Taste, um fortzufahren..."
        };
        
        public string[] NameError =
        {
            @"Dieser Name ist bereits vergeben!"
        };
        
        public string[] NameEmpty =
        {
            @"Der Name muss mind. ein Buchstaben enthalten!"
        };
        
        public string[] NameToLong =
        {
            @"Was meinst du wofür die Box da ist? Dein Name darf sie nicht überschreiten!"
        };

        public string[] ColorError =
        {
            @"Bitte nutze zur Auswahl nur die abgebildeten Zahlen."
        };
        
        public string[] NameField =
        {
            @"+─────────[Gib deinen Namen ein]─────────+",
            @"│                                        │",
            @"+────────────────────────────────────────+"
        };
        
        public string[] ColorField =
        {
            @"+────+",
            @"│    │",
            @"+────+"
        };

        public void CreateChatInterface()
        {
            int width = Console.WindowWidth;
            for (int i = 0; i < width; i++)
            {
                Console.Write("─");
            }
        }
        
        public void EmptyLine(int line)
        {
            Console.SetCursorPosition(0, line);
            int width = Console.WindowWidth;
            for (int i = 0; i < width; i++)
            {
                Console.Write(" ");
            }
        }
        
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