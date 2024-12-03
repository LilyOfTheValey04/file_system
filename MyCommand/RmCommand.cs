using System;
using System.IO;


namespace MyFileSustem.MyCommand
{
    public class RmCommand : ICommand
    {
        private MyContainer container;
        private MetadataManager metadataManager;
        private FileBlockManager fileBlockManager;
        private string containerFileName;
        private Metadata fileMetadata;

        public RmCommand(MyContainer container, MetadataManager metadataManager, FileBlockManager fileBlockManager, string containerFileName)
        {
            this.container = container;
            this.metadataManager = metadataManager;
            this.fileBlockManager = fileBlockManager;
            this.containerFileName = containerFileName;
        }

        public void Execute()
        {
            // Отваряме контейнера за четене и писане
            FileStream containerStream = container.GetContainerStream();
            // Намираме метаданните за файла
            fileMetadata = FindMetadataForFile(containerStream, containerFileName);

            if (fileMetadata == null)
            {
                Console.WriteLine($"Error: File '{containerFileName}' not found in the container.");
                return;
            }

            // Определяме броя на блоковете, които заемат съдържанието на файла
            int totalBlocks = (fileMetadata.FileSize + container.FileBlockSize - 1) / container.FileBlockSize;
            int currentBlock = fileMetadata.BlocksPositionsList.GetFirst();

            // Освобождаваме блоковете, които са свързани с файла
            foreach (int blockNumber in fileMetadata.BlocksPositionsList)
            {
                container.ReleaseBlock(blockNumber);
            }

            // Изтриваме метаданните на файла
            long metadataOffset = fileMetadata.MetadataOffset;
            ClearMetadata(containerStream, metadataOffset);

            ClearFileBlocks(containerStream);

            Console.WriteLine($"File '{containerFileName}' successfully deleted from the container.");
        }

        public void Undo()
        {
            Console.WriteLine("Undo operation is not implemented for RmCommand.");
        }

        // Метод за намиране на метаданните на файла по неговото име
        private Metadata FindMetadataForFile(FileStream containerStream, string fileName)
        {
            long metadataOffset = container.MetadataOffset;

            for (int i = 0; i < container.MetadataBlockCount; i++)
            {
                Metadata metadata = metadataManager.ReadMetadata(containerStream, metadataOffset + i * Metadata.MetadataSize);

                if (metadata != null && metadata.FileName == fileName)
                {
                    return metadata;
                }
            }

            return null;
        }

        // Метод за изчистване на метаданните (зануляване) на даден файл
        private void ClearMetadata(FileStream containerStream, long offset)
        {
            byte[] emptyMetadata = new byte[Metadata.MetadataSize];
            containerStream.Seek(offset, SeekOrigin.Begin);
            containerStream.Write(emptyMetadata, 0, emptyMetadata.Length);
        }

        private void ClearFileBlocks(FileStream containerStream)
        {
            foreach (int blockNumber in fileMetadata.BlocksPositionsList)
            {
                long blockOffset = container.DataOffset + blockNumber * container.FileBlockSize;
                containerStream.Seek(blockOffset, SeekOrigin.Begin);

                byte[] emptyBlock = new byte[container.FileBlockSize];
                containerStream.Write(emptyBlock, 0, emptyBlock.Length);

                container.ReleaseBlock(blockNumber);
            }

        }
    }
}
