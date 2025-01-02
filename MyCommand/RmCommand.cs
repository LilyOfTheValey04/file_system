using System;
using System.IO;


namespace MyFileSustem.MyCommand
{
    public class RmCommand : ICommand
    {
        private MyContainer container;
        private Metadata fileMetadata;
        private MetadataManager metadataManager;
        private FileBlockManager fileBlockManager;
        private string containerFileName;

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

            try
            {
                // Намираме метаданните за файла
                fileMetadata = FindMetadataForFile(containerStream, containerFileName);

                if (fileMetadata == null)
                {
                    Console.WriteLine($"Error: File '{containerFileName}' not found in the current directory.");
                    return;
                }

                // Освобождаваме блоковете на файла
                fileBlockManager.ClearFileBlocks(containerStream, fileMetadata);

                // Изтриваме метаданните на файла
                metadataManager.ClearMetadata(containerStream, fileMetadata.Offset);

                Console.WriteLine($"File '{containerFileName}' successfully deleted from the container.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting file '{containerFileName}': {ex.Message}");
            }
           
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

                if (metadata != null && metadata.Name == fileName && metadata.Location==container.CurrentDirectory)
                {
                    return metadata;
                }
            }

            return null;
        }


    }
}

