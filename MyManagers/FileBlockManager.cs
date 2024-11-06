using System.IO;

namespace MyFileSustem
{
    public class FileBlockManager
    {
        //Този клас управлява блоковете от данни

        // Запис на съдържание във файл
        public  void WriteBlock(FileStream container, byte[] date, int startBlock,int blockSize)
        {
            long blockOffSet = startBlock * blockSize;//Това изчислява началната позиция в байтове (офсета), на която записът ще започне в контейнера
            container.Seek(blockOffSet,SeekOrigin.Begin);

            container.Write(date,0,date.Length);
        }

        //Четене на съдържанието на даден блок
        public byte[] ReadBlock(FileStream container, int size,int startBlock,int blockSize)
        {
            long blockOffSet = startBlock * blockSize;
            container.Seek(blockOffSet,SeekOrigin.Begin);

            byte[] buffer = new byte[size];
            container.Read(buffer,0,size);

            return buffer;
        }
    }
}
