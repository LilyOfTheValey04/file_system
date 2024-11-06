using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem.MyCommand
{
   /* public class CpinCommand:ICommand
    {
        //Команда cpin - Копиране на файл в контейнера
        //Копира файл с име aaa.txt от път “C:\” в контейнера.
        //Името на файла в контейнера е bbb.txt. 
        private string sourcePath;
        private string destinationFileName;
        private MyContainer container;
        private MetadataManager metadataManager;
        private BlockManager blockManager;
        private MyBitMap bitmap;

        public CpinCommand(MyContainer container, MetadataManager metadataManager, BlockManager blockManager, string sourcePath, string destinationFileName, MyBitMap bitmap)
        {
            this.container = container;
            this.metadataManager = metadataManager;
            this.blockManager = blockManager;
            this.sourcePath = sourcePath;
            this.destinationFileName = destinationFileName;
            this.bitmap = bitmap;
        }

       /* public void Execute()
        {
            // Логиката за копиране на файла в контейнера
            using (var fileStream = container.OpenContainer(FileMode.Open))
            {
                // Зареждане на файла на порции и запис в контейнера
                using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
                {
                    int blockSize = container.BlockSize;
                    int blockIndex = FindFreeBlock();
                    if (blockIndex < 0)
                    {
                        Console.WriteLine("No free block available in the container.");
                        return;
                    }
                    byte[] buffer = new byte[blockSize];
                    int bytesRead;
                    int startingBlockIndex = blockIndex;
                    while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        blockManager.WriteBlock(fileStream, buffer, blockIndex, blockSize);
                        bitmap.MarkBlockAsUsed(blockIndex); // Маркирай блока като зает
                        blockIndex++;
                    }

                    // Запис на метаданните за новия файл
                    metadataManager.FileName = destinationFileName;
                    metadataManager.FileSize = (int)sourceStream.Length;
                    metadataManager.BlockPosition = startingBlockIndex;
                    metadataManager.MetadataWriter(fileStream, CalculateMetadataOffset());
                }
            }
        }

        private int FindFreeBlock()
        {
            // Метод за намиране на първия свободен блок в контейнера
           

            for (int i = 0; i < bitmap.Size; i++)
            {
                if (bitmap.IsBlockFree(i))
                {
                    return i; // Връщаме индекса на свободния блок
                }
            }
            return -1; // Няма свободни блокове
        }

      /*  private long CalculateMetadataOffset()
        {
            // Изчисляване на позицията за записа на метаданните
            // Примерно, тук трябва да има реална логика
            using (var stream = container.OpenContainer(FileMode.Open))
            {
                return stream.Length; // Връщаме текущата дължина на потока
            }
        }


    }*/


}

