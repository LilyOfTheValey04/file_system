using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSustem.Test
{
    internal class Tester
    {
        public static void TestCreateContainer()
        {
            var container = new MyContainer();
            container.CreateContainer();

            // Check if file exists
            if (File.Exists("filesystem.bin"))
            {
                Console.WriteLine("Container created successfully.");
            }
            else
            {
                Console.WriteLine("Container creation failed.");
            }

            // Check file size (bitmap size + empty blocks)
            long expectedSize = (MyContainer.DefaultBlockCount / 8) + (MyContainer.DefaultBlockSize * MyContainer.DefaultBlockCount);
            var fileInfo = new FileInfo("filesystem.bin");
            if (fileInfo.Length == expectedSize)
            {
                Console.WriteLine("Container size is correct.");
            }
            else
            {
                Console.WriteLine("Container size is incorrect.");
            }
        }

        public static void TestOpenContainer()
        {
            static void TestOpenContainer()
            {
                var container = new MyContainer();
                container.OpenContainer(FileMode.Open);

                // Perform some operation to ensure the bitmap is loaded
                int freeBlock = container.FindAndMarkFreeBlock();
                if (freeBlock >= 0)
                {
                    Console.WriteLine($"Free block found and marked as used: {freeBlock}");
                }
                else
                {
                    Console.WriteLine("No free block found or failed to load bitmap.");
                }
            }
        }

     /*   public static void TestBitmapOperations()
        {
            var container = new MyContainer();
            container.CreateContainer();

            container.OpenContainer(FileMode.Open);

            // Find and mark a free block
            int block = container.FindAndMarkFreeBlock();
            if (block >= 0)
            {
                Console.WriteLine($"Marked block {block} as used.");

                // Check if the block is now used
                bool isFree = container._bitmap.IsBlockFree(block);
                if (!isFree)
                {
                    Console.WriteLine($"Block {block} is correctly marked as used.");
                }
                else
                {
                    Console.WriteLine($"Error: Block {block} should be marked as used.");
                }

                // Release the block
                container.ReleaseBlock(block);
                Console.WriteLine($"Released block {block}.");

                // Check if the block is now free
                isFree = container._bitmap.IsBlockFree(block);
                if (isFree)
                {
                    Console.WriteLine($"Block {block} is correctly marked as free.");
                }
                else
                {
                    Console.WriteLine($"Error: Block {block} should be marked as free.");
                }
            }
            else
            {
                Console.WriteLine("No free block found.");
            }
        }*/

        public static void RunAllTests()
        {
            TestCreateContainer();
            TestOpenContainer();
            //TestBitmapOperations();
        }
    }
}
    

