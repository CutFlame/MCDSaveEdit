using MCDSaveEdit;
using MCDSaveEdit.Data;
using MCDSaveEdit.Save.Models.Profiles;
using MCDSaveEdit.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PakReader;
using PakReader.Pak;
using PakReader.Parsers.Objects;
using System;
using System.Collections.Generic;
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
            pakIndex.UseKey(FGuid.Zero, Secrets.PAKS_AES_KEYS[0].key.Substring(2).ToBytesKey());
            Assert.AreEqual(80386, pakIndex.Count());

            var pakImageResolver = new PakContentResolver(pakIndex, null);
            pakImageResolver.loadPakFiles();
            Assert.AreEqual(269, ItemDatabase.all.Count);
            Assert.AreEqual(118, EnchantmentDatabase.allEnchantments.Count);

            var stringLibrary = pakImageResolver.loadLanguageStrings("ru-RU");
            //Using Russian language in order to guarantee every string will not match the english key
            Assert.IsNotNull(stringLibrary);
            R.loadExternalStrings(stringLibrary);
            Assert.AreEqual(2537, R.totalStringCount);

            //Find all the missing and mismatched strings

            foreach(var item in ItemDatabase.all)
            {
                Assert.AreNotEqual(item, R.itemName(item), $"itemName({item}) failed");
                Assert.AreNotEqual(item, R.itemDesc(item), $"itemDesc({item}) failed");
            }

            foreach(var enchantment in EnchantmentDatabase.allEnchantments)
            {
                Assert.AreNotEqual(enchantment, R.enchantmentName(enchantment), $"enchantmentName({enchantment}) failed");
                //R.enchantmentEffect(enchantment); //missing many of these strings
                Assert.AreNotEqual(enchantment, R.enchantmentDescription(enchantment), $"enchantmentDescription({enchantment}) failed");
            }

            foreach (var armorProperty in ItemDatabase.armorProperties)
            {
                Assert.AreNotEqual(armorProperty, R.armorProperty(armorProperty), $"armorProperty({armorProperty}) failed");
                Assert.AreNotEqual(armorProperty, R.armorPropertyDescription(armorProperty), $"armorPropertyDescription({armorProperty}) failed");
            }
        }

        [TestMethod]
        public async Task TestReadSaveFile()
        {
            var solutionDirectory = TestUtilities.tryGetProjectDirectoryInfo()?.FullName;
            Assert.IsNotNull(solutionDirectory);
            var filePath = Path.Combine(solutionDirectory, "TestData", "Blank.dat");
            //var filePath = Path.Combine(Constants.FILE_DIALOG_INITIAL_DIRECTORY, "2533274911688652", "Characters", "Blank.dat");
            using var stream = await TestUtilities.decryptFileIntoStream(filePath);
            stream!.Seek(0, SeekOrigin.Begin);
            var profile = await ProfileParser.Read(stream!);
            Assert.IsNotNull(profile);
            Assert.AreEqual(0, profile!.Xp);
            Assert.AreEqual(0, profile!.TotalGearPower);
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
        [TestMethod]
        public async Task TestHandleReadAndWriteBlank()
        {
            using var stream = TestUtilities.generateStreamFromString(BLANK);

            var copy = new MemoryStream();
            await stream!.CopyToAsync(copy);

            stream!.Seek(0, SeekOrigin.Begin);
            var profile = await ProfileParser.Read(stream!);
            using var output = await ProfileParser.Write(profile);
            verifyNoDataLossOnWrite(copy, output);
        }

        [DataRow("NoEnchantments.dat")]
        [DataRow("ReasonableCheating.dat")]
        [DataRow("Casual.dat")]
        [DataRow("Power200.dat")]
        [DataRow("Blank.dat")]
        [DataRow("SwitchFile.dat")]
        [DataRow("UnreasonableCheating.dat")]
        [DataTestMethod]
        public async Task TestNoDataLossOnWrite(string filename)
        {
            var filePath = Path.Combine(Constants.FILE_DIALOG_INITIAL_DIRECTORY, "2533274911688652", "Characters", filename);
            using var stream = await TestUtilities.decryptFileIntoStream(filePath);

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
            var totalUnequal = TestUtilities.countUnequalLines(inputLines, outputLines);
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
