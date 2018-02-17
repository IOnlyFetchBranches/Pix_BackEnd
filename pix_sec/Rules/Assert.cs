using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace pix_sec.Rules
{
    public abstract class Assert
    {
        public static void AssertExists(params object[] subjects)
        {
            foreach(object o in subjects)
            {
                if (o == null || o.Equals(null))
                {
                    throw new SecurityException("Failed Assert: Exists \n");
                }
            }
        }
    }
}
