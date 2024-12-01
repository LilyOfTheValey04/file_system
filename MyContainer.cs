using MyFileSustem.CusLinkedList;
using System;
using System.IO;

namespace MyFileSustem
{
    public class MyContainer
    {

        //Този клас създава и управлява контейнера
        // Константи за размера и броя на блоковете по подразбиране

        public const int DefaultBlockSize = 1024;
        public const int DefaultBlockCount = 1024;

        public int BlockSize { get; private set; }
        public int BlockCount { get; private set; }

        public string ContainerFileAddress { get; private set; }
        public int MetadataRegionSize { get; private set; }

        public MyBitMap _bitmap;
        private readonly MetadataManager _metadataManager;

        private FileStream stream;
        private int metadataRegionSize;


        public MyContainer(string containerFileAddress, int maxFile = 1024)
        {
            ContainerFileAddress = containerFileAddress;
            BlockSize = DefaultBlockSize;
            BlockCount = DefaultBlockCount;
            _bitmap = new MyBitMap(BlockCount);
            // Определяме размер за метаданни за `maxFiles`
            metadataRegionSize = maxFile * Metadata.MetadataSize;
        }

        // Определя позицията на битмапа
        public long BitmapOffset => 0;

        // Определя позицията на метаданите след битмапа
        public long MetadataOffset => BitmapOffset + (_bitmap.Size / 8);

        // Определя позицията на блоковете със съдържание след метаданите
        public long DataOffset => MetadataOffset + metadataRegionSize;

        public void CreateContainer()
        {
            stream = new FileStream(ContainerFileAddress, FileMode.Create, FileAccess.Write);

            _bitmap.Serialize(stream); // Записваме битмапа

            // Преместваме позицията на стрийма до началото на областта за метаданни и запълваме с нули
            stream.Seek(MetadataOffset, SeekOrigin.Begin);
            stream.Write(new byte[metadataRegionSize], 0, metadataRegionSize);

            // Записваме празни блокове след метаданите
            stream.Seek(DataOffset, SeekOrigin.Begin);
            byte[] emptyBlock = new byte[BlockSize];
            for (int i = 0; i < BlockCount; i++)
            {
                stream.Write(emptyBlock, 0, emptyBlock.Length); // Записваме празния блок
            }
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
            stream = new FileStream(ContainerFileAddress, FileMode.Open, FileAccess.ReadWrite);
            stream.Seek(BitmapOffset, SeekOrigin.Begin);
            _bitmap.Deserialize(stream);
        }

        public void CloseContainerStream()
        {
            stream?.Close();
        }

        public FileStream GetContainerStream()
        {
            if (stream == null)
            {
                throw new Exception("the container stream is not open");
            }
            return stream;
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

        public void AddFile(string sourceFilePath, string containerFileName)
        {
            // Прочетете съдържанието на файла от даденото местоположение
            byte[] fileData = File.ReadAllBytes(sourceFilePath);
            int fileSize = fileData.Length;

            // Изчисляваме броя на блоковете, които ще ни трябват за файла
            int requiredBlocks = (int)Math.Ceiling((double)fileSize / BlockSize);

            // Проверка дали има достатъчно свободни блокове
            if (_bitmap.CountFreeBlocks() < requiredBlocks)
            {
                throw new InvalidOperationException("No enough blocks in the container to store the file");
            }

            // Намиране на свободни блокове
            int[] allocatedBlocks = new int[requiredBlocks];
            for (int i = 0; i < requiredBlocks; i++)
            {
                allocatedBlocks[i] = FindAndMarkFreeBlock();
            }

            // Записване на файла в контейнера
            using (FileStream containerStream = new FileStream(containerFileName, FileMode.Open, FileAccess.Write))
            {
                for (int i = 0; i < requiredBlocks; i++)
                {
                    int blockIndex = allocatedBlocks[i];
                    long blockOffset = DataOffset + blockIndex * BlockSize;

                    //Изчисляваме дължината на данните, които трябва да бъдат записани в текущия блок
                    int bytesToWrite = Math.Min(BlockSize, fileSize - i * BlockSize);
                    containerStream.Seek(blockOffset, SeekOrigin.Begin);
                    containerStream.Write(fileData, i * BlockSize, bytesToWrite);
                }

                // Записване на метаданни за файла
                int metadataCount = _metadataManager.GetTotalMetadataCount(containerStream, MetadataOffset, metadataRegionSize);
                long metadataOffSet = MetadataOffset + metadataCount * Metadata.MetadataSize;
                Metadata fileMetadata = new Metadata(
            fileName: containerFileName,
            fileLocation: ContainerFileAddress,
            fileDateTime: DateTime.Now,
            fileSize: fileSize,
            metadataOffset: metadataOffSet,
            blockPosition: allocatedBlocks[0]
            );

                _metadataManager.MetadataWriter(containerStream, fileMetadata);
            }
        }
    }
}
