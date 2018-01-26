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

        private  Profile baseProf = new Profile();

        //Controls
        private readonly int MAX_QUERY_ITEM_COUNT = 1000; //The max a single request can get back at a time

      public  async Task<JsonResult<IQueryable<Profile>>> Get(bool use,int limit)
      {

          
          var settings = new JsonSerializerSettings();

            //Null List
          baseProf.Username = "NULL LIST";
          List<Profile> emptyList = new List<Profile>()
          {
              baseProf
          };




            //If limited
            if (use)
          {
              if (limit > MAX_QUERY_ITEM_COUNT)
              {
                  limit = MAX_QUERY_ITEM_COUNT;
              }
                 var results = await session.GetAll(limit);
              var returnable = results != null;
              if(returnable)
                return new JsonResult<IQueryable<Profile>>(results.Take(limit), settings, Encoding.ASCII, Request);
              else
              {
                  return new JsonResult<IQueryable<Profile>>(emptyList.AsQueryable(), settings, Encoding.ASCII, Request);
                }
            }


          var dResults = await session.GetAll(limit);
          var empty = dResults != null;
            
          if(!empty)
            return new JsonResult<IQueryable<Profile>>(dResults.Take(MAX_QUERY_ITEM_COUNT), settings, Encoding.ASCII, Request );
          else
          {
             
              return new JsonResult<IQueryable<Profile>>(emptyList.AsQueryable(),settings,Encoding.ASCII, Request);
          }


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
