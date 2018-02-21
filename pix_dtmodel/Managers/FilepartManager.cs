using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pix_dtmodel.Models;

namespace pix_dtmodel.Managers
{
    public abstract class FilepartManager
    {
        private static Dictionary<string, OutgoingPartedFile> uploads = new Dictionary<string, OutgoingPartedFile>();

        /// <summary>
        /// Add Filepart to the manager to support resumable upload
        /// </summary>
        /// <param name="id">Id Associated with multipart file.</param>
        /// <param name="filePart">Multipart </param>
        public static void RegisterUpload(string id,ref OutgoingPartedFile filePart)
        {
            uploads.Add(id, filePart);
        }
    }
}
