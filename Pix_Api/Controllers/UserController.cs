using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using MongoDB;
using MongoDB.Driver;
using Newtonsoft.Json;
using pix_dtmodel.Connectors;
using pix_dtmodel.Managers;
using pix_dtmodel.Models;

namespace Pix_Api.Controllers
{
  public class UserController : ApiController
  {
        private static IMongoDatabase database =
          DataManager.Init(Defaults.ConnectionStrings.Localhost, Defaults.DatabaseNames.TestProd, null);

      private Session<Profile> session = new Session<Profile>(database, Defaults.Collections.Users);

        //Controls
      private readonly int MAX_QUERY_ITEM_COUNT = 1000; //The max a single request can get back at a time

      public  async Task<JsonResult<IQueryable<Profile>>> Get(bool use,int limit)
      {

          var results = await session.GetAll(limit);
          var settings = new JsonSerializerSettings();
            var httpreq = new HttpRequestMessage();

        

          //If limited
          if (use)
          {
              if (limit > MAX_QUERY_ITEM_COUNT)
              {
                  limit = MAX_QUERY_ITEM_COUNT;
              }
              
              return new JsonResult<IQueryable<Profile>>(results.AsQueryable().Take(limit), settings, Encoding.ASCII, httpreq);
            }
            
        return new JsonResult<IQueryable<Profile>>(results.AsQueryable().Take(MAX_QUERY_ITEM_COUNT), settings, Encoding.ASCII, httpreq );


      }

      public async Task<JsonResult<Profile>> GetUserById(string id)
      {
          var result = await session.GetRecordById(id);
          if (result == null)
          {
              return null;
          }
          else
          {
                //Filter result
        
              return new JsonResult<Profile>(result,new JsonSerializerSettings(),Encoding.ASCII, new HttpRequestMessage());
          }
      }

        
        
  }
}
