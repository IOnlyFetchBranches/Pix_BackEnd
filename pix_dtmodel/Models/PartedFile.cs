using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using pix_dtmodel.Managers;
using pix_dtmodel.Util;

namespace pix_dtmodel.Models
{
    internal interface IPartedFile
    {
        long CurrentBlockPosition();
        long RemainingBlocks();
        long SentBytes();
        byte[] NextBlock();
        byte[] PreviousBlock();
    }


    public class OutgoingPartedFile :IPartedFile
    {
        
        //Store our position in sent parts
        private long pos = 0;
        //Block size
        private long blockSize;

        //Id
        private readonly string id;

        //Parted ByteList
        private readonly List<byte[]> blockList;

        
        //Will need to judge the number of divisions and provide that witht the size of the remainder block
        //It will be able to report this to a JSONfor the server to send to the client 
        /// <summary>
        /// Besure to 
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="blockposition"></param>
        /// <param name="file"></param>
        public OutgoingPartedFile(string id,byte[] file)
        {
            //Default position on blocklist
            pos = 0;

            this.id = id;


            blockList = new List<byte[]>(51); //50 equal parts and 1 remainder array
            blockSize = (long) Math.Floor((double)(file.Length / 50));

            var rembytes = new byte[file.Length % 50 + 1];

            var remstart = file.Length - file.Length % 50; //So this way if we get any left over decimal we know where it will end
            var remend = file.Length;
            //At this point we determine any margin of error, resulting in rounding down
            int remIndex = 0;
            DtLogger.LogG("Parted","File Size,Rem Size" +file.Length + " " +rembytes.Length);
            for (long startByte = remstart; startByte < remend; startByte++)
            {
                rembytes[remIndex] = file[startByte]; //Assign it to the rem array
                remIndex++;
            }
            //set eremainder
            blockList[51] = rembytes;

            for (int block = 0; block < blockList.Capacity - 1; block++)
            {
                var blockBytes = new byte[blockSize];
                for (int b = 0; b < blockBytes.Length; b++)
                {
                    blockBytes[b] = file[block * blockSize + b];
                }
                blockList[block] = blockBytes;
            }  


        }

        public long CurrentBlockPosition() => pos;

        public long RemainingBlocks() => blockList.Capacity - pos;


        public long SentBytes() => pos * blockSize;

        public byte[] NextBlock()
        {
            
            pos++;
            return blockList[(int) (pos - 1)];
        }

        public byte[] PreviousBlock()
        {
            if (pos > 0)
            {
                pos--;
                return blockList[(int) pos];
            }
            else
            {
                return blockList[0];
            }
        }
    }
}
