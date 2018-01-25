using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MongoDB.Driver;
using pix_dtmodel.Connectors;
using pix_dtmodel.Managers;
using pix_dtmodel.Models;

namespace Pix_Backend
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        



        public MainWindow()
        {
        
            //Start window
            InitializeComponent();


            //Hide Stuff
            Login_PBar.Opacity = 0;
            //Control Sanitizing
             bool isValid = false;
            //Define Componentsdo
            Login_Button.Click += ((sender, e) =>
            {
                

                    isValid = true;
                    //Define on click to check the boxes and request up against the backend
                    //Sanitize

                    //Check Blanks
                    if (Login_Username.Text.Contains(" "))
                    {
                        isValid = false;
                        var oldcolor = Login_Username.Foreground; //Save old color
                        Login_Username.Foreground = new SolidColorBrush(Colors.Red); //Set new color

                        //Prompt User
                        MessageBox.Show(MainWindow.GetWindow(this), "Username Cannot Contain Spaces!", "Invalid Entry");
                        Login_Username.Foreground = oldcolor;


                    }

                    if (Login_Password.Password.Equals(String.Empty) && isValid)
                    {
                        isValid = false;
                        var oldcolor = Login_Password.Foreground; //Save old color
                        Login_Password.Foreground = new SolidColorBrush(Colors.Red); //Set new color

                        //Prompt User
                        MessageBox.Show(MainWindow.GetWindow(this), "Enter A Password!", "Invalid Entry");
                        Login_Password.Foreground = oldcolor;


                    }

                    if (Login_Password.Password.Contains(" ") && isValid)
                    {
                        isValid = false;
                        var oldcolor = Login_Password.Foreground; //Save old color
                        Login_Password.Foreground = new SolidColorBrush(Colors.Red); //Set new color

                        //Prompt User
                        MessageBox.Show(MainWindow.GetWindow(this), "Password Cannot Contain Spaces!", "Invalid Entry");
                        Login_Password.Foreground = oldcolor;


                    }


                    //Check chars
                    if (isValid)
                        foreach (char c in Login_Username.Text)
                        {
                            if (pix_sec.Rules.Text.AllowedCharacters.Contains(c)
                        || pix_sec.Rules.Text.AllowedCharacters.Contains(Char.ToLower(c)))
                            {
                                continue;
                            }
                            else
                            {
                                MessageBox.Show(this, "Username cannot contain character : " + c + "!");

                                isValid = false;
                            }

                        }

                    if (isValid)
                    {
                        var hasher = new SHA256Managed();
                        //Grab password and username
                        var username = Login_Username.Text;
                        var password = Login_Password.Password;

                        byte[] hashedBytes = hasher.ComputeHash(Encoding.ASCII.GetBytes(password));

                        AttemptLoginToBack(username, hashedBytes, false);

                    }
              
            });

            //Animate Here
            DoubleAnimation fadeOutThenIn = new DoubleAnimation(.7, 1, new Duration(TimeSpan.FromSeconds(2)));
            fadeOutThenIn.AutoReverse = true;
            fadeOutThenIn.RepeatBehavior = RepeatBehavior.Forever;
            fadeOutThenIn.AccelerationRatio = .4;
            fadeOutThenIn.DecelerationRatio = .5;

            //End of fade Animation
            



            Storyboard animator = new Storyboard();
            animator.Children.Add(fadeOutThenIn);
            //Static property setting for the Looper
            //Set target
            Storyboard.SetTargetName(fadeOutThenIn, Login_Logo.Name);
            Storyboard.SetTargetProperty(fadeOutThenIn, new PropertyPath(OpacityProperty));
            

            //Set triggers
            Login_Logo.Loaded += (sender, a) =>
            {
                //Begin

                animator.Begin(this);

            };

            //Check for presaved Hardware Sensitive creds
            try
            {
                Directory.CreateDirectory("C:\\Users\\" + Environment.UserName + "\\.pix\\cred\\");
                FileStream credIn = new FileStream("C:\\Users\\" + Environment.UserName + "\\.pix\\cred\\sesh.psh",
                    FileMode.Open);
                FileStream ivIn = new FileStream("C:\\Users\\" + Environment.UserName + "\\.pix\\cred\\iv.pcrd",
                    FileMode.Open);

                byte[] cryptSec = new byte[credIn.Length];
                byte[] Iv = new byte[ivIn.Length];

                ivIn.Read(Iv, 0, Iv.Length); //tan
                credIn.Read(cryptSec, 0, cryptSec.Length); //dtani n

                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();

                byte[] key = SHA256Managed.Create().TransformFinalBlock(Encoding.ASCII.GetBytes(GetHwid()), 0,
                    Encoding.ASCII.GetBytes(GetHwid()).Length);
                aes.KeySize = 256;
                aes.IV = Iv;
                aes.Key = key;

                //get Bytes;

                byte[] good = aes.CreateDecryptor().TransformFinalBlock(cryptSec, 0, cryptSec.Length);

                //
                MemoryStream mem = new MemoryStream(good);
                







                if (credIn.Length > 0)
                {
                    BinaryFormatter reader = new BinaryFormatter();
                    SystemRecord.Cred cred = (SystemRecord.Cred) reader.Deserialize(mem);

                    //Attempt Login
                    AttemptLoginToBack(cred.Username, Encoding.ASCII.GetBytes(cred.Hashword), true);

                }
                else
                {
                    //Show warning prompt.
                    MessageBox.Show(this, "For Authorized Users Only.", "Welcome to Pix");

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Non-Preload\n"+e.Message);
            }
         



            



        }

        private async void AttemptLoginToBack(string username, byte[] hashedWordBytes, bool wasSaved) 
        {
            //Graphic stuff
            Login_Button.Opacity = 0;
            Login_Button.IsEnabled = false;
            Login_PBar.Opacity = 1;


           //Open temp DB session to unofficial db
            IMongoDatabase tempData = DataManager.Init(Defaults.ConnectionStrings.Localhost, "config", null);
            Session<pix_dtmodel.Models.SystemRecord.Cred> authChannel = new Session<SystemRecord.Cred>(tempData,"pix_sysadmins");
            //Secure hash again.
            string hashedWord = null;
            if (!wasSaved)
                hashedWord = BitConverter.ToString(new SHA256Managed().ComputeHash(hashedWordBytes))
                    .Replace("-", String.Empty);
            else
            {
                hashedWord = Encoding.ASCII.GetString(hashedWordBytes);
            }
            //Besure to convert username to all lowercase!
            username = username.ToLower();
            //Check
            bool pass = await authChannel.CheckFieldFrom(username, "hashword",hashedWord);

            if (!pass)
            {
                MessageBox.Show(this, "Incorrect Username/Password!", "Invalid Login...", MessageBoxButton.OK,
                    MessageBoxImage.Hand);
                //Graphic stuff
                Login_Button.Opacity = 1;
                Login_Button.IsEnabled = true;
                Login_PBar.Opacity = 0;
            }
            else
            {
                MessageBox.Show(this, "Accepted!", "Welcome "+username, MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);

                //If Remember Credentials was checked, save credentials
                if (Login_RemeberCheckbox.IsChecked.Value)
                {
                    FileStream credOut = new FileStream("C:/Users/" + Environment.UserName + "/.pix/cred/sesh.psh",
                        FileMode.OpenOrCreate);

                    //Get cred
                    var cred = await authChannel.GetRecordById(username);

                    BinaryFormatter serializer = new BinaryFormatter();
                    MemoryStream memCred = new MemoryStream();
                    //Read to mem
                    serializer.Serialize(memCred, cred);

                    memCred.Seek(0, SeekOrigin.Begin);

                    var mem = new byte[memCred.Length];

                    memCred.Read(mem, 0, mem.Length);

                    AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                    byte[] key = SHA256Managed.Create().TransformFinalBlock(Encoding.ASCII.GetBytes(GetHwid()), 0,
                        Encoding.ASCII.GetBytes(GetHwid()).Length);
                    aes.KeySize = 256;
                    aes.GenerateIV();
                    aes.Key = key;
                    var iv = aes.IV;
                    //Write Bytes;
                    File.Create("C:/Users/" + Environment.UserName + "/.pix/cred/iv.pcrd").Write(iv, 0, iv.Length);

                    var crypt = aes.CreateEncryptor(aes.Key, aes.IV); // Encrypted

                    var cryptMem = crypt.TransformFinalBlock(mem,0,mem.Length);

                    //Write
                    credOut.Write(cryptMem,0,cryptMem.Length);






                }


                //Proceed with application

                var app_entry = new PrimaryApp();
                app_entry.Show();
                this.Close();
                // System.Environment.Exit(0); //Nothing yet

            }
        }

        private string GetHwid()
        {
            var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            ManagementObjectCollection mbsList = mbs.Get();
            string id = "";
            foreach (ManagementObject mo in mbsList)
            {
                id = mo["ProcessorId"].ToString();
                break;
            }

            return id;
        }
    
    }
}
