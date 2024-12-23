using MyFileSustem.CusLinkedList;
using System;
using System.IO;

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
                        _metadataManager.ClearMetadata(containerStream, item.Offset);

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
            return directoryMetadata != null && directoryMetadata.Type == MetadataType.Directory;
        }
        public bool IsItRootDirectory(string currentDirectory, Metadata metadata)
        {
            return currentDirectory == "/" || metadata == null || Utilities.IsItNullorWhiteSpace(metadata.Location);
        }
        public void ChangeDirectory(string directoryName)
        {
            //!!!!
            FileStream containerStream = _container.GetContainerStream();
            try
            {
                if (directoryName == "..")
                {
                    // Връщане към родителска директория
                    Metadata currentMetadata = _metadataManager.FindDirectoryMetadata(containerStream, _container.CurrentDirectory);

                    if (IsItRootDirectory(_container.CurrentDirectory,currentMetadata))
                    {
                        Console.WriteLine("Already at the root directory");
                        return;
                    }

                    _container.CurrentDirectory = currentMetadata.Location;
                    Console.WriteLine($"Change current directory to {_container.CurrentDirectory}");
                }
                else if (directoryName == "\\")
                {
                    // Връщане към коренната директория
                    if (_container.CurrentDirectory=="/")
                    {
                        Console.WriteLine("Already at the root directory");
                        return;
                    }
                    _container.CurrentDirectory = "/";
                    Console.WriteLine("Changed directory to root.");
                }
                else
                {
                    // Смяна към поддиректория
                    //  string newPath = $"{_container.CurrentDirectory}{directoryName}".Replace("//","/") ;
                    string newPath = Path.Combine(_container.CurrentDirectory, directoryName).Replace("\\", "/");

                    Metadata targetMetadata = _metadataManager.FindDirectoryMetadata(containerStream, newPath);

                    if (targetMetadata == null || targetMetadata.Type != MetadataType.Directory)
                    {
                        Console.WriteLine($"Directory {directoryName} no found");
                        return;
                    }
                    _container.CurrentDirectory = newPath;
                    Console.WriteLine($"Directory changed to {_container.CurrentDirectory}");

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error changing the directory:{ex.Message}");
            }
        }
    }
}
