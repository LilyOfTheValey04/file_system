using MyFileSustem.CusLinkedList;
using System;
using System.IO;


namespace MyFileSustem.MyCommand
{
    public class CpoutCommand : ICommand
    {
        private MyContainer container;
        private MetadataManager metadataManager;
        private Metadata fileMetadata;
        private FileBlockManager fileBlockManager;
        private MyBitMap bitmap;
        private string containerFileName;
        private string destinationPath;

        public CpoutCommand(MyContainer container, MetadataManager metadataManager, FileBlockManager fileBlockManager, string containerFileName, string destinationPath)
        {
            this.container = container;
            this.metadataManager = metadataManager;
            this.fileBlockManager = fileBlockManager;
            this.containerFileName = containerFileName;
            this.destinationPath = destinationPath;
        }

        public void Execute()
        {

            FileStream containerStream = container.GetContainerStream();
            try
            {
                fileMetadata = FindMetadataForFile(containerStream, containerFileName);
                if (fileMetadata == null)
                {
                    Console.WriteLine("File not found");
                    return;
                }

                FileStream outputStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);
                MyLinkedList<int> fileBlocks = BuildBlockLinkedList(containerStream, fileMetadata.BlockPosition);

                byte[] buffer = new byte[container.BlockSize];
                int remainingBytes = fileMetadata.FileSize;

                foreach (int blockIndex in fileBlocks)
                {
                    long blockOffset = container.DataOffset + blockIndex * container.BlockSize;
                    containerStream.Seek(blockOffset, SeekOrigin.Begin);

                    int bytesToRead = Math.Min(container.BlockSize, remainingBytes);
                    containerStream.Read(buffer, 0, bytesToRead);
                    outputStream.Write(buffer, 0, bytesToRead);

                    remainingBytes -= bytesToRead;
                    if (remainingBytes <= 0) break;
                }

                Console.WriteLine($"File '{containerFileName}' successfully copied to '{destinationPath}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during cpout execution: {ex.Message}");
                throw;
            }
        }


        public void Undo()
        {
            // Тази команда не изисква Undo, защото не променя контейнера.
            Console.WriteLine("Undo operation is not applicable for CpoutCommand.");
        }
        // Помощен метод за намиране на метаданните на файла
        private Metadata FindMetadataForFile(FileStream containerStream, string fileName)
        {
            //long metadataOffset = fileMetadata.MetadataOffset; problem null
            long metadataOffset = container.MetadataOffset;
            Console.WriteLine($"Searching for file: {fileName}");
            for (int i = 0; i < container.BlockCount; i++)
            {
                long currentOffset = metadataOffset + i * Metadata.MetadataSize;
                Metadata metadata = metadataManager.ReadMetadata(containerStream, currentOffset);
                // Логване на всеки опит за намиране на метаданни
                // Console.WriteLine($"Checking metadata at offset: {metadataOffset + i * Metadata.MetadataSize}");
                if (metadata != null && metadata.FileName == fileName)
                {
                    Console.WriteLine("Metadata found for file: " + fileName);
                    return metadata;
                }
            }
            return null;
        }
        /*  private Metadata FindMetadataForFile(FileStream containerStream, string fileName)
          {
              long metadataOffset = container.MetadataOffset; // Начало на метаданните
              Console.WriteLine($"Searching for file: {fileName}");

              for (int i = 0; i < container.BlockCount; i++)
              {
                  // Изчисляване на текущия offset за четене
                    long currentOffset = metadataOffset + i * Metadata.MetadataSize;

                  Metadata metadata = metadataManager.ReadMetadata(containerStream, currentOffset);

                  // Проверка на валидността на метаданните
                  if (metadata == null || string.IsNullOrWhiteSpace(metadata.FileName))
                  {
                      continue; // Пропуснете празните записи
                  }

                  Console.WriteLine($"Checking metadata at offset: {currentOffset}");
                  if (string.Equals(metadata.FileName.Trim(), fileName.Trim(), StringComparison.OrdinalIgnoreCase))
                  {
                      Console.WriteLine("Metadata found for file: " + fileName);
                      return metadata;
                  }
              }

              Console.WriteLine("Metadata not found for file: " + fileName);
              return null; // Ако не е намерен файлът
          }*/


        /// <summary>
        /// Създава свързан списък от блоковете на файла, започвайки от началния блок.
        /// </summary>
        private MyLinkedList<int> BuildBlockLinkedList(FileStream stream, int startBlock)
        {
            MyLinkedList<int> blockList = new MyLinkedList<int>();
            int currentBlock = startBlock;

            while (currentBlock != -1)
            {
                blockList.AddLast(currentBlock);// Добавяме текущия блок в списъка

                // Четем следващия блок от текущия блок
                currentBlock = fileBlockManager.GetNextBlock(stream, currentBlock);
            }
            return blockList;
        }
    }
}