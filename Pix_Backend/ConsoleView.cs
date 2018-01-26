using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Pix_Backend
{
    class ConsoleView : TextBox
    {
         


        public ConsoleView(bool redirectOutput)
        {
            this.Tag = "Console";
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.TextAlignment = TextAlignment.Center;
            this.AcceptsReturn = false;
            this.IsReadOnly = true;

        
            
    
            this.AllowDrop = false;
            

            this.Height = 350;
            this.Width = 350;
        
            //Set Output
            if (redirectOutput)
            {
                ConsoleWriter consoleOut = new ConsoleWriter(this);
                Console.SetOut(consoleOut);
                Console.WriteLine("<-----BEGIN----->");
               

            }


        }

        public void PrintLine(string text)
        {
            this.AppendText("\n" + text);
        }
        public void Print(string text)
        {
            this.AppendText(text);
        }


    }

    class ConsoleWriter : TextWriter
    {
        public override Encoding Encoding { get; }
        private ConsoleView console;

        public ConsoleWriter(ConsoleView console)
        {
            this.console = console;
        }

        public override void Write(char c)
        {
            console.Print(c+"");
        }

        public override void Write(string s)
        {
            console.PrintLine(s);
        }
    }
}
