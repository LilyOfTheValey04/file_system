using MyFileSustem.CusLinkedList;
using System;

namespace MyFileSustem
{
    public class Metadata
    {
        public const int MetadataSize = 512;
        public string FileName { get; set; }
        public string FileLocation { get; set; }
        public DateTime FileDateTime { get; set; }
        public int FileSize { get; set; }
        public long MetadataOffset { get; set; }
        public MyLinkedList<int> BlocksPositionsList { get; set; }

        public Metadata(string fileName, string fileLocation, DateTime fileDateTime, int fileSize, long metadataOffset, MyLinkedList<int> blocksPositionsList)
        {
            FileName = fileName;
            FileLocation = fileLocation;
            FileDateTime = fileDateTime;
            FileSize = fileSize;
            MetadataOffset = metadataOffset;
            BlocksPositionsList = blocksPositionsList;
        }

        //мога ли да използвам този метод на готово
        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(FileName))
            {
                Console.WriteLine("Invalue name");
                return false;
            }
            if (FileSize < 0)
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
            Console.Write("Block positions: [");

            foreach (var position in BlocksPositionsList)
            {
                if (position != BlocksPositionsList.tail.Data)
                {
                    Console.Write($"{position}, ");
                }
                else
                {
                    Console.Write($"{position}");
                }
            }

            Console.Write("].");
        }

        public void AddBlock(int blockPosition)
        {
            BlocksPositionsList.AddLast(blockPosition);
        }

        public void RemoveBlock(int blockPosition) 
        { 
            BlocksPositionsList.Remove(blockPosition); 
        }
    }
}
