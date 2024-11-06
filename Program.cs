using MyFileSustem.Test;
using System;
using System.Collections.Generic;
using System.IO;

namespace MyFileSustem
{
    internal class Program
    {

        static void Main(string[] args)
        {
            // Create a test file path
            string filePath = "test_metadata.dat";

            try
            {
                // Step 1: Create and populate original metadata
                MetadataManager originalMetadata = new MetadataManager
                {
                    FileName = "example.txt",
                    FileLocation = "/documents",
                    FileDateTime = DateTime.Now,
                    FileSize = 2048,
                    BlockPosition = 5
                };

                // Step 2: Write the metadata to a file
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    originalMetadata.MetadataWriter(stream, 0);
                }

                // Step 3: Read the metadata from the file
                MetadataManager readMetadata = new MetadataManager();
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    readMetadata.MetadataReader(stream, 0);
                }

                // Step 4: Validate and display results
                bool isWorking = originalMetadata.FileName == readMetadata.FileName &&
                                 originalMetadata.FileLocation == readMetadata.FileLocation &&
                                 originalMetadata.FileDateTime == readMetadata.FileDateTime &&
                                 originalMetadata.FileSize == readMetadata.FileSize &&
                                 originalMetadata.BlockPosition == readMetadata.BlockPosition;

                if (isWorking)
                {
                    Console.WriteLine("Metadata class is working correctly!");
                }
                else
                {
                    Console.WriteLine("Metadata class is NOT working correctly.");
                    // Output differences for better debugging
                    Console.WriteLine($"Expected: {originalMetadata.FileName}, Read: {readMetadata.FileName}");
                    Console.WriteLine($"Expected: {originalMetadata.FileLocation}, Read: {readMetadata.FileLocation}");
                    Console.WriteLine($"Expected: {originalMetadata.FileDateTime}, Read: {readMetadata.FileDateTime}");
                    Console.WriteLine($"Expected: {originalMetadata.FileSize}, Read: {readMetadata.FileSize}");
                    Console.WriteLine($"Expected: {originalMetadata.BlockPosition}, Read: {readMetadata.BlockPosition}");
                }

                // Optional: Display the read metadata
                Console.WriteLine("Read Metadata:");
                readMetadata.DisplayMetadata();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Clean up: Delete the test file
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            //    Tester.RunAllTests();

        }
        }

    }

