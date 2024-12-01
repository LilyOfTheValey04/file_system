

namespace MyFileSustem
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
}
