using MyFileSustem.CusLinkedList;
using System;
using System.IO;



namespace MyFileSustem.MyCommand
{
    public class CpinCommand : ICommand
    {
        //Команда cpin - Копиране на файл в контейнера
        //Копира файл с име aaa.txt от път “C:\” в контейнера.
        //Името на файла в контейнера е bbb.txt. 
        private MyContainer container;
        private MetadataManager metadataManager;
        private Metadata metadata;
        private FileBlockManager fileBlockManager;
        private MyBitMap bitmap;
        private string sourcePath;
        private string containerFileName;

        public CpinCommand(MyContainer container, MetadataManager metadataManager, FileBlockManager fileBlockManager, string sourcePath, string containerFileName, MyBitMap bitmap)
        {
            this.container = container;
            this.metadataManager = metadataManager;
            this.fileBlockManager = fileBlockManager;
            this.sourcePath = sourcePath;
            this.containerFileName = containerFileName;
            this.bitmap = bitmap;
        }

        public void Execute()
        {

            FileStream containerStream = container.GetContainerStream(); // Получаване на потока без using
            FileStream sourceFileStream = null;

            try
            {
                sourceFileStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);

                int fileSize = (int)sourceFileStream.Length;
                int containerBlockSize = container.FileBlockSize;
                int requiredBlocks = (int)Math.Ceiling((double)fileSize / containerBlockSize);
                if (fileSize == 0)
                {
                    requiredBlocks = 1;
                }
                MyLinkedList<int> allocatedBlocks = new MyLinkedList<int>();

                // Алокация на блокове
                for (int i = 0; i < requiredBlocks; i++)
                {
                    int freeBlock = bitmap.FindFirstFreeBlock();
                    if (freeBlock == -1)
                    {
                        throw new Exception("Not enough space in the container");
                    }
                    bitmap.MarkBlockAsUsed(freeBlock);
                    allocatedBlocks.AddLast(freeBlock);
                }

                byte[] buffer = new byte[containerBlockSize];
                int bytesRead;

                // Записване на данни в блока
                foreach (int blockIndex in allocatedBlocks)
                {
                    long blockOffset = container.DataOffset + blockIndex * containerBlockSize;
                    containerStream.Seek(blockOffset, SeekOrigin.Begin);
                    bytesRead = sourceFileStream.Read(buffer, 0, containerBlockSize);
                    containerStream.Write(buffer, 0, bytesRead);
                }

                string currentDirectory = container.CurrentDirectory;
                // Записване на метаданни
                int metadataCount = metadataManager.GetTotalMetadataCount(containerStream, container.MetadataOffset, container.MetadataRegionSize);
                long metadataOffset = container.MetadataOffset + metadataCount * Metadata.MetadataSize;
                metadata = new Metadata(
                    Name: containerFileName,
                    Location: currentDirectory,
                    Type:MetadataType.File,
                    DateOfCreation: DateTime.Now,
                    Size: fileSize,
                    MetadataOffset: metadataOffset,
                    allocatedBlocks
                );

                metadataManager.MetadataWriter(containerStream, metadata);

                // Актуализиране на BlockPositionList на текущата директория
                Metadata currentDirectoryMetadata = metadataManager.GetMetadataByLocation(containerStream, currentDirectory);
                currentDirectoryMetadata.BlocksPositionsList.AddLast((int)(metadataOffset / Metadata.MetadataSize));
                metadataManager.UpdateMetadata(containerStream, currentDirectoryMetadata);

                bitmap.Serialize(containerStream);

                Console.WriteLine($"File '{containerFileName}' added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during cpin execution: {ex.Message}");
                throw;
            }
            finally
            {
                // Затваряме изходния файл, но не и контейнера
                sourceFileStream?.Close();
            }
        }

        public void Undo()
        {
            if (metadataManager != null)
            {
                foreach (int current in metadata.BlocksPositionsList)
                {
                    container.ReleaseBlock(current);
                }
            }
        }
    }
}


