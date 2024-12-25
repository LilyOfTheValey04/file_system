using MyFileSustem.CusLinkedList;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyFileSustem
{
    //Този клас управлява информацията за файловете и папките 
    public class MetadataManager
    {
        private MyContainer myContainer;

        public MetadataManager(MyContainer myContainer)
        {
            this.myContainer = myContainer;
        }

        // Запис на метаданни във файловата система
        public void MetadataWriter(FileStream container, Metadata metadata)
        {
            BinaryWriter binaryWriter = new BinaryWriter(container);

            container.Seek(metadata.Offset, SeekOrigin.Begin);
            // Логване на стойностите преди записване
            Console.WriteLine($"Writing metadata for file: {metadata.Name},Type:{metadata.Type}");
            Console.WriteLine($"File size: {metadata.Size}, Location: {metadata.Location}");

            binaryWriter.Write(metadata.Name);
            binaryWriter.Write((int)metadata.Type);
            binaryWriter.Write(metadata.Location);
            binaryWriter.Write(metadata.DateOfCreation.Ticks);// convert DateTime to long(Ticks) and writes
            binaryWriter.Write(metadata.Size);
            // Запис на списъка с блокове
           
                binaryWriter.Write(metadata.BlocksPositionsList.Count); // Записваме броя блокове
                foreach (var position in metadata.BlocksPositionsList)
                {
                    binaryWriter.Write(position);
                }
        }

        // Четене на метаданни от контейнера
        public Metadata ReadMetadata(FileStream container, long offset)
        {
            BinaryReader reader = new BinaryReader(container, System.Text.Encoding.Default, leaveOpen: true);

            try
            {
                container.Seek(offset, SeekOrigin.Begin);
                string fileName = reader.ReadString();
                MetadataType type = (MetadataType)reader.ReadInt32();
                if (Utilities.IsItNullorWhiteSpace(fileName))
                {

                    return null; // Пропускаме невалидните записи
                }

                string fileLocation = reader.ReadString();
                long dateTimeTicks = reader.ReadInt64();
                DateTime fileDateTime = new DateTime(dateTimeTicks);
                int fileSize = reader.ReadInt32();

                if (type == MetadataType.File)
                {

                    // Четене на списъка с блокове
                    int blockCount = reader.ReadInt32();
                    MyLinkedList<int> blockPositions = new MyLinkedList<int>();
                    for (int i = 0; i < blockCount; i++)
                    {
                        blockPositions.AddLast(reader.ReadInt32());
                    }
                    return new Metadata(fileName, fileLocation, type, fileDateTime, fileSize, offset, blockPositions);
                }
                else if (type == MetadataType.Directory)
                {
                    int blockCount = reader.ReadInt32(); // Четем броя на блоковете
                    MyLinkedList<int> blockPositions = new MyLinkedList<int>();
                    for (int i = 0; i < blockCount; i++)
                    {
                        blockPositions.AddLast(reader.ReadInt32()); // Добавяме номерата на блоковете
                    }
                    return new Metadata(fileName, fileLocation, type, fileDateTime, 0, offset, blockPositions);
                }
               
                return null;
            }
            catch (EndOfStreamException)
            {
                // Ако достигнем края на потока
                Console.WriteLine($"Debug: Reached end of containerStream while reading metadata at offset {offset}.");
                return null;
            }
            catch (IOException ex)
            {
                // За I/O грешки като неуспешен достъп до файла
                Console.WriteLine($"Error reading metadata at offset {offset}: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                // За всякакви други неочаквани грешки
                Console.WriteLine($"Unexpected error reading metadata at offset {offset}: {ex.Message}");
                return null;
            }
            finally
            {
                // Ръчно освобождаваме BinaryReader
                reader.Dispose();
            }

        }

        //този метод е за редакция
        public int GetTotalMetadataCount(FileStream container, long metadataOffset, int metadataRegionSize)
        {
            //metadataRegionSize-Максималният размер (в байтове) на региона, запазен за метаданните.
            int count = 0;
            long currentOffset = metadataOffset;

            //Продължава, докато текущата позиция не достигне края на региона. Това предотвратява излизането извън зададените граници на метаданните.
            while (currentOffset < myContainer.MetadataBlockCount * metadataOffset)
            {
                try
                {
                    container.Seek(currentOffset, SeekOrigin.Begin);
                    using (BinaryReader reader = new BinaryReader(container, System.Text.Encoding.Default, true))
                    {
                        // Четем първия стринг от метаданните, за да проверим дали има валиден запис
                        string fileName = reader.ReadString();
                        if (Utilities.IsItNullorWhiteSpace(fileName))
                        {
                            Console.WriteLine($"Debug: Empty metadata at offset {currentOffset}. Stopping count.");
                            break; // Спираме, ако имаме празен запис (няма повече валидни метаданни)
                        }
                        // Увеличаваме брояча за метаданните
                        count++;
                        // Придвижваме текущия офсет с размера на една метаданна структура
                        currentOffset += Metadata.MetadataSize;

                    }
                }

                catch (EndOfStreamException)
                {
                    // Ако достигнем края на потока, излизаме от цикъла
                    Console.WriteLine($"Debug: Reached end of metadata region at offset {currentOffset}. Stopping count.");
                    break;
                }
                catch (IOException ex)
                {
                    // Ако възникне I/O грешка, изписваме съобщение и прекратяваме
                    Console.WriteLine($"Error reading metadata at offset {currentOffset}: {ex.Message}. Stopping count.");
                    break;
                }
                catch (Exception ex)
                {
                    // Обработваме всякакви други грешки, но прекратяваме, за да избегнем некоректно четене
                    Console.WriteLine($"Unexpected error reading metadata at offset {currentOffset}: {ex.Message}. Stopping count.");
                    break;
                }
            }

            return count; // Връщаме броя на валидните метаданни

        }

        public Metadata FindDirectoryMetadata(FileStream container, string directoryPath)
        {

            long currentOffset = myContainer.MetadataOffset;
            long metadataRegionEnd = currentOffset + myContainer.MetadataBlockCount * Metadata.MetadataSize;
            while (currentOffset < metadataRegionEnd)
            {
                container.Seek(currentOffset,SeekOrigin.Begin);
                Metadata metadata = ReadMetadata(container,currentOffset);
                if (metadata==null)
                {
                    currentOffset += Metadata.MetadataSize;
                    continue;
                }
                //  if (metadata.Name == directoryPath && metadata.Type == MetadataType.Directory) 

                string directoryName = directoryPath;
                if (!directoryPath.Equals("/"))
                {
                    directoryName = Path.GetFileName(directoryPath);
                }

                //if (string.Equals(metadata.Name, directoryPath, StringComparison.OrdinalIgnoreCase) && metadata.Type == MetadataType.Directory)
                if (string.Equals(metadata.Name, directoryName, StringComparison.OrdinalIgnoreCase) && metadata.Type == MetadataType.Directory)
                {
                    return metadata;
                }
                currentOffset += Metadata.MetadataSize;
            }
            return null;
        }

        public void createRootDir()
        {
            Metadata root = FindDirectoryMetadata(myContainer.GetContainerStream(), "/");
            if (root == null)
            {
                // Създаване на коренна директория
                long rootMetadataOfSet = 128;
                Metadata rootDirectory = new Metadata(
                                            Name: "/",
                                            Location: "/",
                                            Type: MetadataType.Directory,
                                            DateOfCreation: DateTime.Now,
                                            Size: 0,
                                            MetadataOffset: rootMetadataOfSet,
                                            BlocksPositionsList: new MyLinkedList<int>()
                                             );
                MetadataWriter(myContainer.GetContainerStream(), rootDirectory);
            }
        }

        /* public MyLinkedList<Metadata> GetDirectoryContent (FileStream containerStream, Metadata directoryMetadata)
         {
             MyLinkedList<Metadata> content = new MyLinkedList<Metadata>();

             long currentOffset = myContainer.MetadataOffset;
             long metadataRegionEnd = currentOffset + myContainer.MetadataBlockCount * Metadata.MetadataSize;

             while(currentOffset<metadataRegionEnd)
             {
                 containerStream.Seek(currentOffset,SeekOrigin.Begin);
                 Metadata metadata = ReadMetadata(containerStream,currentOffset);

                 if (metadata==null)
                 {
                     currentOffset += Metadata.MetadataSize;
                     continue;
                 }

                 // Проверяваме дали метаданните принадлежат на текущата директория
                 if (metadata.Location==directoryMetadata.Name)
                 {
                     content.AddLast(metadata);
                 }

                 currentOffset += Metadata.MetadataSize;
             }
             return content;
         }*/
        public List<Metadata> GetDirectoryContent(FileStream container, Metadata directoryMetadata)
        {
            if (directoryMetadata.Type != MetadataType.Directory)
                throw new ArgumentException("Provided metadata is not a directory.");

            List<Metadata> content = new List<Metadata>();

            foreach (int blockPosition in directoryMetadata.BlocksPositionsList)
            {
                Metadata fileMetadata = ReadMetadata(container, blockPosition);
                if (fileMetadata != null)
                {
                    content.Add(fileMetadata);
                }
                else
                {
                    Console.WriteLine($"Warning: Unable to read metadata at block position {blockPosition}");
                }
            }

            return content;
        }
        public void UpdateMetadata(FileStream containerStream, Metadata metadata)
        {
            if (metadata.BlocksPositionsList.Count > 1)
            {
                throw new InvalidOperationException("Metadata exceeds the allowed size and spans multiple blocks.");
            }

            // Проверка на офсета
            if (metadata.Offset < myContainer.MetadataOffset || metadata.Offset >= myContainer.MetadataOffset + myContainer.MetadataRegionSize)
            {
                throw new Exception("Invalid metadata offset.");
            }

            // Използване на MetadataWriter за актуализация
            MetadataWriter(containerStream, metadata);
        }

        public Metadata GetMetadataByLocation(FileStream containerStream, string location)
{
    long currentOffset = myContainer.MetadataOffset;

    for (int i = 0; i <  myContainer.MetadataRegionSize / Metadata.MetadataSize; i++)
    {
        // Използване на ReadMetadata за четене на данни
        Metadata metadata = ReadMetadata(containerStream, currentOffset);

        if (metadata != null && metadata.Location == location && metadata.Type == MetadataType.Directory)
        {
            return metadata;
        }

        currentOffset += Metadata.MetadataSize;
    }

    throw new Exception($"No metadata found for location: {location}");
}




        // Метод за изчистване на метаданните (зануляване) на даден файл
        public void ClearMetadata(FileStream containerStream, long offset)
        {
            byte[] emptyMetadata = new byte[Metadata.MetadataSize];
            containerStream.Seek(offset, SeekOrigin.Begin);
            containerStream.Write(emptyMetadata, 0, emptyMetadata.Length);
        }

        public long GetNextAvalibleMetadataOfset(FileStream fileStream, long metadataOffset, int metadataCount)
        {
            // Преглежда всяко място в областта за метаданни
            for (int i = 0; i < metadataCount; i++)
            {
                long offset =metadataOffset+ i* Metadata.MetadataSize;

                // Прочети метаданните на текущата позиция
                Metadata metadata = ReadMetadata(fileStream,offset);
                if (metadata == null ||Utilities.IsItNullorWhiteSpace(metadata.Name))
                {
                    return offset;
                }
            }

            // Ако всички позиции са заети, върни грешка или следващата налична позиция
            throw new Exception("Not avalible space for the new matadata");
        }

        public void AddFileToDirectory(Metadata directoryMetadata, Metadata fileMetadata)
        {
            if (directoryMetadata == null || directoryMetadata.Type != MetadataType.Directory)
                throw new InvalidOperationException("Invalid directory metadata.");

            directoryMetadata.BlocksPositionsList.AddLast((int)(fileMetadata.Offset / Metadata.MetadataSize));
        }



    }
}

