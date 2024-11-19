using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            using (FileStream containerStream = new FileStream(container.ContainerFileAddress, FileMode.Open, FileAccess.Write))
            using (FileStream sourceFileStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read)) // Adjusted to read from sourcePath
            {
                // Get file size
                int fileSize = (int)sourceFileStream.Length;
                int startBlock = container.FindAndMarkFreeBlock();
                int currentBlock = startBlock;

                // Write the file in chunks to the container
                byte[] buffer = new byte[container.BlockSize];
                int bytesRead;

                while ((bytesRead = sourceFileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileBlockManager.WriteBlock(containerStream, buffer, currentBlock, container.BlockSize);
                    currentBlock = container.FindAndMarkFreeBlock();
                }

                // Запис на метаданните след битмапа и преди съдържанието
                long metadataPosition = container.MetadataOffset;

                // Initialize metadata with constructor and write to container
                metadata = new Metadata(
                    fileName: containerFileName,
                    fileLocation: sourcePath,
                    fileDateTime: DateTime.Now,
                    fileSize: fileSize,
                    metadataOffset: metadataPosition,
                    blockPosition: startBlock
                );

                metadataManager.MetadataWriter(containerStream,metadata);
            }
        }



        public void Undo()
        {
            if (metadataManager!=null)
            {
                container.ReleaseBlock(metadata.BlockPosition);
            }
        }
    }
    }

