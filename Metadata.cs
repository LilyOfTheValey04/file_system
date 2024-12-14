using MyFileSustem.CusLinkedList;
using System;

namespace MyFileSustem
{
    public class Metadata
    {
        public static int MetadataSize = 512;
        public MetadataType Type { get; set; }// Тип: файл или директория
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime DateOfCreation { get; set; }
        public int Size { get; set; }  // Размер: за файлове, 0 за директории
        public long Offset { get; set; }
        public MyLinkedList<int> BlocksPositionsList { get; set; }// За файлове, празен за директории
        public int IdParent { get; set; }// ID на родителската директория (или -1 за кореновата)



        public Metadata(string Name, string Location,MetadataType Type, DateTime DateOfCreation, int Size, long MetadataOffset, MyLinkedList<int> BlocksPositionsList)
        {
            this.Name = Name;
            this.Type= Type;    
            this.Location = Location;
            this.DateOfCreation = DateOfCreation;
            this.Size = Size;
            this.Offset = MetadataOffset;
            this.BlocksPositionsList = BlocksPositionsList;
        }

        //мога ли да използвам този метод на готово
        public bool Validate()
        {
            if (Utilities.IsItNullorWhiteSpace(Name))
            {
                Console.WriteLine("Invalue name");
                return false;
            }
            if (Type == MetadataType.File && Size < 0)
            {
                Console.WriteLine("Invalid file size");
                return false;
            }
            return true;
        }

        public void DisplayMetadata()
        {
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Type: {Type}");
            Console.WriteLine($"Location: {Location}");
            Console.WriteLine($"Creation Date: {DateOfCreation}");
            Console.WriteLine($"Size: {Size}");
            Console.WriteLine($"Parent ID: {IdParent}");
            Console.Write("Block positions: [");

            if (Type == MetadataType.File)
            {


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

        }

        // Добавяне/премахване на блок (само за файлове)
        public void AddBlock(int blockPosition)
        {
            if (Type == MetadataType.File)
            {
                BlocksPositionsList.AddLast(blockPosition);
            }
        }
        
        public void RemoveBlock(int blockPosition)
        {
            if (Type == MetadataType.File)
            {
                BlocksPositionsList.Remove(blockPosition);
            }
        }

        public bool isDirectory()
        {
            return Type == MetadataType.Directory;
        }
        public bool isFile()
        {
            return Type == MetadataType.File;
        }
    }
}
