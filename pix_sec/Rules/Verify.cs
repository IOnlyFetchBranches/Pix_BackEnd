using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pix_dtmodel.Connectors;
using pix_dtmodel.Managers;
using pix_dtmodel.Models;
using pix_dtmodel.Util;

namespace pix_sec.Rules
{
    public abstract class Verify
    {
        //Used to verify actions
        private static Session<SessionToken> tokenSession = new Session<SessionToken>(
            DataManager.Init(Defaults.ConnectionStrings.Localhost,Defaults.DatabaseNames.TestProd, null),
            Defaults.Collections.SessionData
            );

        public async static Task<bool> CheckUser(pix_dtmodel.Models.SessionToken token)
        {
            var result = tokenSession.CheckFieldFrom(token.Gid, Defaults.Fields.Users.Uid, token.Uid);
            if (result.Exception != null)
            {
                DtLogger.LogG("Verify",result.Exception.Message);
            }
            return await result;
        }

    }
}
