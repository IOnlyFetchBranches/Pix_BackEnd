using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pix_sec.Rules
{
    public abstract class Text
    {

        public static char[] AllowedCharacters => new char[]
                {
                    'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
                    'u', 'v', 'w', 'x', 'y', 'z', '0','1','2','3','4','5','6','7','8','9', '-', '/','*','$','#','@','!','^','&'
                };

        

    }
}
