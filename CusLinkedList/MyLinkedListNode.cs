
namespace MyFileSustem.CusLinkedList
{
    public class MyLinkedListNode<T>
    {
        public T Data;
        public MyLinkedListNode<T> Next;

        public MyLinkedListNode(T data)
        {
            Data = data;
            Next = null;
        }

    }
}
