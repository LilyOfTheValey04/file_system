using MyFileSustem.MyCommand;
using MyFileSustem.Test;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyFileSustem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string containerFilePath = "containerFile.bin";
            MyContainer container = new MyContainer(containerFilePath);
            MetadataManager metadataManager = new MetadataManager();
            FileBlockManager blockManager = new FileBlockManager();
            int countBlocks = container.BlockCount;
            MyBitMap bitMap = new MyBitMap(countBlocks);


            
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

            // Initialize command handler
            CommandInvoker invoker = new CommandInvoker(container, metadataManager, blockManager, bitMap);
            Console.WriteLine("Welcome to my file system");
            Console.WriteLine("Enter your commands (type 'exit' to quit)");

            // Command loop
            while (true)
            {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (input == null || input.Trim().ToLower() == "exit")
                {
                    Console.WriteLine("Exiting, bye");
                    break;
                }

                string[] commandArgs = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
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


    


    

