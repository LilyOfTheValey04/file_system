using MyFileSustem.CusLinkedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyFileSustem.MyCommand
{
    public class LsCommand : ICommand
    {
        private MyContainer container;
        private MetadataManager metadataManager;

        public LsCommand(MyContainer container, MetadataManager metadataManager)
        {
            this.container = container;
            this.metadataManager = metadataManager;
        }

        public void Execute()
        {
            try
            {
                FileStream containerStream = container.GetContainerStream();

                long metadataOffset = container.MetadataOffset;
                int metadataCount = container.MetadataBlockCount;

                // Начало на таблицата
               
                Console.WriteLine();
                Console.WriteLine("+" + new string('-', 18) + "+" + new string('-', 30) + "+" + new string('-', 15) + "+" + new string('-', 20) + "+" + new string('-', 10) + "+" + new string('-', 10) + "+" + new string('-', 20) + "+");
                Console.WriteLine("|" + PadCenter("File Name", 18) + "|" + PadCenter("Path", 30) + "|" + PadCenter("Type", 15) + "|" + PadCenter("Date/Time", 20) + "|" + PadCenter("Size", 10) + "|" + PadCenter("Offset", 10) + "|" + PadCenter("Blocks", 20) + "|");
                Console.WriteLine("+" + new string('-', 18) + "+" + new string('-', 30) + "+" + new string('-', 15) + "+" + new string('-', 20) + "+" + new string('-', 10) + "+" + new string('-', 10) + "+" + new string('-', 20) + "+");
                // bool anyFilesFound = false;
                bool anyFilesOrDirectoriesFound = false;


                for (int i = 0; i < metadataCount; i++)
                {
                    long offset = metadataOffset + i * Metadata.MetadataSize;

                    Metadata metadata = metadataManager.ReadMetadata(containerStream, offset);

                    if (metadata != null && !Utilities.IsItNullorWhiteSpace(metadata.Name)&& metadata.Location == container.CurrentDirectory)
                    {
                        anyFilesOrDirectoriesFound = true;

                        string type = metadata.Type == MetadataType.Directory ? "Directory" : "File";

                        MyLinkedList<int> blockPositions = metadata.BlocksPositionsList; // вашия свързан списък
                        int count = 0;

                        List<string> blockPositionsList = new List<string>();
                        if (metadata.Type == MetadataType.File)
                        {
                            foreach (var block in blockPositions)
                            {
                                if (count < 20)
                                {
                                    blockPositionsList.Add(block.ToString()); // Преобразувате в string и добавяте в нов списък
                                    count++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }


                        // Извеждаме информацията за файла
                      
                        Console.WriteLine("|" + PadCenter(metadata.Name, 18) + "|"
                        + PadCenter(metadata.Location, 30) + "|" // Ползва Location
                        + PadCenter(type, 15) + "|"
                        + PadCenter(metadata.DateOfCreation.ToString("yyyy-MM-dd HH:mm:ss"), 20) + "|"
                        + PadCenter(metadata.Size.ToString(), 10) + "|"
                        + PadCenter(metadata.Offset.ToString(), 10) + "|"
                        + PadCenter(type == "File" ? Utilities.MyJoin(", ", blockPositionsList) : "N/A", 20) + "|");

                        Console.WriteLine("+" + new string('-', 18) + "+" + new string('-', 30) + "+" + new string('-', 15) + "+" + new string('-', 20) + "+" + new string('-', 10) + "+" + new string('-', 10) + "+" + new string('-', 20) + "+");
                        //  + PadCenter(metadata.Type == MetadataType.File ? Utilities.MyJoin(", ", blockPositionsList) : "N/A", 20) + "|");
                        //+ PadCenter(Utilities.MyJoin(", ", blockPositionsList), 20) + "|");
                        // + PadCenter(string.Join(", ", metadata.BlocksPositionsList.Take(20)), 20) + "|");

                        // Разделител след всеки файл
                        //Console.WriteLine("+" + new string('-', 18) + "+" + new string('-', 15) + "+" + new string('-', 20) + "+" + new string('-', 10) + "+" + new string('-', 10) + "+" + new string('-', 20) + "+");
                       // Console.WriteLine("+" + new string('-', 18) + "+" + new string('-', 15) + "+" + new string('-', 15) + "+" + new string('-', 20) + "+" + new string('-', 10) + "+" + new string('-', 10) + "+" + new string('-', 20) + "+");
                    }
                }

                if (!anyFilesOrDirectoriesFound)
                {
                    Console.WriteLine("No files were found in the container.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during 'ls' operation: {ex.Message}");
            }
        }

        public void Undo()
        {
            Console.WriteLine("Undo operation is not applicable for LsCommand.");
        }

        // Utility method to center-align text within a fixed-width field
        private string PadCenter(string text, int width)
        {
            int padding = width - text.Length;
            if (padding <= 0) return text.Substring(0, width);
            int padLeft = padding / 2 + text.Length;
            // return text.PadLeft(padLeft).PadRight(width);
            return CustomPadLeftAndRight(text, width);

        }
        public static string CustomPadLeftAndRight(string text, int width, char paddingChar = ' ')
        {
            if (text == null)
            {
                text = ""; // Ако текстът е null, го заменяме с празен низ
            }

            int totalPadding = width - text.Length;

            if (totalPadding <= 0)
            {
                return text; // Ако не е необходимо да добавяме символи, връщаме текста без промяна
            }

            int leftPadding = totalPadding / 2;
            int rightPadding = totalPadding - leftPadding;

            // Добавяме padding символи вляво и вдясно
            string left = new string(paddingChar, leftPadding);
            string right = new string(paddingChar, rightPadding);

            return left + text + right;
        }

    }
}
