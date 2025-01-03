﻿using System;
using System.IO;


namespace MyFileSustem
{
    public class MyBitMap
    {
        private readonly byte[] _bitmap;
        private readonly int _size;

        public MyBitMap(int size)
        {
            _size = size;
            _bitmap = new byte[(size + 7) / 8]; // Initialize the bitmap
            for (int i = 0; i < _size; i++)
            {
                MarkBlockAsFree(i);
            }

        }

        //проверява дали даден блок в контейнера е свободен
        public bool IsBlockFree(int index)
        {
            if (index < 0 || index >= _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Block index is out of range.");
            }

            return (_bitmap[index / 8] & (1 << (index % 8))) == 0;
        }

        public void MarkBlockAsUsed(int index)
        {
            if (index < 0 || index >= _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Block index is out of range.");
            }

            _bitmap[index / 8] |= (byte)(1 << (index % 8));
        }

        public void MarkBlockAsFree(int index)
        {
            if (index < 0 || index >= _size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Block index is out of range.");
            }

            _bitmap[index / 8] &= (byte)~(1 << (index % 8));
        }

        public int CountFreeBlocks()
        {
            int freeblocks = 0;
            for (int i = 0; i < _size; i++)
            {
                if (IsBlockFree(i))
                {
                    ++freeblocks;
                }
            }
            return freeblocks;
        }

        public int Size => _size;

        // Serialize bitmap to container
        public void Serialize(FileStream container)
        {
            container.Seek(0, SeekOrigin.Begin);// Премества указателя за четене/писане в началото на файла.
            container.Write(_bitmap, 0, _bitmap.Length);// Записва съдържанието на масива _bitmap в контейнера.
        }


        // Deserialize bitmap from container
        public void Deserialize(FileStream container)
        {
            container.Seek(0, SeekOrigin.Begin);
            container.Read(_bitmap, 0, _bitmap.Length);
        }

        // Find the first free block
        public int FindFirstFreeBlock()
        {
            for (int i = 0; i < _size; i++)
            {
                try
                {
                    if (IsBlockFree(i))
                    {
                        Console.WriteLine($"Debug: Found free block at index {i}");
                        return i;
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
            Console.WriteLine("Debug: No free blocks found.");
            return -1; // No free block found
        }
    }
}
