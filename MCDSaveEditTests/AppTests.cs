using DungeonTools.Save.File;
using MCDSaveEdit;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PakReader.Pak;
using PakReader.Parsers.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
#nullable enable

namespace MCDSaveEditTests
{
    [TestClass]
    public class AppTests
    {
        [TestMethod]
        public void TestExtractGameFiles()
        {
            string? paksFolderPath = ImageUriHelper.usableGameContentIfExists();
            if (string.IsNullOrWhiteSpace(paksFolderPath))
            {
                Assert.Fail("No usable Game Content Files found");
                return;
            }

            PakFilter? filter = new PakFilter(new[] { Constants.PAKS_FILTER_STRING }, false);
            PakIndex? pakIndex = new PakIndex(path: paksFolderPath!, cacheFiles: true, caseSensitive: true, filter: filter);
            pakIndex.UseKey(FGuid.Zero, Secrets.PAKS_AES_KEY_STRING);
            Assert.AreEqual(45388, pakIndex.Count());

            var pakImageResolver = new PakImageResolver(pakIndex);
            pakImageResolver.loadPakFiles();
            Assert.AreEqual(187, ItemExtensions.all.Count);
            Assert.AreEqual(83, EnchantmentExtensions.allEnchantments.Count);

            //Find all the missing and mismatched strings

            foreach(var item in ItemExtensions.all)
            {
                R.itemName(item);
                R.itemDesc(item);
            }

            foreach(var enchantment in EnchantmentExtensions.allEnchantments)
            {
                R.enchantmentName(enchantment);
                //R.enchantmentEffect(enchantment); //missing many of these strings
                R.enchantmentDescription(enchantment);
            }
        }

        [TestMethod]
        public async Task TestReadSaveFile()
        {
            var filePath = Path.Combine(Constants.FILE_DIALOG_INITIAL_DIRECTORY, "2533274911688652", "Characters", "Blank.dat");
            using var stream = await decryptFileIntoStream(filePath);
            stream!.Seek(0, SeekOrigin.Begin);
            var profile = await ProfileParser.Read(stream!);
            Assert.AreEqual(0, profile.Xp);
            Assert.AreEqual(0, profile.TotalGearPower);
        }

        private async Task<Stream?> decryptFileIntoStream(string filePath)
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

        [TestMethod]
        public async Task TestNoDataLossOnWrite()
        {
            var filePath = Path.Combine(Constants.FILE_DIALOG_INITIAL_DIRECTORY, "2533274911688652", "Characters", "Blank.dat");
            using var stream = await decryptFileIntoStream(filePath);

            var copy = new MemoryStream();
            await stream!.CopyToAsync(copy);

            stream!.Seek(0, SeekOrigin.Begin);
            var profile = await ProfileParser.Read(stream!);
            using var output = await ProfileParser.Write(profile);
            verifyNoDataLossOnWrite(copy, output);
        }

        private void verifyNoDataLossOnWrite(Stream input, Stream output)
        {
            var inputLines = getLinesFromJsonStream(input).ToArray();
            var outputLines = getLinesFromJsonStream(output).ToArray();
            int inputLineIndex = 0;
            int outputLineIndex = 0;
            long totalUnequal = 0;
            for (; inputLineIndex < inputLines.Length && outputLineIndex < outputLines.Length; inputLineIndex++, outputLineIndex++)
            {
                if (inputLines[inputLineIndex].StartsWith("\"pendingRewardItem\"")) { inputLineIndex++; }
                if (outputLines[outputLineIndex].StartsWith("\"pendingRewardItem\"")) { outputLineIndex++; }
                if (!inputLines[inputLineIndex].Equals(outputLines[outputLineIndex]))
                {
                    if (inputLines[inputLineIndex].StartsWith("\"power\"") || outputLines[outputLineIndex].StartsWith("\"power\"")) { continue; }
                    totalUnequal++;
                    Console.WriteLine("{0}:{1}", inputLineIndex, inputLines[inputLineIndex]);
                    Console.WriteLine("{0}:{1}", outputLineIndex, outputLines[outputLineIndex]);
                }
            }
            Assert.AreEqual(0, totalUnequal);
        }

        private IEnumerable<string> getLinesFromJsonStream(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var streamStr = stream.readAllText();
            var reader = new StringReader(Utilities.prettyJson(streamStr));
            return reader.readAllLines();
        }

        [TestMethod]
        public void TestProfile()
        {
            var profile = new ProfileSaveFile();
            Assert.IsNotNull(profile);
        }
    }
}
