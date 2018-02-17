using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Services.Description;
using Newtonsoft.Json;
using pix_dtmodel.Util;
using pix_sec.Rules;

namespace Pix_Api.Models
{
    //Easily json Serializable class for quick http responses.
    public abstract class ResponseBuilder 
    {
        public static IRequestable Make()
        {
            return new ResponsePlans();
        }
        

    //Base
        public class ResponsePlans : IRequestable { 
            

        internal ResponsePlans()
        {
            //Does nothing really
        }
            public ISimpleRespondable AsSimple()
            {
                return new SimpleResponse();
            }

            public IMultipartRespondable AsMultipart()
            {
                return new MultiPartResponse();
            }
        }




    //Thread safe?
    public class SimpleResponse : ISimpleRespondable
    {

        private readonly IDictionary<string, string> content;

        private HttpStatusCode code;

        private MediaTypeHeaderValue type;


        internal SimpleResponse()
        {
            //Private contructor
            content = new ConcurrentDictionary<string, string>();
            
        }

        






        public HttpResponseMessage ToJsonResponse()
        {
            HttpResponseMessage res = new HttpResponseMessage(code);

            res.Content = new StringContent(ToJson());


            if (res == null)
            {
                throw new Exception("Response must exist!");
            }
            if(type != null)
                {
                    res.Content.Headers.ContentType = type;
                }

            

            return res;
        }

        public string ToJson()
        {
            var json = JsonConvert.SerializeObject(content);
            return json;
        }

        public ISimpleRespondable Add(string key, string data)
        {
            if(!content.ContainsKey(key))
                content.Add(key, data);
            return this;
        }

        public ISimpleRespondable Delete(string key)
        {
            content.Remove(key);
            return this;
        }

        public ISimpleRespondable WithCode(HttpStatusCode code)
        {
            this.code = code;
            return this;
        }

        public ISimpleRespondable WithType(string mime)
        {
            this.type = new MediaTypeHeaderValue(mime);
            return this;
        }
    }


    public interface IRequestable
    {
        IMultipartRespondable AsMultipart();
        ISimpleRespondable AsSimple();
    
        
        }

        //DEFINEINTERFACES
    public interface IRespondable<T>
    {
        T WithCode(HttpStatusCode code);
        }
        public interface ISimpleRespondable : IRespondable<ISimpleRespondable>
        {
            ISimpleRespondable WithType(string mime);
            ISimpleRespondable Add(string key, string data);
            ISimpleRespondable Delete(string key);
            HttpResponseMessage ToJsonResponse();
            string ToJson();
        }

        public interface IMultipartRespondable : IRespondable<IMultipartRespondable>
        {
            IMultipartRespondable PutBytes(string filename, byte[] bytes, string mime);
            HttpResponseMessage ToMultiPartResponse();
        }

        //DONE
        //Multipart
        private class MultiPartResponse : IMultipartRespondable
        {
            private MultipartFormDataContent content;
            private readonly string boundary = "--"+ DateTime.Now.ToString("MMddyyyyhhmmss") + "--";

            private List<MediaTypeHeaderValue> types = new List<MediaTypeHeaderValue>();
            private List<string> names = new List<string>();


            private HttpStatusCode code = HttpStatusCode.OK;
            

            internal MultiPartResponse()
            {
                //Setups?
                content = new MultipartFormDataContent(boundary);

            }




              
            

            public IMultipartRespondable WithCode(HttpStatusCode code)
            {
                this.code = code;
                return this;
            }

            public IMultipartRespondable PutBytes(string filename, byte[] bytes, string mime)
            {
                var data = new ByteArrayContent(bytes);

                var type = new MediaTypeHeaderValue(mime);

                types.Add(type);

            ;
                

                data.Headers.Add("Content-Type", mime);

                //if filename has 2 quotes already
                if(filename.StartsWith("\"") && filename.EndsWith("\""))
                    data.Headers.Add("Content-Disposition", "attachment; filename=" + filename);
                else
                {
                    filename = filename.Replace("\"", "");
                    filename = "\"" + filename + "\"";
                    data.Headers.Add("Content-Disposition", "attachment; filename=" + filename);
                }

                names.Add(filename);



                content.Add(data);

                return this;

            }

            public HttpResponseMessage ToMultiPartResponse()
            {
                
                var res = new HttpResponseMessage(code);
                res.Content = content;

                if (types.Count == 0)
                {
                    res.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                }
                else
                {
                    var multiContent = res.Content.ReadAsMultipartAsync();
                }



                DtLogger.LogG("ResponseBuilder", "Returning Multipart!");

                return res;
            }

           
        }

        }
}