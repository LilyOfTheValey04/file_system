using MyFileSustem.MyManagers;
using System;
using System.Collections.Generic;
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

        public MdCommand(MyContainer container, MetadataManager metadataManager, string directoryName)
        {
            this.container = container;
            this.metadataManager = metadataManager;
            this.directoryName = directoryName;
        }

        public void Execute()
        {
            try
            {
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
