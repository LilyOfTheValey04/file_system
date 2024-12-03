using System;
using System.IO;

namespace MyFileSustem
{
    public class FileBlockManager
    {
        //Този клас управлява блоковете от данни
        public MyContainer Container { get; set; }

        public FileBlockManager(MyContainer container)
        {
            Container = container;
        }

        // Запис на съдържание във файл
        public void WriteBlock(FileStream container, byte[] date, int startBlock, int blockSize)
        {
            long blockOffSet = startBlock * blockSize;//Това изчислява началната позиция в байтове (офсета), на която записът ще започне в контейнера
            container.Seek(blockOffSet, SeekOrigin.Begin);

            container.Write(date, 0, date.Length);
        }

        //Четене на съдържанието на даден блок
        public byte[] ReadBlock(FileStream container, int size, int startBlock, int blockSize)
        {
            long blockOffSet = startBlock * blockSize;
            container.Seek(blockOffSet, SeekOrigin.Begin);

            byte[] buffer = new byte[size];
            container.Read(buffer, 0, size);

            return buffer;
        }

        public int GetNextBlock(FileStream stream, int currentBlock)
        {
            if (Container == null)
            {
                throw new InvalidOperationException("Container is not initialized in FileBlockManager.");
            }
            //long nextBlockCurrentOffset = Container.DataOffset + currentBlock * Container.FileBlockSize;
            long nextBlockCurrentOffset = Container.DataOffset + currentBlock * Container.FileBlockSize + Container.FileBlockSize - sizeof(int);
            byte[] buffer = new byte[sizeof(int)];
            stream.Seek(nextBlockCurrentOffset, SeekOrigin.Begin);
            stream.Read(buffer, 0, buffer.Length);

            return BitConverter.ToInt32(buffer, 0);// Връща следващия блок
        }
    }
}
