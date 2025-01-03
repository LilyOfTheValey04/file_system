using MyFileSustem.CusLinkedList;
using MyFileSustem.MyManagers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem.MyCommand
{

    public class RdCommand : ICommand
    {
        private MyContainer container;
        private DirectoryManager directoryManager;
        private MetadataManager metadataManager;
        private FileBlockManager fileBlockManager;
        private string directoryName;

        public RdCommand(MyContainer container, DirectoryManager directoryManager, MetadataManager metadataManager, FileBlockManager fileBlockManager, string directoryName)
        {
            this.container = container;
            this.directoryManager = directoryManager;
            this.metadataManager = metadataManager;
            this.fileBlockManager = fileBlockManager;
            this.directoryName = directoryName;
        }

        public void Execute()
        {
            FileStream containerStream = container.GetContainerStream();
            try
            {
                Metadata currentDirMetadata = metadataManager.FindDirectoryMetadata(containerStream, container.CurrentDirectory);
                if (currentDirMetadata == null || currentDirMetadata.Type != MetadataType.Directory)
                {
                    Console.WriteLine("Error: Current directory metadata not found or invalid.");
                    return;
                }

                // Търсене на директорията за изтриване в съдържанието на текущата директория
                Metadata directoryToDelete = null;
                var directoryContents = metadataManager.GetDirectoryContent(containerStream, currentDirMetadata);
                foreach (var item in directoryContents)
                {
                    if (item.Type == MetadataType.Directory && item.Name == directoryName)
                    {
                        directoryToDelete = item;
                        break;
                    }
                }

                if (directoryToDelete == null)
                {
                    Console.WriteLine($"Error: Directory '{directoryName}' not found in the current directory.");
                    return;
                }

                // Рекурсивно изтриване на директорията и съдържанието ѝ
                DeleteDirectoryRec(containerStream, directoryToDelete);

                // Изчистване на метаданните
                metadataManager.ClearMetadata(containerStream, directoryToDelete.Offset);

                Console.WriteLine($"Directory '{directoryToDelete.Name}' has been deleted successfully.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting directory '{directoryName}': {ex.Message}");
            }
        }


        public void DeleteDirectoryRec(FileStream containerStream, Metadata directoryMetadata)
        {
            if (directoryMetadata == null)
            {
                Console.WriteLine("Error: Invalid directory metadata during deletion.");
                return;
            }
            MyLinkedList<Metadata> directiryContents = metadataManager.GetDirectoryContent(containerStream, directoryMetadata);


            foreach (var item in directiryContents)
            {
                if (item.Type == MetadataType.File)
                {
                    fileBlockManager.ClearFileBlocks(containerStream, item);
                    metadataManager.ClearMetadata(containerStream, item.Offset);
                }
                else if (item.Type == MetadataType.Directory)
                {
                    // Рекурсивно изтриване на поддиректории
                    DeleteDirectoryRec(containerStream, item);
                    metadataManager.ClearMetadata(containerStream, item.Offset);
                }
            }
        }

        public void Undo()
        {
            Console.WriteLine("Undo operation is not implemented for RdsCommand.");
        }
    }
} 





