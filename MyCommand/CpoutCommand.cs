using MyFileSustem.CusLinkedList;
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
            FileStream containerStream = container.GetContainerStream();

            try {
                // Четем метаданните на файла от контейнера
                fileMetadata = FindMetadataForFile(containerStream, containerFileName);
                if (fileMetadata == null)
                {
                    Console.WriteLine("File not found");
                    return;
                }

                // Създаваме поток за запис на изходния файл
                using (FileStream outputStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                {
                    MyLinkedList<int> fileBlocks= new MyLinkedList<int>();
                    // Изчисляваме броя на блоковете, необходими за файла
                    int totalBlocks = (fileMetadata.FileSize + container.BlockSize - 1) / container.BlockSize;
                    int currentBlock = fileMetadata.BlockPosition;

                    byte[] buffer = new byte[container.BlockSize];
                    int remainingBytes = fileMetadata.FileSize;

                    MyLinkedListNode<int> currentNode = fileBlocks.Find(fileBlocks.GetFirst()); 

                    // Четем и записваме съдържанието на файла блок по блок
                    while(currentNode!=null) 
                    {
                        int bytesToRead = Math.Min(container.BlockSize, remainingBytes);

                        // Четем блока от контейнера
                        long blockOffset = container.DataOffset + currentBlock * container.BlockSize;
                        containerStream.Seek(blockOffset, SeekOrigin.Begin );
                        containerStream.Read( buffer, 0, bytesToRead );

                        // Записваме блока в изходния файл
                        outputStream.Write(buffer, 0, bytesToRead );
                        remainingBytes-= bytesToRead;

                        // Преминаваме към следващия блок
                        currentNode = currentNode.Next;

                        /* Определяме следващия блок, ако има остатъчни данни за четене
                        if (remainingBytes > 0)
                        {
                            currentBlock = container.FindAndMarkFreeBlock();
                            if (currentBlock==-1)
                            {
                                throw new Exception("The blocks of the file are corruped or incompleted");
                            }
                        }*/
                    }
                }
                Console.WriteLine($"File {containerFileName} successfully copied to {destinationPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excetion with executing  cpout commnad: {ex.Message}");
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
            long metadataOffset = 0;
                Console.WriteLine($"Searching for file: {fileName}");
            for (int i = 0; i < container.BlockCount; i++)
            {
                Metadata metadata = metadataManager.MetadataReader(containerStream, metadataOffset + i * Metadata.MetadataSize);
                // Логване на всеки опит за намиране на метаданни
                // Console.WriteLine($"Checking metadata at offset: {metadataOffset + i * Metadata.MetadataSize}");
                if (metadata != null&&metadata.FileName==fileName) 
                {
                    Console.WriteLine("Metadata found for file: " + fileName);
                    return metadata;
                }
            }
            return null;
        }

        /// <summary>
        /// Създава свързан списък от блоковете на файла, започвайки от началния блок.
        /// </summary>
        private MyLinkedList<int> BuildBlockLinkedList(FileStream stream, int startBlock)
        {
            MyLinkedList<int> blockList = new MyLinkedList<int>();
            int currentBlock = startBlock;

            while (currentBlock!=-1) 
            {
                blockList.AddLast(currentBlock);// Добавяме текущия блок в списъка

                // Четем следващия блок от текущия блок
                currentBlock= fileBlockManager.GetNextBlock(stream,currentBlock);
            }
            return blockList;
        }
    }
}