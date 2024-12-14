using System;
using System.ComponentModel;
using System.IO;

namespace MyFileSustem
{
    public class FileBlockManager
    {
        //Този клас управлява блоковете от данни
        public MyContainer Container { get; set; }
       

        public FileBlockManager(MyContainer container)
        {
            Container = container;
            
        }

        // Запис на съдържание във файл
        public void WriteBlock(FileStream container, byte[] date, int startBlock, int blockSize)
        {
            long blockOffSet = startBlock * blockSize;//Това изчислява началната позиция в байтове (офсета), на която записът ще започне в контейнера
            container.Seek(blockOffSet, SeekOrigin.Begin);

            container.Write(date, 0, date.Length);
        }

        //Четене на съдържанието на даден блок
        public byte[] ReadBlock(FileStream container, int size, int startBlock, int blockSize)
        {
            long blockOffSet = startBlock * blockSize;
            container.Seek(blockOffSet, SeekOrigin.Begin);

            byte[] buffer = new byte[size];
            container.Read(buffer, 0, size);

            return buffer;
        }

        public int GetNextBlock(FileStream stream, int currentBlock)
        {
            if (Container == null)
            {
                throw new InvalidOperationException("Container is not initialized in FileBlockManager.");
            }
            //long nextBlockCurrentOffset = Container.DataOffset + currentBlock * Container.FileBlockSize;
            long nextBlockCurrentOffset = Container.DataOffset + currentBlock * Container.FileBlockSize + Container.FileBlockSize - sizeof(int);
            byte[] buffer = new byte[sizeof(int)];
            stream.Seek(nextBlockCurrentOffset, SeekOrigin.Begin);
            stream.Read(buffer, 0, buffer.Length);

            return BitConverter.ToInt32(buffer, 0);// Връща следващия блок
        }

        public void WriteFileToDirectory(FileStream container, byte[] data, int startBlock, int blockSize, Metadata metadataDirectory)
        {
            // Увери се, че директорията е валидна
            if (metadataDirectory == null || metadataDirectory.Type != MetadataType.Directory)
            {
                throw new InvalidOperationException("Invalid directory metadata provided.");
            }

            // Запиши файла
            WriteBlock(container, data, startBlock, blockSize);

            // Актуализирай метаданните на директорията
            metadataDirectory.BlocksPositionsList.AddLast(startBlock);
            metadataDirectory.Size += data.Length;
        }

        public byte[] ReadFileFromDirectory(FileStream container, Metadata fileMetadata)
        {
            if (fileMetadata == null || fileMetadata.Type != MetadataType.File)
            {
                throw new InvalidOperationException("Invalid file metadata provided");
            }

            byte[] fileData = new byte[fileMetadata.Size];
            int bytesRead = 0;
            foreach (int blockIndex in fileMetadata.BlocksPositionsList)
            {
                int bytesToRead = Math.Min(Container.FileBlockSize, fileMetadata.Size - bytesRead);
                byte[] blockData = ReadBlock(container, bytesToRead, blockIndex, Container.FileBlockSize);
                Array.Copy(blockData, 0, fileData, bytesRead, bytesToRead);
                bytesRead += bytesToRead;
            }
            return fileData;
        }

        //public void DeleteFile(FileStream container,Metadata fileMetadata)
        //{
        //    //ask
        //    if (fileMetadata==null || fileMetadata.Type!=MetadataType.File)
        //    {
        //        throw new InvalidOperationException("Invalid file metadata provided");
        //    }

        //    foreach (int  blockIndex in fileMetadata.BlocksPositionsList)
        //    {
        //        Container.ReleaseBlock(blockIndex); // Маркира блоковете като свободни в битмапа
        //    }

        //    // Изчисти информацията за блоковете от метаданните
        //    fileMetadata.BlocksPositionsList.Clear();
        //    fileMetadata.Size = 0;
        //}

        public void ValidateBlockIndex(int blockIndex)
        {
            //Включи тези проверки в методите WriteBlock, ReadBlock, и GetNextBlock.
            if (blockIndex < 0 || blockIndex >= Container.MetadataBlockCount)
            {
                throw new ArgumentOutOfRangeException($"Block index {blockIndex} is out of range");
            }
        }

        public void ClearFileBlocks(FileStream containerStream, Metadata fileMetadata)
        {
           
            foreach (int blockNumber in fileMetadata.BlocksPositionsList)
            {
                long blockOffset = Container.DataOffset + blockNumber * Container.FileBlockSize;
                containerStream.Seek(blockOffset, SeekOrigin.Begin);

                byte[] emptyBlock = new byte[Container.FileBlockSize];
                containerStream.Write(emptyBlock, 0, emptyBlock.Length);

                Container.ReleaseBlock(blockNumber);
            }

        }
    }
}
