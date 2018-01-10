using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using pix_dtmodel;
using pix_dtmodel.Models;

namespace Tester
{   
    class Program
    {
        
        static void Main(string[] args)
        {
            Pic testPic= new Pic();
            testPic.Pid = 1 + "";
            testPic.Uid = 1 + "";
            testPic.Location = "my-url-here";
            DataManager.Testing.addAndDisplayPic(testPic);

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
