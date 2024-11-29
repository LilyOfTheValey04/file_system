using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyFileSustem
{
    public class MetadataManager
    {
        //Този клас управлява информацията за файловете и папките 

        // Запис на метаданни във файловата система
        public void MetadataWriter(FileStream container, Metadata metadata)
        {
            //offset represent the position in the steam where writing begins
            // using makes sure that the BinaryWriter is properly disposed of once it goes out of scope
            BinaryWriter binaryWriter = new BinaryWriter(container);
            
                container.Seek(metadata.MetadataOffset, SeekOrigin.Begin);
            // Логване на стойностите преди записване
            Console.WriteLine($"Writing metadata for file: {metadata.FileName}");
            Console.WriteLine($"File size: {metadata.FileSize}, Location: {metadata.FileLocation}");

                binaryWriter.Write(metadata.FileName);
                binaryWriter.Write(metadata.FileLocation);
                binaryWriter.Write(metadata.FileDateTime.Ticks);// convert DateTime to long(Ticks) and writes
                binaryWriter.Write(metadata.FileSize);
                binaryWriter.Write(metadata.BlockPosition);
               // binaryWriter.Flush();
        }

        // Четене на метаданни от контейнера
        public Metadata MetadataReader(FileStream container, long offset)
        {
            BinaryReader reader = new BinaryReader(container);
            container.Seek(offset, SeekOrigin.Begin);

            try
            {
                string fileName = reader.ReadString();
                if (string.IsNullOrEmpty(fileName))
                {
                    
                    return null; // Пропускаме невалидните записи
                }

                string fileLocation = reader.ReadString();
                long dateTimeTicks = reader.ReadInt64();
                DateTime fileDateTime = new DateTime(dateTimeTicks);
                int fileSize = reader.ReadInt32();
                int blockPosition = reader.ReadInt32();

                return new Metadata(fileName, fileLocation, fileDateTime, fileSize, offset, blockPosition);
            }
            catch (Exception)
            {
                // Ако срещнем грешка, връщаме null за този запис
                return null;
            }

        }

        //този метод е за редакция
       public int GetTotalMetadataCount(FileStream container,long metadataOffset,int metadataRegionSize)
        {
            //metadataRegionSize-Максималният размер (в байтове) на региона, запазен за метаданните.
            int count = 0;
            long currentoffset= metadataOffset;

       //Продължава, докато текущата позиция не достигне края на региона. Това предотвратява излизането извън зададените граници на метаданните.
            while (currentoffset < metadataRegionSize + metadataOffset)
            {
                container.Seek(currentoffset, SeekOrigin.Begin);
                using (BinaryReader reader = new BinaryReader(container,System.Text.Encoding.Default,true))
                {
                    try
                    {
                        // Четем първия стринг от метаданните, за да проверим дали има валиден запис
                        string fileName = reader.ReadString();
                        if (string.IsNullOrEmpty(fileName))
                        {
                            break; // Спираме, ако имаме празен запис (няма повече валидни метаданни)
                        }
                        // Увеличаваме брояча за метаданните
                        count++;
                        // Придвижваме текущия офсет с размера на една метаданна структура
                        currentoffset += Metadata.MetadataSize;

                    }
                    catch (EndOfStreamException)
                    {
                        break; // Ако достигнем края на потока, прекратяваме
                    }

                }
            }
            return count;
        }
    }
}

