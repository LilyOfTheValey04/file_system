using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem
{
    public class MyContainer
    {
        //Този клас създава и управлява контейнера

        public const int DefaultBlockSize = 1024;
        public const int DefaultBlockCount = 1024;
        private const string ContainerFileAdress = "filesystem.bin";//името на файла който представлява контейнера 
        private readonly MyBitMap _bitmap;

        public int BlockSize { get; set; }
        public int BlockCount { get; set; }
        public int BlockOffset { get; set; }

        public MyContainer(int blockSize = DefaultBlockSize, int blockCount = DefaultBlockCount)
        {
            BlockSize = blockSize;
            BlockCount = blockCount;
            _bitmap= new MyBitMap(blockCount); // Инициализиране на битмапа
        }
            
        //създаваме контейнърен файл
        public void CreateContainer()
        {
            using (FileStream stream = new FileStream(ContainerFileAdress, FileMode.Create, FileAccess.Write))
            {
               // Създаваме празен битмап за свободни блокове
               // byte[] bitmap = new byte[BlockCount/8];
               // stream.Write(bitmap, 0, bitmap.Length);
               _bitmap.Serialize(stream);//запазване на битмапа

                // Запълваме останалата част с празни блокове
                byte[] EmptyBlock = new byte[BlockSize];
                for (int i = 0; i < BlockCount; i++)
                {
                    stream.Write(EmptyBlock, 0, BlockSize);
                }
            }
        }

        // Отваря контейнера за четене/писане
        /*  public FileStream OpenContainer(FileMode mode)
          {
              return new FileStream(ContainerFileAdress,mode, FileAccess.ReadWrite);
          }*/
        public void OpenContainer(FileMode mode)
        {
            using ( FileStream stream = new FileStream(ContainerFileAdress, FileMode.Open, FileAccess.ReadWrite))
            {
                _bitmap.Deserialize(stream);//зареждане на битмата,четем съдържанието му
            }
        }

        // Използва се за намиране на свободен блок при запис на нов файл
        public int FindAndMarkFreeBlock()
        {
            int freeBlock = _bitmap.FindFirstFreeBlock();
            if (freeBlock != -1)
            {
                _bitmap.MarkBlockAsUsed(freeBlock);
                return freeBlock;
            }
            else
            {
                throw new InvalidOperationException("No free blocks available in the container.");
            }
            //return freeBlock;
        }

        //Позволява освобождаване на блок, когато файлът бъде изтрит или пренаписан
        public void ReleaseBlock (int blockIndex)
        {
            try
            {
            _bitmap.MarkBlockAsFree(blockIndex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidOperationException($"Failed to revese block at index{blockIndex}:{ex.Message}");
            }
        }
    }
}
    