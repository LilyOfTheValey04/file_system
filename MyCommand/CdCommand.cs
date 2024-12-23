using MyFileSustem.MyManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem.MyCommand
{
    public class CdCommand : ICommand
    {
        private readonly DirectoryManager _directoryManager;
        private readonly string _directoryName;

        public CdCommand(DirectoryManager _directoryManager, string _directoryName)
        {
            this._directoryManager = _directoryManager;
            this._directoryName = _directoryName;

        }
        public void Execute()
        {
            _directoryManager.ChangeDirectory(_directoryName);
        }

        public void Undo()
        {
            Console.WriteLine("Undo operation is not implemented for CdCommand.");
        }
    }
}
