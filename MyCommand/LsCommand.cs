using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MyFileSustem.MyCommand
{
    public class LsCommand : ICommand
    {
        //Команда за извеждане на съдържанието на контейнера 
        //показва на потребителя имената и размерите на всички файлове и папки в текущата директория
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
                    // Вземаме потока от контейнера, без да го затваряме
                    FileStream containerStream = container.GetContainerStream();

                    long metadataOffset = container.MetadataOffset;
                    int metadataCount = container.BlockCount;

                    Console.WriteLine("Listing files in the container...");
                    bool anyFilesFound = false;

                    for (int i = 0; i < metadataCount; i++)
                    {
                        long offset = metadataOffset + i * Metadata.MetadataSize;
                        Metadata metadata = metadataManager.MetadataReader(containerStream, offset);

                        if (metadata != null && !string.IsNullOrWhiteSpace(metadata.FileName))
                        {
                            metadata.DisplayMetadata();
                            Console.WriteLine();
                            anyFilesFound = true;
                        }
                        else if (metadata != null)
                        {
                            Console.WriteLine($"Warning: Corrupted metadata found at index {i}.");
                        }
                    }

                    if (!anyFilesFound)
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
    }
}

