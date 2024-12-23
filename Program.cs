using MyFileSustem.MyCommand;
using MyFileSustem.MyManagers;
using System;
using System.IO;

namespace MyFileSustem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string containerFilePath = "containerFile.bin";
            MyContainer container = new MyContainer(containerFilePath);
            MetadataManager metadataManager = new MetadataManager(container);
            FileBlockManager fileBlockManager = new FileBlockManager(container);
            DirectoryManager directoryManager = new DirectoryManager(metadataManager,container,fileBlockManager);
            int countBlocks = container.MetadataBlockCount;

            // Create or load the container
            if (!File.Exists(containerFilePath))
            {
                Console.WriteLine("Container file not found. Creating a new container...");
                container.CreateContainer();
                Console.WriteLine("Container created successfully.");
            }
            else
            {
                Console.WriteLine("Container found. Loading...");
                // Load bitmap from the container
                Console.WriteLine("Loading bitmap...");

                //   bitMap.Deserialize(container.GetContainerStream());
            }
            container.OpenContainerStream();
            metadataManager.createRootDir();

            // Initialize command handler
            CommandInvoker invoker = new CommandInvoker(container, metadataManager, directoryManager,fileBlockManager, container._bitmap);
            Console.WriteLine("Welcome to my file system");
            Console.WriteLine("Enter your commands (type 'exit' to quit)");

            // Command loop
            while (true)
            {
                Console.Write(container.CurrentDirectory + "> ");
                string input = Console.ReadLine();
                //  if (input == null || input.Trim().ToLower() == "exit")
                if (input == null || Utilities.CustomTrim(Utilities.CustomToLower(input)) == "exit")

                {
                    Console.WriteLine("Exiting, bye");
                    break;
                }

                //string[] commandArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string[] commandArgs = Utilities.CustomSplitAndRemoveEmpty(input,' ');

                if (commandArgs.Length > 0)
                {
                    invoker.Execute(commandArgs);
                }
                else
                {
                    Console.WriteLine("No command entered. Please try again.");
                }
            }
            container.CloseContainerStream();
        }
    }
}







