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
        private string directoryName;

        public RdCommand (MyContainer container, DirectoryManager directoryManager,string directoryName)
        {
            this.container = container;
            this.directoryManager = directoryManager;
            this.directoryName = directoryName;
        }

        public void Execute()
        {
           
            try
            {
                if (!directoryManager.DirectoryExits(directoryName))
                {
                    Console.WriteLine($"Directory '{directoryName}' not found.");
                    return;
                }

                directoryManager.DeleteDirectory(directoryName);
                Console.WriteLine($"Directory '{directoryName}' has been deleted successfully.");
            }
            catch (Exception ex)
            {

                throw new Exception($"Error while deleting derectory {directoryName},{ex.Message}");
            }
        }

        public void Undo()
        {
            Console.WriteLine("Undo operation is not implemented for RdsCommand.");
        }
    }
}