﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Client
{
    internal class TextSnipplets
    {

        public void Welcome()
        {
            string a = "Wilkommen im Chat.";
            string b = "Erzähle uns zunächst etwas über dich";
            string c = "Wie heißt du?";

            Console.WriteLine(a);
            Console.WriteLine(b);
            Console.WriteLine(c);
            
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

        public void Name()
        {
            string a = "Wie heißt du?";
            Console.WriteLine(a);
        }

        public void HaveFun(string color, string alias)
        {
            ColorSettings cs = new ColorSettings();

            Console.Clear();
            Console.Write("Viel spaß beim Chatten ");
            cs.setColor(color);
            Console.Write(alias);
            cs.setColor("white");
            Console.Write(".");
        }

    }
    
    
}