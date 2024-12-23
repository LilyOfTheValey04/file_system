using MyFileSustem.MyManagers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem.MyCommand
{
    internal class MdCommand : ICommand
    {
        private readonly MyContainer container;
        private readonly MetadataManager metadataManager;
        private readonly DirectoryManager directoryManager;
        private readonly string directoryName;

        public MdCommand(MyContainer container, MetadataManager metadataManager, DirectoryManager directoryManager, string directoryName)
        {
            this.container = container;
            this.metadataManager = metadataManager;
            this.directoryManager = directoryManager;
            this.directoryName = directoryName;
        }

        public void Execute()
        {
            try
            {
               string newDirectoryPath = Path.Combine(container.CurrentDirectory, directoryName).Replace("\\","/");
                if (directoryManager.DirectoryExits(newDirectoryPath))
                {
                    Console.WriteLine($"A directory with {directoryName} name already exists");
                    return;
                }
                directoryManager.AddDirectory(container.CurrentDirectory,directoryName);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating directory '{directoryName}':{ex.Message}");
            }
        }

        public void Undo()
        {
            try
            {
                directoryManager.DeleteDirectory(directoryName);
            }
            catch (Exception ex)
            {

                throw new Exception($"Error undoing directory {directoryName} creation : {ex.Message}");
            }
        }
    }
}
