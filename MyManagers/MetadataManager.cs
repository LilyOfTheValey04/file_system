using MyFileSustem.CusLinkedList;
using System;
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

            container.Seek(metadata.MetadataOffset, SeekOrigin.Begin);
            // Логване на стойностите преди записване
            Console.WriteLine($"Writing metadata for file: {metadata.FileName}");
            Console.WriteLine($"File size: {metadata.FileSize}, Location: {metadata.FileLocation}");

            binaryWriter.Write(metadata.FileName);
            binaryWriter.Write(metadata.FileLocation);
            binaryWriter.Write(metadata.FileDateTime.Ticks);// convert DateTime to long(Ticks) and writes
            binaryWriter.Write(metadata.FileSize);
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
                if (string.IsNullOrEmpty(fileName))
                {

                    return null; // Пропускаме невалидните записи
                }

                string fileLocation = reader.ReadString();
                long dateTimeTicks = reader.ReadInt64();
                DateTime fileDateTime = new DateTime(dateTimeTicks);
                int fileSize = reader.ReadInt32();

                // Четене на списъка с блокове
                int blockCount=reader.ReadInt32();
                MyLinkedList<int> blockPositions = new MyLinkedList<int>();
                for (int i = 0; i < blockCount; i++)
                {
                    blockPositions.AddLast(reader.ReadInt32());
                }
                

                return new Metadata(fileName, fileLocation, fileDateTime, fileSize, offset, blockPositions);
            }
            catch (EndOfStreamException)
            {
                // Ако достигнем края на потока
                Console.WriteLine($"Debug: Reached end of stream while reading metadata at offset {offset}.");
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
                        if (string.IsNullOrEmpty(fileName))
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
    }
}

