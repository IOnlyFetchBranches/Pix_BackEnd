using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using Newtonsoft.Json;
using pix_dtmodel.Connectors;
using pix_dtmodel.Managers;
using pix_dtmodel.Models;
using pix_sec;
using pix_sec.Gen;
using pix_sec.Rules;
using Pix_Api.Models;
using static pix_dtmodel.Util.DtLogger;

namespace Pix_Api.Controllers
{
    public class PicsController : ApiController
    {
        private static IMongoDatabase database =
            DataManager.Init(Defaults.ConnectionStrings.Localhost, Defaults.DatabaseNames.TestProd, null);

        private int MAX_QUERY_LENGTH = 100; // Max amount of photos that can be sent/retrieved at a time

        //Open a connection with the Pic collection
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
            //Limit Query length.
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

        //Returns a json result of a querable of json strings...
        [System.Web.Http.ActionName("GetPicsFromUsersByPage")]
        public async Task<JsonResult<IQueryable<string>>> GetPicsFromUsersByPage(long pagenum, long size)
        {
            return null;
        }

        //NonMultipart
        //Required data ["uid","gid","lat","long","data"]
        //Deprecated for now, hopefully forever 
        /*
        public async Task<IHttpActionResult> PostPicture()
        {
            try
            {
                
                    LogG("PicController", "Retrieving Data...");

                    //Read json here

                var encoded = await Request.Content.ReadAsStringAsync();

                LogG("PicController", "Done.");

                var bytes = Convert.FromBase64String(encoded);

                LogG("PicController", "Decoding...");


                var json = Encoding.ASCII.GetString(bytes);

                LogG("PicController", "Deserializing...");


                var final = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);

                LogG("PicController", "Done.");


                //Gather attributes
                string uid = (string) final["uid"];
                string gid = (string) final["gid"]; //This will now be a gid.
                string mime = (string) final["mime"];

                LogG("PicController", "Verify Integrity of Fields...");
                //These must exist
                try
                {
                    //Test
                    pix_sec.Rules.Assert.AssertExists(uid, gid, mime);
                }
                catch (Exception e)
                {
                    LogG("PicController", "Failed-"+e.Message);
                    Debug.WriteLine(e.Message);
                    //Failed
                    return null;
                }

                LogG("PicController", "Passed.");







                LogG("PicController", "Sanitizing...");

                //Sanitize first, will save as uri
                encoded = encoded.Replace("/", "");
                encoded = encoded.Replace("+", "");
                encoded = encoded.Replace("=", "");

                //Generate PID
                //p(encodedFirstTen)(uid skip3 then Ten)(Random 2 digit numb)(encodedreversedFive);
             
                string picid = ID.GenPicId(uid);
                LogG("PicController", "Id Generated -" + picid);

                Debug.WriteLine("\nCreated Pic:\n" + picid);

                //Get b64
                string pic64 = (string) final["data"];
                //Save Picture [returns path]
                string path = PicManager.SavePicture(pic64, uid, picid, mime);

                LogG("PicController", "Saved.");
                //Get Location [Super long instanstiation begins here]
                GeoJsonPoint<GeoJson2DGeographicCoordinates> point = 
                    new GeoJsonPoint<GeoJson2DGeographicCoordinates>
                        (new GeoJson2DGeographicCoordinates
                            (Double.Parse((string)final["long"]), Double.Parse( (string) final["lat"])));

                LogG("PicController", "GeoData Retrieved");





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
                pic.Gid = gid; //This used to be token
                pic.Pid = picid;
                pic.Location = path;
                pic.GeoData = point;
                pic.Name = (string) final["name"];
                //Everything else is optional

                //Fille Lon/Lat properties for ease of access 
                if (final.ContainsKey("Lon"))
                {
                    pic.Lon = (string) final["Lon"];
                    pic.Lat = (string)final["Lat"];

                }


                LogG("PicController", "Model Filled For Pic" + picid);
                //Verify that the pic is from the proper user. (In case of MITM)
                var verified = await pix_sec.Rules.Verify.CheckUser(new SessionToken() {Gid = gid, Uid = uid});

                LogG("PicController", "Verified pid" +picid);
                //Add to database.
                if (verified)
                {
                    await session.Add(pic);
                    LogG("PicController", picid + " added to dbase.");
                }
                    

                //Return ok
                return new OkResult(Request);




            }
            catch (Exception e)
            {
                //Return bad
                Debug.WriteLine(e.Message);
                LogG("PicController", "Error: " + Request.Headers.From +" \n"+ e.Message);

                return new BadRequestErrorMessageResult(e.Message,this);
            }

        }

    */


        //Multipart
        //Required data ["uid","gid","mime","lat","long", "name"]
        //First Step of the multipart process.
        [System.Web.Http.ActionName("begin")]
        public async Task<HttpResponseMessage> BeginUpload()
        {



            {
                try
                {



                    LogG("PicController", "Retrieving Metadata...");

                    //Read json here

                    var encoded = await Request.Content.ReadAsStringAsync();

                    LogG("PicController", "Done.");

                    var bytes = Convert.FromBase64String(encoded);

                    LogG("PicController", "Decoding...");


                    var json = Encoding.ASCII.GetString(bytes);

                    LogG("PicController", "Deserializing...");


                    var final = JsonConvert.DeserializeObject<IDictionary<string, object>>(json);

                    LogG("PicController", "Done.");


                    //Gather attributes
                    string uid = (string)final["uid"];
                    string gid = (string)final["gid"]; //This will now be a gid.
                    string mime = (string)final["mime"];
                    //string token = (string)final["token"]; //Not checking while debugging to make life easier.


                    LogG("PicController", "Verifying Integrity of Fields...");
                    //These must exist
                    try
                    {
                        //Test
                        if (gid == null || mime == null || uid == null)
                        {
                            throw new Exception("An enforced field is missing from the request.");
                        }
                    }
                    catch (Exception e)
                    {
                        LogG("PicController", "Failed-" + e.Message);
                        Debug.WriteLine(e.Message);
                        //Failed
                        return null;
                    }

                    LogG("PicController", "Passed.");







                    LogG("PicController", "Sanitizing for filename : \n\n" + encoded);

                    //Sanitize first, will save as uri
                    encoded = encoded.Replace("/", "");
                    encoded = encoded.Replace("+", "");
                    encoded = encoded.Replace("=", "");

                    LogG("PicController", "Done : \n\n" + encoded);

                    //Generate PID
                    //p(encodedFirstTen)(uid skip3 then Ten)(Random 2 digit numb)(encodedreversedFive);

                    string picid = ID.GenPicId(uid);
                    LogG("PicController", "Id Generated -" + picid);

                    Debug.WriteLine("\nCreated PicHolder:\n" + picid);

        
                   
                    //Get Location [Super long instanstiation begins here]
                    GeoJsonPoint<GeoJson2DGeographicCoordinates> point =
                        new GeoJsonPoint<GeoJson2DGeographicCoordinates>
                            (new GeoJson2DGeographicCoordinates
                                (Double.Parse((string)final["long"]), Double.Parse((string)final["lat"])));

                    LogG("PicController", "GeoData Retrieved");





                    //Create pics object
                    Pic pic = new Pic();


                    //Assign Attributes
                    //Dates 
                    pic.CreationDate = DateTime.Now;
                    pic.ModifiedDate = DateTime.Now;
                    //Check dates
                    Debug.WriteLine("DATECHECK => " + pic.CreationDay + " " + pic.CreationMonth + " ");
                    //Mandatory!
                    pic.Uid = uid;
                    pic.Gid = gid; //This used to be token
                    pic.Pid = picid;
                    pic.Location = "null";
                    pic.GeoData = point;
                    pic.Name = (string)final["name"];
                    pic.Filled = false;
                    //Everything else is optional

                    //Fille Lon/Lat properties for ease of access 
                    if (final.ContainsKey("Lon"))
                    {
                        pic.Lon = (string)final["Lon"];
                        pic.Lat = (string)final["Lat"];

                    }


                    LogG("PicController", "Model Filled For Pic" + picid);
                    //Verify that the pic is from the proper user. (In case of MITM)
                    var verified = await pix_sec.Rules.Verify.CheckUser(new SessionToken() { Gid = gid, Uid = uid });

                    LogG("PicController", "verified =" + verified + " pid=" + picid);

                    //Add to database.
                    if (verified)
                    {
                        LogG("PicController", picid + " adding to dbase");
                        await session.Add(pic);
                        LogG("PicController", picid + " added to dbase.");
                    }
                    else
                    {
                        return ResponseBuilder.Make()
                            .AsSimple()
                            .Add("err", "Verification failure...")
                            .Add("testing", gid + "_aDa/" + uid) //Remove this after debugging...
                            .ToJsonResponse();
                    }

                    //Respond with continue status, uid + url to post upload to.
                    return ResponseBuilder.Make()
                        .AsSimple()
                        .Add("uid", uid)
                        .Add("url", "https://www.api.gopix.xyz/pics/upload/go/" + picid)
                        .WithCode(HttpStatusCode.OK)
                        .ToJsonResponse();




                }
                catch (Exception e)
                {
                    //Return bad
                    Debug.WriteLine(e.Message);
                    LogG("PicController", "Error: " + Request.Headers.From + " \n" + e.Message);



                    return ResponseBuilder.Make()
                        .AsSimple()
                        .WithType("application/json")
                        .Add("err", e.Message)
                        .Add("trace", e.StackTrace)
                        .ToJsonResponse();
                }

            }

        }

        //Required data [multipartdata, set filename in header Content Disposition]
        //Last Step of the multipart process.
        [System.Web.Http.Route("pics/upload/go/{picId}")]
        public async Task<HttpResponseMessage> BeginUpload(string picId)
        {
            

            try
            {
                LogG("PicController", "Go");

                //Get metadata placeholder
                var emptyPic = await session.GetRecordById(picId);


                if (emptyPic == null)
                {
                    LogG("Sec","Invalid Response from "+Request.Headers.From+ Environment.NewLine);
                    return ResponseBuilder.Make()
                        .AsSimple()
                        .WithCode(HttpStatusCode.BadRequest)
                        .Add("err", "Problem with upload link!")
                        .Add("reason", "Could not find data for " + picId)
                        .ToJsonResponse();
                }

                LogG("PicController", "Loading Content from endpoint...");

                //Check if it has been filled
                if (emptyPic.Filled)
                {
                    //Its an expired link 
                    return ResponseBuilder.Make()
                        .AsSimple()
                        .WithCode(HttpStatusCode.Conflict)
                        .Add("message","Link has expired or already used!")
                        .ToJsonResponse();
                }

                //read multipart, if it's multipart of course
                if (Request.Content.IsMimeMultipartContent())
                {
                    LogG("PicController", "Multipart Upload Incoming...");
                    //If multipart
                    var mpStream = await Request.Content.ReadAsMultipartAsync();
                    LogG("PicController", "uid:" + emptyPic.Uid + " " + "Read Stream");
                    ;


                    LogG("PicController", "uid:" + emptyPic.Uid + " " + "Reading... "
                                          + Environment.NewLine + mpStream.Contents.Count + " file(s)...");

                    //Read each file
                    foreach (HttpContent file in mpStream.Contents)
                    {
                        //Get filename
                        var name = file.Headers.ContentDisposition.FileName;

                        LogG("PicController", "name - " + name);

                        //Count var
                        long count = 0;

                        if (file.Headers.ContentType.ToString().Contains("image"))
                        {
                            count++;

                            LogG("PicController", "Processing file(" + count + ")" + " uid = " + emptyPic.Uid);
                            //It's a jpeg
                            var bytes = await file.ReadAsByteArrayAsync();
                            LogG("PicController", "uid:" + emptyPic.Uid + " " + "Encoding...");
                            var b64 = Convert.ToBase64String(bytes);
                            LogG("PicController", "uid:" + emptyPic.Uid + " " + "Saving...");

                            LogG("Debug", "Headers " + file.Headers.ContentType.ToString());


                            var path = PicManager.SavePicture
                                (b64, emptyPic.Uid, picId, file.Headers.ContentType.ToString());

                        

                            LogG("PicController", "uid:" + emptyPic.Uid + " " + "Updating record...");
                            emptyPic.Location = path;
                            emptyPic.Filled = true;
                            
                            var doUpdate = await session.UpdateRecordById(picId, "Location", emptyPic);
                            var doFinalUpdate = await session.UpdateRecordById(picId, "Filled", emptyPic);
        
                            LogG("PicController", "uid:" + emptyPic.Uid + " " + "Done.");


                            Assert.AssertExists(name,Convert.FromBase64String(b64), new MediaTypeHeaderValue("image/jpeg"));


                            //return image data
                            //Files going from server should always be jpeg.
                            return ResponseBuilder.Make()
                                .AsSimple()
                                .Add("status", "success")
                                .Add("pid", emptyPic.Pid)
                                .ToJsonResponse();
                        }
                        else
                        {
                            return ResponseBuilder.Make()
                                .AsSimple()
                                .Add("err", "Content not image...")
                                .WithCode(HttpStatusCode.BadRequest)
                                .ToJsonResponse();
                        }

                    }



                }
                else
                {
                    return ResponseBuilder.Make()
                        .AsSimple()
                        .WithCode(HttpStatusCode.BadRequest)
                        .Add("err", "Data recieved was not multipart and/or corrupted!")
                        .ToJsonResponse();
                }


                return ResponseBuilder.Make()
                    .AsSimple()
                    .WithCode(HttpStatusCode.InternalServerError)
                    .Add("err", "Somehow did not fit a condition...")
                    .ToJsonResponse();
            }
            catch (Exception e)
            {
                return ResponseBuilder.Make()
                    .AsSimple()
                    .WithCode(HttpStatusCode.BadRequest)
                    .Add("err", e.Message)
                    .Add("stack", e.StackTrace)
                    .ToJsonResponse();
            }
        }

        


    }
}
