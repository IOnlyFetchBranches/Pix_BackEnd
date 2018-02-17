using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pix_dtmodel.Adapters
{
    public interface IUpdatable
    {
        IDictionary<string, object> getBundle();
    }
}
