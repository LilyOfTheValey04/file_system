using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem.MyCommand
{
  /*  public class CpoutCommand : ICommand
    {
        private readonly MyContainer container;
        private readonly string sourceFileName; // Име на файла в контейнера
        private readonly string destinationPath; // Пълен път за записа извън контейнера

        public CpoutCommand(MyContainer container, string sourceFileName, string destinationPath)
        {
            this.container = container;
            this.sourceFileName = sourceFileName;
            this.destinationPath = destinationPath;
        }

       /* public void Execute()
        {
            using (var stream = container.OpenContainer(FileMode.Open))
            {
                // Намерете метаданните за целевия файл
                var metadata = FindFileMetadata(stream, sourceFileName);
                if (metadata == null)
                {
                    Console.WriteLine($"File '{sourceFileName}' not found in the container.");
                    return;
                }

                // Създаваме изходен файл във външната файлова система
                using (var outputFile = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                {
                    int remainingBytes = metadata.FileSize;
                    int startBlock = metadata.BlockPosition;
                    byte[] buffer = new byte[container.BlockSize];

                    while (remainingBytes > 0)
                    {
                        // Определяме колко данни да прочетем в текущия блок
                        int bytesToRead = Math.Min(remainingBytes, container.BlockSize);

                        // Четем блок данни от контейнера
                        buffer = ReadBlock(stream, startBlock, bytesToRead);

                        // Записваме данните в изходния файл
                        outputFile.Write(buffer, 0, bytesToRead);

                        // Актуализираме оставащите байтове и блока
                        remainingBytes -= bytesToRead;
                        startBlock++;
                    }
                }

                Console.WriteLine($"File '{sourceFileName}' successfully copied to '{destinationPath}'.");
            }
        }

        private MetadataManager FindFileMetadata(FileStream stream, string fileName)
        {
            long offset = 0;
            while (offset < stream.Length)
            {
                var metadata = new MetadataManager();
                metadata.MetadataReader(stream, offset);

                if (metadata.FileName == fileName)
                {
                    return metadata;
                }

                offset += MetadataManager.MetadataSize;
            }

            return null;
        }

        private byte[] ReadBlock(FileStream stream, int blockIndex, int size)
        {
            byte[] buffer = new byte[size];
            long blockOffset = blockIndex * container.BlockSize;
            stream.Seek(blockOffset, SeekOrigin.Begin);
            stream.Read(buffer, 0, size);
            return buffer;
        }

    }*/
}