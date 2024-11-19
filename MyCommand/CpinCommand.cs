using MyFileSustem.CusLinkedList;
using System;
using System.Collections.Generic;
using System.Data;
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
            FileStream containerStream = container.GetContainerStream(); // Use shared stream

            // Open the source file
            using FileStream sourceFileStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);

            // Get file size
            int fileSize = (int)sourceFileStream.Length;
            int containerBlockSize = container.BlockSize;
            int requiredBlocks = (int)Math.Ceiling((double)fileSize/ containerBlockSize);

            // Allocate blocks
            MyLinkedList<int> allocatedBlocks = new MyLinkedList<int>();
            try
            {
                for (int i = 0; i < requiredBlocks; i++)
                {
                    int freeBlock = bitmap.FindFirstFreeBlock();
                    if (freeBlock==-1)
                    {
                        throw new Exception("Not enought space in the container");
                    }
                    bitmap.MarkBlockAsFree(freeBlock);
                    allocatedBlocks.AddFirst(freeBlock);
                }
                // Write file data into the container
                byte[] buffer = new byte[containerBlockSize];
                int bytesRead=0;
                int currentBlockIndex = 0;

                foreach (int blockIndex in allocatedBlocks) 
                { 
                    long blockOffSet=container.DataOffset + blockIndex*containerBlockSize;
                    containerStream.Seek(blockOffSet, SeekOrigin.Begin );
                    bytesRead = containerStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead>0)
                    {
                        containerStream.Write(buffer,0,buffer.Length);
                    }
                    currentBlockIndex++;

                }
                // Write metadata

                // Initialize metadata with constructor and write to container
                long metadataPosition = container.MetadataOffset;


                metadata = new Metadata(
                    fileName: containerFileName,
                    fileLocation: sourcePath,
                    fileDateTime: DateTime.Now,
                    fileSize: fileSize,
                    metadataOffset: metadataPosition,
                    blockPosition: allocatedBlocks.GetFirst()
                );

                metadataManager.MetadataWriter(containerStream, metadata);

                Console.WriteLine($"File '{containerFileName}' added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during cpin execution: {ex.Message}");
                // Rollback changes
                foreach (int blockIndex in allocatedBlocks)
                {
                    bitmap.MarkBlockAsFree(blockIndex);
                }
                throw; // Rethrow to notify callers
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
    

