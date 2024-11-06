using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem
{
    public class MetadataManager
    {
        //Този клас управлява информацията за файловете и папките 

        public const int MetadataSize = 512;

        public string FileName { get; set; }
        public string FileLocation {  get; set; }
        public DateTime FileDateTime { get; set; }
        public int FileSize { get; set; }

        public int BlockPosition { get; set; }
     

        // Запис на метаданни във файловата система
        public void MetadataWriter (FileStream container, long offSet)
        {
            //offset represent the position in the steam where writing begins
            // using makes sure that the BinaryWriter is properly disposed of once it goes out of scope
            using (BinaryWriter binaryWriter = new BinaryWriter(container))
            {
                container.Seek(offSet, SeekOrigin.Begin);
                binaryWriter.Write(FileName);
                binaryWriter.Write(FileLocation);
                binaryWriter.Write(FileDateTime.Ticks);// convert DateTime to long(Ticks) and writes
                binaryWriter.Write(FileSize);
                binaryWriter.Write(BlockPosition);
            }
        }

        // Четене на метаданни от контейнера
        public void MetadataReader(FileStream container, long offSet)
        {
            using (BinaryReader binaryReader = new BinaryReader(container))
            {
                container.Seek(offSet, SeekOrigin.Begin);
                FileName = binaryReader.ReadString();
                FileLocation = binaryReader.ReadString();
                long dateTimeTicks = binaryReader.ReadInt64(); // Read the long ticks and convert to DateTime
                FileDateTime = new DateTime(dateTimeTicks);
                FileSize = binaryReader.ReadInt32();
                BlockPosition = binaryReader.ReadInt32();
            }
        }

        //мога ли да използвам този метод на готово
        public bool Validate()
        { 
            if(string.IsNullOrWhiteSpace(FileName))
            {
                Console.WriteLine("Invalue name");
                return false;
            }
            if (FileSize<0)
            {
                Console.WriteLine("Invalid file size");
                return false;
            }
            return true;
        }

        public void DisplayMetadata()
        {
            Console.WriteLine($"File name:{FileName}");
            Console.WriteLine($"File location:{FileLocation}");
            Console.WriteLine($"File data and time of creation:{FileDateTime}");
            Console.WriteLine($"File size:{FileSize}");
            Console.WriteLine($"Block position:{BlockPosition}");
        }


    }
}

