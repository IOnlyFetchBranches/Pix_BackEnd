using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pix_Backend
{
    /*
     * This class defines Views that can be added to a Content View
     */
    public abstract class DataView 
    {
        protected string dataLabel;

        
        //Init Data
        public abstract void FillData();
    
        //Get The DataLabel, which describes the data.
        public string GetDataLabel;
    }

}
