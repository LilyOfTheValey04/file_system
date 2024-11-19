using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem
{
    internal class FileBlock
    {

       // public int BlockCount { get; set; }
        public int StartBlock { get; set; }
        public int BlockSize { get; set; }
        public int BlockOffset { get; set; }

        public FileBlock(int blockSize )
        {
            BlockSize = blockSize;
           // BlockCount = blockCount;
        }
    }
}
