using System;
using System.IO;
using System.Text;

namespace pix_dtmodel.Util
{
    public abstract class DtLogger
    {

        //Default log
        private static FileStream LogFile;

         static DtLogger()
        {
           //Create a default Directory +Log File if it doesnt exist
            Directory.CreateDirectory("C:\\DebugLogs\\Global");

            LogFile = new FileStream("C:\\DebugLogs\\Global\\LogOut-"+ DateTime.Now.ToString("MM-dd-yyyy")+".log",
                FileMode.Append , FileAccess.Write, FileShare.Read);
            //Done

            AppDomain.CurrentDomain.ProcessExit += (args, s) =>
            {
                //Close on app exit
                LogG("System", "<Exit Called> Closing File Handles...");
                LogFile.Dispose();


            };

        }
        




        public static void Log(string Message, FileStream logger)
        {
            var newLine = Encoding.ASCII.GetBytes(Environment.NewLine);

            logger.Write(newLine,0,newLine.Length);
            logger.Write(Encoding.ASCII.GetBytes(Message), 0, Encoding.ASCII.GetBytes(Message).Length);
            

        }

        //Log to Global
        public static void LogG(string From, string Message)
        {

            //Open a file
            var newLine = Encoding.ASCII.GetBytes(Environment.NewLine);
            //Format it properly
            Message = From +" ["+ DateTime.Now.ToString("hh:mm:ss") +"]" + ":" + Message;
            //Write new Line
            LogFile.Write(newLine, 0, newLine.Length);
            //Write Message
            LogFile.Write(Encoding.ASCII.GetBytes(Message), 0, Encoding.ASCII.GetBytes(Message).Length);

            //Flush to file
            LogFile.Flush();

           
        }
    }
}