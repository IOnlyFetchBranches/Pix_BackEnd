using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using MongoDB.Driver;
using Newtonsoft.Json;
using pix_dtmodel.Connectors;
using pix_dtmodel.Managers;
using pix_dtmodel.Models;

namespace Pix_Api.Controllers
{
    public class PicsController : ApiController
    {
        private static IMongoDatabase database =
            DataManager.Init(Defaults.ConnectionStrings.Localhost, Defaults.DatabaseNames.TestProd, null);

        private int MAX_QUERY_LENGTH = 100; // Max amount of photos that can be sent at a time

        private static Session<Pic> session = new Session<Pic>(database, Defaults.Collections.Pics);

        public async Task<Image> Get(string id)
        {
            var result = await session.GetRecordById(id);
            //Attempt to load image
            try
            {
                Image image = Image.FromFile(result.Location);

                return image;
            }
            catch (Exception e)
            {
                return null;
            }

        }

        public async Task<JsonResult<IQueryable<string>>> GetMultiple(string id, int amount)
        {
            if (amount > MAX_QUERY_LENGTH)
            {
                amount = MAX_QUERY_LENGTH;
            }

            var results = await session.GetSomeById(id,amount);
            //Attempt to load image
            try
            {
                List<string> images = new List<string>();
                foreach (Pic pic in results)
                {
                    var bytes = File.ReadAllBytes(pic.Location);
                    var b64 = Convert.ToBase64String(bytes); //b64  the images result

                    //Add image bytes as base 64.
                    images.Add(b64);


                }
                return new JsonResult<IQueryable<string>>(images.AsQueryable(),new JsonSerializerSettings(), Encoding.ASCII, Request);
            
            }
            catch (Exception e)
            {
                    return null;
            }

        }

        public async Task<IHttpActionResult> PostPicture()
        {
            try
            {
                
                var encoded = await Request.Content.ReadAsStringAsync();
               
                var bytes = Convert.FromBase64String(encoded);


                var json = Encoding.ASCII.GetString(bytes);

               
                var final = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);

                //Gather attributes
                string uid = (string) final["uid"];
                string mime = (string) final["mime"];







                //Sanitize first, will save as uri
                encoded = encoded.Replace("/", "");
                encoded = encoded.Replace("+", "");
                encoded = encoded.Replace("=", "");

                //Generate PID
                //p(encodedFirstTen)(uid skip3 then Ten)(Random 2 digit numb)(encodedreversedFive);

                string picid = (string) "p" + encoded.Substring(0, 10) + uid.Substring(3, 10) +
                               new Random().Next(10, 99) + encoded.Substring(50,5);

                Debug.WriteLine("\nCreated Pic:\n" + picid);

                //Get b64
                string pic64 = (string) final["data"];
                //Sav Picture [returns path]
                string path = PicManager.SavePicture(pic64, uid, picid, mime);

                //Create pics object
                Pic pic = new Pic();

                //Assign Attributes
                //Dates 
                pic.CreationDate = DateTime.Now;
                pic.ModifiedDate = DateTime.Now;
                //Check dates
                Debug.WriteLine("DATECHECK => " + pic.CreationDay + " " + pic.CreationMonth +" ");
                //Mandatory!
                pic.Uid = uid;
                pic.Pid = picid;
                pic.Location = path;
                pic.Name = (string) final["name"];
                //Everything else is optional, you can serialize it to the database now! 

                //Check for Location
                if (final.ContainsKey("Lon"))
                {
                    pic.Lon = (string) final["Lon"];
                    pic.Lat = (string)final["Lat"];

                }
                await session.Add(pic);

                //Return ok
                return new OkResult(Request);




            }
            catch (Exception e)
            {
                //Return bad
                Debug.WriteLine(e.Message);
                
                
                return new BadRequestErrorMessageResult(e.Message,this);
            }

        }

    }
}
