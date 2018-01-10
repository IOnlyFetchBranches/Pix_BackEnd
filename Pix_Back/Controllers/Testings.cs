using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using pix_dtmodel;
using pix_dtmodel.Models;
using WebGrease.Extensions;


namespace Pix_Back.Controllers
{
    public class Testing
    {
        private static IMongoDatabase connection;
        private static Session<pix_dtmodel.Models.Pic> picsSession;

        private void addAndDisplay()

        {
            connection = DataManager.Init("mongodb://localhost", "admin", null);
            picsSession = new Session<Pic>(connection,"pics");
        }

         void main(String[] args)
        {
            addAndDisplay();
        }


    }
}