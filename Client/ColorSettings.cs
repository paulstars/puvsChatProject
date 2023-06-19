
namespace Client
{
    internal class ColorSettings
    {
        private readonly TextSnipplets ts = new  TextSnipplets();
        
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
        public string ColorSelection(string[] colorRange)
        {
                const string defaultColor = "white";
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
                            this.ts.WriteText(1, this.ts.ColorText, defaultColor, false);
                            this.ts.WriteText(3, this.ts.ColorField, defaultColor, true);
                        }
                        else
                        {
                            this.ts.WriteText(1, this.ts.ColorError, defaultColor, false);
                            this.ts.WriteText(3, this.ts.ColorField, "red", true);
                        }
                        
                    
                        // Displays all available color options
                        for (var i = 0; i < colorRange.Length; i++)
                        {
                            var counter = "0" + i;
                            Console.SetCursorPosition(Console.WindowWidth/2-7, 7+i);
                            if (counter.Length > 2)
                            {
                                counter = i.ToString();
                            }

                            SetColor(colorRange[i]);
                            Console.WriteLine(counter + " " + "■" + " " + colorRange[i]);
                            Thread.Sleep(25);
                        }
                    
                        // input of colorChoice
                        Console.SetCursorPosition(Console.WindowWidth / 2 - 1, 4);
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