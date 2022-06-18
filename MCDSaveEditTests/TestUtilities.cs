using DungeonTools.Save.File;
using MCDSaveEdit.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
#nullable enable

namespace MCDSaveEditTests
{
    public static class TestUtilities
    {
        public static DirectoryInfo? tryGetProjectDirectoryInfo(string? currentPath = null)
        {
            var directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.csproj").Any())
            {
                directory = directory.Parent;
            }
            return directory;
        }

        public static async Task<Stream?> decryptFileIntoStream(string filePath)
        {
            var file = new FileInfo(filePath);
            using FileStream inputStream = file.OpenRead();
            bool encrypted = SaveFileHandler.IsFileEncrypted(inputStream);
            if (!encrypted)
            {
                Assert.Fail($"The file \"{file.Name}\" was in an unexpected format.");
                return null;
            }
            Stream? stream = await FileProcessHelper.Decrypt(inputStream);
            if (stream == null)
            {
                Assert.Fail($"Content of file \"{file.Name}\" could not be converted to a supported format.");
                return null;
            }
            return stream;
        }

        public static Stream generateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static int countUnequalLines(string[] inputLines, string[] outputLines)
        {
            int inputLineIndex = 0;
            int outputLineIndex = 0;
            int totalUnequal = 0;
            for (; inputLineIndex < inputLines.Length && outputLineIndex < outputLines.Length; inputLineIndex++, outputLineIndex++)
            {
                if (inputLines[inputLineIndex].EndsWith("null,")) { inputLineIndex++; }
                if (outputLines[outputLineIndex].EndsWith("null,")) { outputLineIndex++; }

                if (!inputLines[inputLineIndex].Equals(outputLines[outputLineIndex]))
                {
                    if (inputLines[inputLineIndex].StartsWith("\"power\"") && outputLines[outputLineIndex].StartsWith("\"power\"")) { continue; }
                    if (inputLines[inputLineIndex].StartsWith("\"priceMultiplier\"") && outputLines[outputLineIndex].StartsWith("\"priceMultiplier\"")) { continue; }
                    if (inputLines[inputLineIndex].StartsWith("\"rebateFraction\"") && outputLines[outputLineIndex].StartsWith("\"rebateFraction\"")) { continue; }
                    totalUnequal++;
                    Console.WriteLine(">>> {0}:{1}", inputLineIndex, inputLines[inputLineIndex]);
                    Console.WriteLine("<<< {0}:{1}", outputLineIndex, outputLines[outputLineIndex]);
                }
            }
            return totalUnequal;
        }

    }
}
