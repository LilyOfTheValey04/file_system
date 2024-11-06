using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem.MyCommand
{
   /* public class RmCommand : ICommand
    {

        private MyContainer container;
        private MetadataManager metadataManager;
        private string fileName;

        public RmCommand(MyContainer container, MetadataManager metadataManager, string fileName)
        {
            this.container = container;
            this.metadataManager = metadataManager;
            this.fileName = fileName;
        }

       /* public void Execute()
        {
            using (var fileStream = container.OpenContainer(FileMode.Open))
            {
                metadataManager.MetadataReader(fileStream, CalculateMetadataOffset());
                if (metadataManager.FileName == fileName)
                {
                    // Логика за изтриване на файла и маркиране на блоковете като свободни
                    metadataManager = null;
                    Console.WriteLine($"{fileName} was deleted successfully.");
                }
                else
                {
                    Console.WriteLine("File not found.");
                }
            }
        }

        private long CalculateMetadataOffset()
        {
            // Изчисляване на позицията на метаданните за търсене
            return 0; // Примерно, тук трябва да има реална логика
        }
    }*/
}
