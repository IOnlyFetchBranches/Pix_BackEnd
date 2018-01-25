using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pix_dtmodel.Managers
{
    public abstract class PicManager
    {
        private static string baseDir = "C:\\Pix\\Users\\"; //Root Dir


        public static string SavePicture(string pic64,string uid, string pid, string MIME)
        {
            DirectoryInfo userDir = Directory.CreateDirectory(baseDir + uid + "\\pics");
            
            //Convert Picture64 to bytes.
            var bytes = Convert.FromBase64String(pic64);

            //Check or you can write it directly to a file
            
            //For now just write it
            if (MIME.Contains("jpeg"))
            {
                File.WriteAllBytes(userDir.FullName + "\\" + pid + ".jpg", bytes);
                return userDir.FullName + "\\" + pid + ".jpg";
            }
            else
            {
                return "Jpeg Mime not recieved!";
            }



        }
    }
}
