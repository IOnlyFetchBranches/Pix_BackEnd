using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using pix_dtmodel.Connectors;
using pix_dtmodel.Models;

namespace pix_dtmodel.Adapters
{
    public interface IIndexer
    {
        void MakeIndex();

    }


}
