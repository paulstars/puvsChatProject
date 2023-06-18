
namespace Client
{
    internal class ColorSettings
    {
        TextSnipplets ts = new();
        
        public void SetColor(string selection)
        {
            if (Enum.TryParse(selection, true, out ConsoleColor color))
            {
                Console.ForegroundColor = color;
            }
            else
            {
                Console.WriteLine("Error: Es scheint sich um keine Farbe zuhandeln");
            }

        }
        public string ColorSelection()
        {
                // color source
                string[] colorRange =
                    {
                        "darkBlue", "darkGreen", "darkCyan",
                        "darkRed", "darkMagenta", "darkYellow",
                        "gray", "darkGray", "blue", "green", "cyan",
                        "red", "magenta", "yellow", "white"
                    };

                var colorDictionary = new Dictionary<string, string>();
                var ind = 0;
                
                // builds the dictionary for the colors
                foreach (var color in colorRange)
                {
                    var counter = "0" + ind;
                    if (counter.Length > 2)
                    {
                        counter = ind.ToString();
                    }
                    
                    colorDictionary.Add(counter, color);

                    ind++;
                }

                string? answer;
                var first = true;
            
                // continues till a valid color is given
                do
                {
                    do
                    {
                        // Change the text in the first iteration
                        if (first)
                        {
                            Console.WriteLine("Was ist deine Lieblingsfarbe?");
                        }
                        else
                        {
                            Console.WriteLine("Diese Eingabe ist nicht korrekt!");
                            Console.WriteLine("Bitte Versuchen Sie es erneut!");
                        }
                        this.ts.DotLine();
                    
                        // Displays all available color options
                        for (var i = 0; i < colorRange.Length; i++)
                        {
                            var counter = "0" + i;
                            if (counter.Length > 2)
                            {
                                counter = i.ToString();
                            }

                            this.SetColor(colorRange[i]);
                            Console.WriteLine(counter + " " + "■" + " " + colorRange[i]);

                        }
                    
                        // input of colorChoice
                        answer = Console.ReadLine();
                        Console.Clear();


                        first = false;
                    } while (answer == null);
                } while (!colorDictionary.ContainsKey(answer));

                // get the color from the dictionary
                colorDictionary.TryGetValue(answer, out var colorChoice);
            
                return colorChoice!;
        }
    }
}