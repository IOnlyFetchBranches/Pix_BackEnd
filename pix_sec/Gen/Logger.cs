﻿using System;
using System.IO;
using System.Text;

namespace pix_sec.Gen
{
    public abstract class Logger
    {
        public static void Log(string Message, FileStream logger)
        {
            var newLine = Encoding.ASCII.GetBytes(Environment.NewLine);

            logger.Write(newLine,0,newLine.Length);
            logger.Write(Encoding.ASCII.GetBytes(Message), 0, Encoding.ASCII.GetBytes(Message).Length);
            

        }
    }
}