using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem.MyCommand
{
    internal class CommandInvoker
    {
            private MyContainer container;
            private MetadataManager metadataManager;
            private FileBlockManager fileBlockManager;
            private MyBitMap bitMap;

            public CommandInvoker(MyContainer container, MetadataManager metadataManager, FileBlockManager fileBlockManager, MyBitMap bitMap)
            {
                this.container = container;
                this.metadataManager = metadataManager;
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
                    Console.WriteLine("Usage: cpin <sourcePath> <containerFileName>");
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
                    Console.WriteLine("Usage: cpout <containerFileName> <destinationPath>");
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
                    Console.WriteLine("Usage: rm <containerFileName>");
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

        }
    }



