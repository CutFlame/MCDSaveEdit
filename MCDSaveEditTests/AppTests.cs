using DungeonTools.Save.File;
using MCDSaveEdit;
using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Save.Models.Profiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PakReader;
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
        private readonly AppModel _model = new AppModel();

        [TestMethod]
        public void TestExtractGameFiles()
        {
            string? paksFolderPath = _model.usableGameContentIfExists();
            if (string.IsNullOrWhiteSpace(paksFolderPath))
            {
                Assert.Fail("No usable Game Content Files found");
                return;
            }

            PakFilter? filter = new PakFilter(new[] { Constants.PAKS_FILTER_STRING }, false);
            PakIndex? pakIndex = new PakIndex(path: paksFolderPath!, cacheFiles: true, caseSensitive: true, filter: filter);
            pakIndex.UseKey(FGuid.Zero, Secrets.PAKS_AES_KEY_STRINGS[0].Substring(2).ToBytesKey());
            Assert.AreEqual(64969, pakIndex.Count());

            var pakImageResolver = new PakImageResolver(pakIndex, null);
            pakImageResolver.loadPakFiles();
            Assert.AreEqual(242, ItemExtensions.all.Count);
            Assert.AreEqual(108, EnchantmentExtensions.allEnchantments.Count);

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

            foreach (var armorProperty in ItemExtensions.armorProperties)
            {
                R.armorProperty(armorProperty);
                R.armorPropertyDescription(armorProperty);
            }
        }

        [TestMethod]
        public async Task TestReadSaveFile()
        {
            var solutionDirectory = TryGetProjectDirectoryInfo()?.FullName;
            Assert.IsNotNull(solutionDirectory);
            var filePath = Path.Combine(solutionDirectory, "TestData", "Blank.dat");
            //var filePath = Path.Combine(Constants.FILE_DIALOG_INITIAL_DIRECTORY, "2533274911688652", "Characters", "Blank.dat");
            using var stream = await decryptFileIntoStream(filePath);
            stream!.Seek(0, SeekOrigin.Begin);
            var profile = await ProfileParser.Read(stream!);
            Assert.AreEqual(0, profile.Xp);
            Assert.AreEqual(0, profile.TotalGearPower);
        }

        public static DirectoryInfo? TryGetProjectDirectoryInfo(string? currentPath = null)
        {
            var directory = new DirectoryInfo(currentPath ?? Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.csproj").Any())
            {
                directory = directory.Parent;
            }
            return directory;
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

        private const string BLANK = @"
{
  ""bonus_prerequisites"": [],
  ""clone"": false,
  ""cosmetics"": [],
  ""cosmeticsEverEquipped"": [],
  ""creationDate"": ""Jun 23, 2020"",
  ""currency"": [],
  ""customized"": false,
  ""items"": [],
  ""itemsFound"": [],
  ""name"": """",
  ""playerId"": ""C2BC12F8-4800-51B5-D9E3-6F9E2865F96D"",
  ""progressionKeys"": [],
  ""skin"": ""steve"",
  ""timestamp"": 1592978132,
  ""totalGearPower"": 0,
  ""trialsCompleted"": [],
  ""version"": 1,
  ""xp"": 0
}
";

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        [TestMethod]
        public async Task TestHandleReadAndWriteBlank()
        {
            using var stream = GenerateStreamFromString(BLANK);

            var copy = new MemoryStream();
            await stream!.CopyToAsync(copy);

            stream!.Seek(0, SeekOrigin.Begin);
            var profile = await ProfileParser.Read(stream!);
            using var output = await ProfileParser.Write(profile);
            verifyNoDataLossOnWrite(copy, output);
        }

        [DataRow("Power200.dat")]
        [DataRow("Blank.dat")]
        [DataRow("UnreasonableCheating.dat")]
        [DataTestMethod]
        public async Task TestNoDataLossOnWrite(string filename)
        {
            var filePath = Path.Combine(Constants.FILE_DIALOG_INITIAL_DIRECTORY, "2533274911688652", "Characters", filename);
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
