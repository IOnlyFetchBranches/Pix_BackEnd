using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using pix_dtmodel;
using pix_dtmodel.Managers;
using pix_dtmodel.Models;
using pix_sec;
namespace Tester
{   
    class Program
    {

        private static LoginManager login = pix_sec.LoginManager.getInstance();

        static void Main(string[] args)
        {

            string hashedPass =
                BitConverter.ToString(new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes("LOLMYPASSWORD")));

            Task<User> testUsr = login.CreateNewUser("llmuzical@gmail.com", hashedPass, "Muzical");


            //Literally just wait for the callbacks lol
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
