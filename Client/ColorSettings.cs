using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class ColorSettings
    {
        TextSnipplets ts = new TextSnipplets();

        public void setColor(string selection)
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
            string[] colorRange =
                {
                    "darkBlue", "darkGreen", "darkCyan",
                    "darkRed", "darkMagenta", "darkYellow",
                    "gray", "darkGray", "blue", "green", "cyan",
                    "red", "magenta", "yellow", "white"
                };

            Dictionary<string, string> colorDictionary = new Dictionary<string, string>()
                {
                    { "darkBlue", "00" },{ "darkGreen", "01" },{ "darkCyan", "02" },{ "darkRed", "03" },
                    { "darkMagenta", "04" },{ "darkYellow", "05" },{ "gray", "06" },{ "darkGray", "07" },
                    { "blue", "08" },{ "green", "09" },{ "cyan", "10" },{ "red", "11" },{ "magenta", "12" },
                    { "yellow", "13" },{ "white", "14" }
                };

            string colorChoice = "";

            Console.WriteLine("Was ist deine Lieblingsfarbe?");
            ts.DotLine();


            for (int i = 0; i < colorRange.Length; i++)
            {
                string counter = "0" + i;
                if (counter.Length > 2)
                {
                    counter = i.ToString();
                }

                setColor(colorRange[i]);
                Console.WriteLine(counter + " " + "■" + " " + colorRange[i]);

            }

            string? answer = Console.ReadLine();

            foreach (var kvp in colorDictionary)
            {
                if (kvp.Value == answer)
                {
                    colorChoice = kvp.Key;
                    break;
                }

            }
            return colorChoice;
        }
    }
}