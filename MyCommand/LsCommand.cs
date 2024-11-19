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
            using (FileStream containerStream = new FileStream(container.ContainerFileAddress, FileMode.Open, FileAccess.Read))
            {
                long metatadaOffset = container.MetadataOffset;
                int metadataCount = container.BlockCount;
                Console.WriteLine("Listing files in the container");
                bool anyFilesFount = false;

                for (int i = 0; i < metadataCount; i++)
                {
                    long offset = metatadaOffset + i * Metadata.MetadataSize;
                    Metadata metadata = metadataManager.MetadataReader(containerStream, offset);
                    if (metadata != null && !string.IsNullOrWhiteSpace(metadata.FileName))
                    {
                        metadata.DisplayMetadata();
                        Console.WriteLine();
                        anyFilesFount = true;
                    }
                }
                if (!anyFilesFount)
                {
                    Console.WriteLine("No files were fount in the container");
                }
            }

        }

        public void Undo()
        {
            Console.WriteLine("Undo operation is not applicable for LsCommand.");
        }
    }
}

