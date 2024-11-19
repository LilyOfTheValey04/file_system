using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
            using (FileStream containerStream = new FileStream(container.ContainerFileAddress,FileMode.Open,FileAccess.Read))
            {
                // Четем метаданните на файла от контейнера
                fileMetadata= FindMetadataForFile(containerStream,containerFileName);
                if (fileMetadata == null)
                {
                    Console.WriteLine("File not found");
                    return;
                }

                // Създаваме поток за запис на изходния файл
                using (FileStream outputStream= new FileStream(destinationPath,FileMode.Create,FileAccess.Write))
                {
                    // Изчисляваме броя на блоковете, необходими за файла
                    int totalBlocks=(fileMetadata.FileSize+container.BlockSize-1)/container.BlockSize;
                    int currentBlock = fileMetadata.BlockPosition;

                    byte[] buffer = new byte[container.BlockSize];
                    int remainingBytes = fileMetadata.FileSize;

                    // Четем и записваме съдържанието на файла блок по блок
                    for (int i = 0; i < totalBlocks; i++)
                    {
                        int bytesToRead = Math.Min(container.BlockSize, remainingBytes);

                        // Четем блока от контейнера
                        byte[] fileData = fileBlockManager.ReadBlock(containerStream,bytesToRead,currentBlock,container.BlockSize);
                        outputStream.Write(fileData, 0, bytesToRead);

                        remainingBytes -= bytesToRead;

                        // Определяме следващия блок, ако има остатъчни данни за четене
                        if (remainingBytes>0)
                        {
                            currentBlock = container.FindAndMarkFreeBlock();
                        }
                    }
                }
                Console.WriteLine($"File {containerFileName} successfully copied to {destinationPath}");
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
            long metadataOffset = fileMetadata.MetadataOffset;
            for (int i = 0; i < container.BlockCount; i++)
            {
                Metadata metadata=metadataManager.MetadataReader(containerStream,metadataOffset+i*Metadata.MetadataSize);
                if (metadata != null&&metadata.FileName==fileName) 
                {
                    return metadata;
                }
            }
            return null;
        }
    }
}