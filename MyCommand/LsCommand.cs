using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MyFileSustem.MyCommand
{
  /*  public class LsCommand : ICommand
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

     /*   public void Execute()
        {
            using (var fileStream = container.OpenContainer(FileMode.Open))
            {
                // Четене и извеждане на метаданните
                metadataManager.MetadataReader(fileStream, CalculateMetadataOffset());
                Console.WriteLine($"{metadataManager.FileName}\t{metadataManager.FileSize}B");
            }
        }

        private long CalculateMetadataOffset()
        {
            // Изчисляване на позицията на метаданните за извеждане
            return 0; // Примерно, тук трябва да има реална логика
        }
    }*/


}

