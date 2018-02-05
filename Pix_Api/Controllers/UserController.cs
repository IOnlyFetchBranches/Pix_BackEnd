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

      private readonly int MAX_QUERY_PAGE_SIZE = 100; //The max a single page request can pull per page.


        [ActionName("UserGetAll")]
        public  async Task<JsonResult<IQueryable<Profile>>> Get(int limit)
      {

          
          var settings = new JsonSerializerSettings();

            //If limited
          
              if (limit > MAX_QUERY_ITEM_COUNT)
              {
                  limit = MAX_QUERY_ITEM_COUNT;
              }

            var results = await session.GetAll(limit);
            return new JsonResult<IQueryable<Profile>>(results, settings, Encoding.ASCII, Request);
            
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

        [ActionName("GetUsersByPage")]
      public async Task<JsonResult<IQueryable<Profile>>> GetUsersByPage(long itemsperpage, long pagenum)
      {
          if (itemsperpage > MAX_QUERY_PAGE_SIZE)
          {
            itemsperpage = MAX_QUERY_PAGE_SIZE;
          }
          if (pagenum > Int64.MaxValue)
          {
              return null; //Too_Many_Pages
          }

          if (pagenum > 1)
          {
              var skip = pagenum * itemsperpage;
              var results = await session.GetSomeAfterOffset((int) skip, (int) itemsperpage);
              return new JsonResult<IQueryable<Profile>>(results, new JsonSerializerSettings(), Encoding.ASCII, this);
          }
          else //iF on page one
          {
                //Run basic query 
              var results = await session.GetSomeAfterOffset(0, (int)itemsperpage);
              return new JsonResult<IQueryable<Profile>>(results, new JsonSerializerSettings(), Encoding.ASCII, this);
            }
          
      }


        
        
  }
}
