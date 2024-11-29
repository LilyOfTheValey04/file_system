using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
