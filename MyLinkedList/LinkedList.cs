using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem.MyLinkedList
{
    internal class LinkedList<T> : IEnumerable<T>
    {
        private LinkedListNode<T> head; // the first node in the list

        public LinkedList()
        {
            head = null;
        }
        
        public int Count
        {
            get
            {
                int count = 0;
                LinkedListNode<T> current = head;
                while (current != null)
                {
                    count++;
                    current = current.Next;
                }
                return count;
            }
        }

        public bool IsReadOnly => false;

        public void AddFirst(T item)
        {
            LinkedListNode<T> newNode = new LinkedListNode<T>(item);
            if (head == null)
            {
                head = newNode;
            }
            else
            {
                newNode.Next = head;
                head = newNode;
            }
        }

        // Добавяне на нов елемент в края на списъка
        public void AddLast(T item)
        {
            LinkedListNode<T> newNode = new LinkedListNode<T>(item);
            if (head == null)
            {
                head = newNode;
            }
            else
            {
                LinkedListNode<T> current = head;
                while (current.Next != null)
                {
                    current = current.Next;
                }
                current.Next = newNode;
            }
        }

        public void Clear()
        {
            head = null;
        }

        // Проверка дали даден елемент съществува
        public bool Contains(T item)
        {
            LinkedListNode<T> current = head;
            while (current != null)
            {
                if (current.Data.Equals(item))
                {
                    return true;
                }
                current = current.Next;
            }
            return false;
        }
        public LinkedListNode<T> Find(T item)
        {
            LinkedListNode<T> current = head;
            while (current != null)
            {
                if (current.Data.Equals(item))
                {
                    return current;
                }
                current = current.Next;
            }
            return null;
        }

        // Копиране на елементите в масивя
        public void CopyTo(T[] array, int arrayIndex)
        {
            LinkedListNode<T> current = head;
            for (int i = arrayIndex; i < array.Length && current != null; i++)
            {
                array[i] = current.Data;
                current = current.Next;
            }
        }


        public bool Remove(T item)
        {
            if (head == null)
            {
                return false;
            }

            if (head.Data.Equals(item))
            {
                head = head.Next;
                return true;
            }

            LinkedListNode<T> current = head;
            while (current.Next != null)
            {
                if (current.Next.Data.Equals(item))
                {
                    // Remove the next node by skipping it
                    current.Next = current.Next.Next;
                    return true; // Node removed
                }

                current = current.Next; // Move to the next node
            }

            return false; // Item not found(not removed)
        }

        public bool RemoveFirst()
        {
            LinkedListNode<T> current = head;
            if (head == null)
            {
                return false;
            }
            else
            {
                head = head.Next;
                return true;
            }
        }

        public bool RemoveLast()
        {
            LinkedListNode<T> current = head;
            if (head == null)
            {
                return false;
            }
            if (head.Next == null)
            {
                head = null;
                return true;
            }

            while (current.Next.Next != null)//Обхождане до предпоследния елемент
            {
                current = current.Next;
            }
            current.Next = null;
            return true;
        }

        public void Reverse()
        {
            LinkedListNode<T> previous = null;
            LinkedListNode<T> current = head;
            LinkedListNode<T> next = null;

            while (current!=null)
            {
                next = current.Next; //запазваме връзка към следващия ел за да не го изгубем
                current.Next = previous; // объръщамe посоката на връзката
                previous = current;
                current= next;
            }
            head = previous;
        }
    

        // Итератор за обхождане на списъка
        public IEnumerator<T> GetEnumerator()
        {
            LinkedListNode<T> current = head;
            while(current!=null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }
        // Неявен интерфейс за IEnumerator
        // Explicit implementation of IEnumerable.GetEnumerator
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // Call the generic version
        }
    }
}
