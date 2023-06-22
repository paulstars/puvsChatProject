
namespace Client
{
    /// <summary>
    /// Collection of classes relating to color selection and changes.<br></br>
    /// <see cref="SetColor"/><br></br>
    /// <see cref="ColorSelection"/>
    /// </summary>
    internal class ColorSettings
    {
        private readonly TextSnippets ts = new  TextSnippets();
        
        /// <summary>
        /// Sets the color of every following Text with a color that correlates with a color from an default array given by the Server
        /// </summary>
        /// <param name="selection"> string - Value to select the color of the following Text displays</param>
        public void SetColor(string selection)
        {
            if (Enum.TryParse(selection, true, out ConsoleColor color))
            {
                Console.ForegroundColor = color;
            }
            else
            {
                Console.WriteLine("Error: Es scheint sich um keine Farbe zu handeln!");
            }

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorRange"> string[] - available colors received by the server </param>
        /// <returns> colorChoice - The color chosen by the user.</returns>
        public string ColorSelection(string[] colorRange)
        {
            const string defaultColor = "white";
                var colorDictionary = new Dictionary<string, string>();
                var ind = 0;
                
                // Builds the dictionary for the colors. Using an Index to create following structure: ("00","darkBlue")
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
                    // Change the text in the first iteration.
                        if (first)
                        {
                            this.ts.WriteText(1, this.ts.ColorText, defaultColor, false);
                            this.ts.WriteText(3, this.ts.ColorField, defaultColor, true);
                        }
                        else
                        {
                            this.ts.WriteText(1, this.ts.ColorError, defaultColor, true);
                            this.ts.WriteText(3, this.ts.ColorField, "red", true);
                        }
                        
                        // Displays all available color options.
                        for (var i = 0; i < colorRange.Length; i++)
                        {
                            var counter = "0" + i;
                            Console.SetCursorPosition(Console.WindowWidth/2-7, 7+i);
                            if (counter.Length > 2)
                            {
                                counter = i.ToString();
                            }
    
                            this.SetColor(colorRange[i]);
                            Console.WriteLine(counter + " " + "■" + " " + colorRange[i]);
                            
                            // Renders first display slower
                            if (first)
                            {
                               Thread.Sleep(25); 
                            }
                            
                        }

                        // Set the input of colorChoice
                        Console.SetCursorPosition(Console.WindowWidth / 2 - 1, 4);
                        this.SetColor(defaultColor);
                        answer = Console.ReadLine();
                        Console.Clear();


                        first = false;
                        
                } while (answer != null && !colorDictionary.ContainsKey(answer));

                // get the color from the dictionary
                colorDictionary.TryGetValue(answer, out var colorChoice);
            
                return colorChoice!;
        }
    }
}