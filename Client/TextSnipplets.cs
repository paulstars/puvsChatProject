﻿namespace Client
{
    internal class TextSnipplets
    {

        public void Welcome()
        {
            System.Console.WriteLine("Wilkommen im Chat.");
            System.Console.WriteLine("Erzähle uns zunächst etwas über dich");
            System.Console.WriteLine("Wie heißt du?");            
        }

        public void DotLine()
        {
            int conWidth = Console.WindowWidth;

            for (int i = 0; i < conWidth; i++)
            {
                Console.Write("-");
            }

            Console.WriteLine("");
        }
        
        public void HaveFun(string color, string alias)
        {
            ColorSettings cs = new ColorSettings();

            //Console.Clear();
            Console.Write("Viel spaß beim Chatten ");
            cs.setColor(color);
            Console.Write(alias);
            cs.setColor("white");
            Console.Write(".");
        }

    }
    
    
}