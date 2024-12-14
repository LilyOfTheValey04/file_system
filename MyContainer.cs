using MyFileSustem.CusLinkedList;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace MyFileSustem
{
    public class MyContainer
    {
        //Този клас създава и управлява контейнера
        // Константи за размера и броя на блоковете по подразбиране

        public const int DefaultFileBlockSize = 1024;
        public const int DefaultMetadataCount = 1024;

        public int FileBlockSize { get; private set; }
        public int MetadataBlockCount { get; private set; }

        public string ContainerFileAddress { get; private set; }
        public int MetadataRegionSize { get; private set; }

        public MyBitMap _bitmap;
        private readonly MetadataManager _metadataManager;

        private FileStream containerStream;
        private int metadataRegionSize;
        private string currentDirectory = "/"; // Започваме от root директорията
        public string CurrentDirectory
        {
            get => currentDirectory;
            private set 
            {
                if(string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Directory path can not be null");
                }
                currentDirectory= value;
            }
        }

        public MyContainer(string containerFileAddress, int maxFilesCount = DefaultMetadataCount)
        {
            ContainerFileAddress = containerFileAddress;
            FileBlockSize = DefaultFileBlockSize;
            MetadataBlockCount = maxFilesCount;
            _bitmap = new MyBitMap(MetadataBlockCount);
            // Определяме размер за метаданни за `maxFiles`
            metadataRegionSize = maxFilesCount * Metadata.MetadataSize;
        }

        // Определя позицията на битмапа
        public long BitmapOffset => 0;

        // Определя позицията на метаданите след битмапа
        public long MetadataOffset => BitmapOffset + (_bitmap.Size / 8);

        // Определя позицията на блоковете със съдържание след метаданите
        public long DataOffset => MetadataOffset + metadataRegionSize;

        public void CreateContainer()
        {
            containerStream = new FileStream(ContainerFileAddress, FileMode.Create, FileAccess.Write);
            
                _bitmap.Serialize(containerStream); // Записваме битмапа

                // Преместваме позицията на стрийма до началото на областта за метаданни и запълваме с нули
                containerStream.Seek(MetadataOffset, SeekOrigin.Begin);
                containerStream.Write(new byte[metadataRegionSize], 0, metadataRegionSize);

                // Записваме празни блокове след метаданите
                containerStream.Seek(DataOffset, SeekOrigin.Begin);
                byte[] emptyBlock = new byte[FileBlockSize];
                for (int i = 0; i < MetadataBlockCount; i++)
                {
                    containerStream.Write(emptyBlock, 0, emptyBlock.Length); // Записваме празния блок
                }
            // Създаване на коренна директория
            long rootMetadataOfSet = MetadataOffset;
            Metadata rootDirectory = new Metadata(
                                        Name: "/",
                                        Location: "/",
                                        Type: MetadataType.Directory,
                                        DateOfCreation: DateTime.Now,
                                        Size: 0,
                                        MetadataOffset: rootMetadataOfSet,
                                        BlocksPositionsList: new MyLinkedList<int>()
                                         );
            _metadataManager.MetadataWriter(containerStream,rootDirectory);
            
        }

        public void OpenContainer(FileMode mode)
        {
            using (FileStream stream = new FileStream(ContainerFileAddress, FileMode.Open, FileAccess.ReadWrite))
            {
                stream.Seek(BitmapOffset, SeekOrigin.Begin);
                _bitmap.Deserialize(stream);//зареждане на битмата,четем съдържанието му
              
                if (_bitmap.CountFreeBlocks() == 0)
                {
                    Console.WriteLine("Debug: No free blocks, but bitmap deserialized successfully.");
                    // Consider throwing an error here or verifying the container manually.
                }
            }
        }

        public void OpenContainerStream()
        {
            containerStream= new FileStream(ContainerFileAddress,FileMode.Open,FileAccess.ReadWrite);
            containerStream.Seek(BitmapOffset, SeekOrigin.Begin);
            _bitmap.Deserialize(containerStream);
        }

        public void CloseContainerStream()
        {
            containerStream?.Close();
        }

        public FileStream GetContainerStream()
        { 
            if(containerStream==null)
            {
                throw new Exception("the container fileWriteStream is not open");
            }
            return containerStream;
        }
        // Използва се за намиране на свободен блок при запис на нов файл
        public int FindAndMarkFreeBlock()
        {
            int freeBlock = _bitmap.FindFirstFreeBlock();
            if (freeBlock != -1)
            {
                _bitmap.MarkBlockAsUsed(freeBlock);
                return freeBlock;
            }
            else
            {
                throw new InvalidOperationException("No free blocks available in the container.");
            }
        }

        //Позволява освобождаване на блок, когато файлът бъде изтрит или пренаписан
        public void ReleaseBlock(int blockIndex)
        {
            try
            {
                _bitmap.MarkBlockAsFree(blockIndex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidOperationException($"Failed to revese block at index{blockIndex}", ex);
            }
        }

        public void AddFile(string sourceFilePath, string containerFileName,string containerDirectory)
        {
            // Прочетете съдържанието на файла от даденото местоположение
            byte[] fileData = File.ReadAllBytes(sourceFilePath);
            int fileSize = fileData.Length;

            // Изчисляваме броя на блоковете, които ще ни трябват за файла
            int requiredBlocks = (int)Math.Ceiling((double)fileSize / FileBlockSize);

            // Проверка дали има достатъчно свободни блокове
            if (_bitmap.CountFreeBlocks() < requiredBlocks)
            {
                throw new InvalidOperationException("No enough blocks in the container to store the file");
            }

            // Проверка дали директорията съществува
            Metadata directoryMetadata = _metadataManager.FindDirectoryMetadata(containerStream,containerDirectory);
            if (directoryMetadata==null || directoryMetadata.Type!=MetadataType.Directory)
            {
                throw new InvalidOperationException($"Directory {containerDirectory} doent exist.");
            }

            // Намиране на свободни блокове
            MyLinkedList<int> allocatedBlocks = new MyLinkedList<int>();
            for (int i = 0; i < requiredBlocks; i++)
            {
                int freeBlock = FindAndMarkFreeBlock();
                allocatedBlocks.AddLast(freeBlock);
            }
              
            // Записване на файла в контейнера
            using (FileStream fileWriteStream = new FileStream(containerFileName, FileMode.Open, FileAccess.Write))
            {
                for (int i = 0; i < requiredBlocks; i++)
                {
                    int blockIndex = allocatedBlocks.Find(i).Data;
                    long blockOffset = DataOffset + blockIndex * FileBlockSize;

                    //Изчисляваме дължината на данните, които трябва да бъдат записани в текущия блок
                    int bytesToWrite = Math.Min(FileBlockSize, fileSize - i * FileBlockSize);
                    fileWriteStream.Seek(blockOffset, SeekOrigin.Begin);
                    fileWriteStream.Write(fileData, i * FileBlockSize, bytesToWrite);
                }

                // Записване на метаданни за файла
                int metadataCount = _metadataManager.GetTotalMetadataCount(fileWriteStream, MetadataOffset, metadataRegionSize);
                long metadataOffSet = MetadataOffset + metadataCount * Metadata.MetadataSize;
                Metadata fileMetadata = new Metadata(
                    Name: containerFileName,
                    Location: ContainerFileAddress,
                    Type:MetadataType.File,
                    DateOfCreation: DateTime.Now,
                    Size: fileSize,
                    MetadataOffset: metadataOffSet,
                    BlocksPositionsList: allocatedBlocks
                );

                _metadataManager.MetadataWriter(fileWriteStream, fileMetadata);
                Console.WriteLine($"File '{containerFileName}' added to directory '{containerDirectory}'.");
            }
        }

        

        
    }
}
