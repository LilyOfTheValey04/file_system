
namespace MyFileSustem
{
    internal class FileBlock
    {
        public int StartBlock { get; set; }
        public int BlockSize { get; set; }
        public int BlockOffset { get; set; }

        public FileBlock(int blockSize)
        {
            BlockSize = blockSize;
        }
    }
}
