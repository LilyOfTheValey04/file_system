using MyFileSustem.CusLinkedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem.MyManagers
{
    public class DirectoryManager
    {
        private readonly FileBlockManager _fileblockManager;
        private readonly MetadataManager _metadataManager;
        private readonly MyContainer _container;

        public DirectoryManager(MetadataManager metadataManager, MyContainer container, FileBlockManager fileBlockManager)
        {
            _metadataManager = metadataManager;
            _container = container;
            _fileblockManager = fileBlockManager;
        }

        public void AddDirectory(string parentDirectory, string directoryName)
        {
            if (string.IsNullOrEmpty(parentDirectory))
            {
                parentDirectory = _container.CurrentDirectory;
            }

            Metadata parentMeatadata = _metadataManager.FindDirectoryMetadata(_container.GetContainerStream(), parentDirectory);
            if (parentMeatadata == null || parentMeatadata.Type != MetadataType.Directory)
            {
                throw new InvalidOperationException($"The parent directory {parentDirectory} is not found");
            }
            long metadataOffset = _container.MetadataOffset +
                             _metadataManager.GetTotalMetadataCount(_container.GetContainerStream(),
                                                                    _container.MetadataOffset,
                                                                    _container.MetadataRegionSize) * Metadata.MetadataSize;
            Metadata newDirectory = new Metadata(Name: directoryName,
                Location: parentDirectory,
                Type: MetadataType.Directory,
                DateOfCreation: DateTime.Now,
                Size: 0,
                MetadataOffset: metadataOffset,
                BlocksPositionsList: new MyLinkedList<int>()
                );
            _metadataManager.MetadataWriter(_container.GetContainerStream(), newDirectory);
            Console.WriteLine($"Directory '{directoryName}' created in '{parentDirectory}'.");
        }

        public void DeleteDirectory(string directoryPath)
        {
            FileStream containerStream = null;
            try
            {
                containerStream = _container.GetContainerStream();
                Metadata directoryMetadata = _metadataManager.FindDirectoryMetadata(containerStream, directoryPath);
                if (directoryMetadata == null || directoryMetadata.Type != MetadataType.Directory)
                {
                    throw new InvalidOperationException($"Directory '{directoryPath}' does not exist.");
                }

                // Взимаме всички файлове и поддиректории в директорията
                var derectoryContend = _metadataManager.GetDirectoryContent(containerStream, directoryMetadata);

                // Изтриваме файловете и поддиректориите рекурсивно
                foreach (var item in derectoryContend)
                {
                    if (item.Type == MetadataType.File)
                    {
                        // Метод за изтриване на файл
                        _fileblockManager.ClearFileBlocks(containerStream, item);
                        _metadataManager.ClearMetadata(containerStream,item.Offset);

                    }
                    else if (item.Type == MetadataType.Directory)
                    {
                        DeleteDirectory(item.Name);// Рекурсивно изтриване на поддиректории
                    }
                }

                // Накрая изтриваме текущата директория
                long metadataOffset = directoryMetadata.Offset;
                _metadataManager.ClearMetadata(containerStream, metadataOffset);
                Console.WriteLine($"Directory '{directoryPath}' successfully deleted.");

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error deleting derectory '{directoryPath}' : {ex.Message}");
            }
            finally
            {
                if (containerStream == null) 
                { 
                    _container.CloseContainerStream();
                }
            }

        }

        public bool DirectoryExits(string directoryPath)
        {
            Metadata directoryMetadata = _metadataManager.FindDirectoryMetadata(_container.GetContainerStream(), directoryPath);
            return directoryMetadata != null || directoryMetadata.Type == MetadataType.Directory;
        }
    }
}
