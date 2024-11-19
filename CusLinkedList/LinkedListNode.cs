using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem.CusLinkedList
{
    public class LinkedListNode<T>
    {
        public T Data;
        public LinkedListNode<T> Next;

        public LinkedListNode(T data)
        {
            Data = data;
            Next = null;

        }

    }
}
