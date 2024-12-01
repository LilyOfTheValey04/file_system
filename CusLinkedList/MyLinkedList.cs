using System;
using System.Collections;
using System.Collections.Generic;


namespace MyFileSustem.CusLinkedList
{
    public class MyLinkedList<T> : IEnumerable<T>
    {
        private MyLinkedListNode<T> head; // the first node in the list
        private MyLinkedListNode<T> tail; // Keep a reference to the last node


        public MyLinkedList()
        {
            head = null;
            tail = null; // Initialize tail as null
        }

        public int Count
        {
            get
            {
                int count = 0;
                MyLinkedListNode<T> current = head;
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
            MyLinkedListNode<T> newNode = new MyLinkedListNode<T>(item);
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
            MyLinkedListNode<T> newNode = new MyLinkedListNode<T>(item);
            if (head == null)
            {
                head = newNode;
                tail = newNode; // The first node is also the last one
            }
            else
            {
                tail.Next = newNode; // Add the new node after the last one
                tail = newNode; // Update tail to the new node
            }
        }

        public void Clear()
        {
            head = null;
        }

        // Проверка дали даден елемент съществува
        public bool Contains(T item)
        {
            MyLinkedListNode<T> current = head;
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
        public MyLinkedListNode<T> Find(T item)
        {
            MyLinkedListNode<T> current = head;
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

        //// Ако не е намерен елемент, връщаме null
        public LinkedList<T> FindFirstWhich(Func<T, bool> matchCondition)
        {
            MyLinkedListNode<T> current = head;
            while (current != null)
            {
                if (matchCondition(current.Data))// Ако обектът отговаря на условията
                {

                }
                current = current.Next;
            }
            return null;// Ако не е намерен елемент, връщаме null
        }

        public T GetFirst()
        {
            if (head == null)
            {
                throw new InvalidOperationException("Cannot retrieve the first element from an empty list.");
            }
            return head.Data;
        }
        // Копиране на елементите в масивя
        public void CopyTo(T[] array, int arrayIndex)
        {
            MyLinkedListNode<T> current = head;
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

            MyLinkedListNode<T> current = head;
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
            MyLinkedListNode<T> current = head;
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
            MyLinkedListNode<T> current = head;
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
            MyLinkedListNode<T> previous = null;
            MyLinkedListNode<T> current = head;
            MyLinkedListNode<T> next = null;

            while (current != null)
            {
                next = current.Next; //запазваме връзка към следващия ел за да не го изгубем
                current.Next = previous; // объръщамe посоката на връзката
                previous = current;
                current = next;
            }
            head = previous;
        }


        // Итератор за обхождане на списъка
        public IEnumerator<T> GetEnumerator()
        {
            MyLinkedListNode<T> current = head;
            while (current != null)
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
