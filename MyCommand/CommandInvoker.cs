using MyFileSustem.CusLinkedList;
using MyFileSustem.MyManagers;
using System;
using System.ComponentModel;
using System.IO;


namespace MyFileSustem.MyCommand
{
    internal class CommandInvoker
    {
        private MyContainer container;
        private MetadataManager metadataManager;
        private DirectoryManager directoryManager;
        private FileBlockManager fileBlockManager;
        private MyBitMap bitMap;
        private Metadata metadata;

        public CommandInvoker(MyContainer container, MetadataManager metadataManager,DirectoryManager directoryManager ,FileBlockManager fileBlockManager, MyBitMap bitMap)
        {
            this.container = container;
            this.metadataManager = metadataManager;
            this.directoryManager = directoryManager;
            this.fileBlockManager = fileBlockManager;
            this.bitMap = bitMap;
        }

        public void Execute(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("No commands enter.");
                return;
            }

            // Първият аргумент е името на командата
            string commandName = args[0].ToLower();
            try
            {
                switch (commandName)
                {
                    case "cpin":
                        ExecuteCpin(args);
                        break;

                    case "cpout":  // Поправено от "cpon" на "cpout"
                        ExecuteCpout(args);
                        break;

                    case "ls":
                        ExecuteLs(args);
                        break;

                    case "rm":
                        ExecuteRm(args);
                        break;
                    case "md":
                        ExecuteMd(args);
                        break;
                    case "cd":
                        ExecuteCd(args);
                        break;
                    case "rd":
                        ExecuteRd(args);
                        break;

                    default:
                        Console.WriteLine($"Unknown command: {commandName}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing command '{commandName}': {ex.Message}");
            }
        }

        private void ExecuteCpin(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: cpin <sourcePath> <directoryName>");
                return;
            }
            string sourcePath = args[1];
            string containerFileName = args[2];
            ICommand cpinCommand = new CpinCommand(container, metadataManager, fileBlockManager, sourcePath, containerFileName, bitMap);
            cpinCommand.Execute();
        }

        private void ExecuteCpout(string[] args)  // Поправено от "ExecuteCpon" на "ExecuteCpout"
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: cpout <directoryName> <destinationPath>");
                return;
            }
            string containerFileName = args[1];
            string destinationPath = args[2];
            ICommand cpoutCommand = new CpoutCommand(container, metadataManager, fileBlockManager, containerFileName, destinationPath);
            cpoutCommand.Execute();
        }

        private void ExecuteRm(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: rm <directoryName>");
                return;
            }
            string containerFileName = args[1];
            ICommand rmCommand = new RmCommand(container, metadataManager, fileBlockManager, containerFileName);
            rmCommand.Execute();
        }

        private void ExecuteLs(string[] args)
        {
            ICommand lsCommand = new LsCommand(container, metadataManager);
            lsCommand.Execute();
        }

        private void ExecuteMd(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: md <directoryName>");
                return;
            }

            string directoryName = args[1];

            if (Utilities.IsItNullorWhiteSpace(directoryName))
            {
                Console.WriteLine("Invalid name");
                return;
            }

            /*   // Определяне на пътя за новата директория (по подразбиране в root директорията)
               string location = "/";
               FileStream containerStream = container.GetContainerStream();
               long newMetadataOffset = metadataManager.GetNextAvalibleMetadataOfset(containerStream, container.MetadataOffset, container.MetadataBlockCount);

               // Създаване на нова директория
               Metadata newDirectory = new Metadata(
                   Name: directoryName,
                   Location: location,
                   Type: MetadataType.Directory,
                   DateOfCreation: DateTime.Now,
                   Size: 0,
                   MetadataOffset: newMetadataOffset,
                   BlocksPositionsList: new MyLinkedList<int>());
               metadataManager.MetadataWriter(containerStream, newDirectory);*/
            // Създаване на екземпляр на MdCommand
            MdCommand mdCommand = new MdCommand(container, metadataManager, directoryManager,directoryName);

            // Извикване на Execute метода на MdCommand
            mdCommand.Execute();
            //Console.WriteLine($"Directory '{directoryName}' created at location '{}'.");

        }
        private void ExecuteCd(string[] args)
        {
            if (args.Length<2)
            {
                Console.WriteLine("Usage:cd <directoryName>");
                return;
            }
            try
            {
            string directoryName = args[1];
            ICommand cdCommand = new CdCommand(directoryManager,directoryName);
            cdCommand.Execute();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing cd command: {ex.Message}");
            }

        }
        private void ExecuteRd(string[] args)
        {
           // FileStream containerStream = container.GetContainerStream();
            if (args.Length < 2)
            {
                Console.WriteLine("Useage: rd <directoryName>");
                return;
            }
            string directoryName = args[1];
            ICommand rdCommand = new RdCommand(container,directoryManager,directoryName);
            rdCommand.Execute();
        }


    }
}



