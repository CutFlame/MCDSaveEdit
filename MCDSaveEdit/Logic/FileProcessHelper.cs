using DungeonTools.Save.File;
using System;
using System.IO;
using System.Threading.Tasks;



namespace MCDSaveEdit
{
    public static class FileProcessHelper
    {
        public static async ValueTask ProcessFile(FileInfo file, bool overwrite)
        {
            if (!file.Exists)
            {
                await Console.Error.WriteLineAsync($"[  ERROR  ] File \"{file.FullName}\" could not be found and has been skipped.");
                return;
            }

            using FileStream inputStream = file.OpenRead();
            bool encrypted = SaveFileHandler.IsFileEncrypted(inputStream);

            Stream? processed = encrypted ? await Decrypt(inputStream) : await Encrypt(inputStream);
            if (processed == null)
            {
                await Console.Out.WriteLineAsync($"[  ERROR  ] Content of file \"{file.Name}\" could not be converted to a supported format.");
                return;
            }

            processed.Seek(0, SeekOrigin.Begin);
            string outputFile = GetOutputFilePath(file, encrypted, overwrite);
            using FileStream outputStream = File.Open(outputFile, FileMode.Create, FileAccess.Write);
            await processed.CopyToAsync(outputStream);
        }

        public static async ValueTask<Stream?> Decrypt(Stream data)
        {
            Stream decrypted = await EncryptionProviders.Current.DecryptAsync(data);
            Stream result = SaveFileHandler.RemoveTrailingZeroes(decrypted);
            result.Seek(0, SeekOrigin.Begin);
            return result;
        }

        public static async ValueTask<Stream?> Encrypt(Stream data)
        {
            using Stream encrypted = await EncryptionProviders.Current.EncryptAsync(data);
            Stream result = SaveFileHandler.PrependMagicToEncrypted(encrypted);
            result.Seek(0, SeekOrigin.Begin);
            return result;
        }

        private static string GetOutputFilePath(FileInfo fileInfo, bool isEncrypted, bool overwrite)
        {
            string targetExtension = fileInfo.Extension.ToUpperInvariant() switch
            {
                "" => isEncrypted ? ".json" : "", // Special case for Switch which has no file extension
                _ => isEncrypted ? ".json" : ".dat",
            };

            string idealFileName = $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}{targetExtension}";
            if (string.Equals(fileInfo.Name, idealFileName, StringComparison.CurrentCultureIgnoreCase))
            {
                idealFileName = $"{Path.GetFileNameWithoutExtension(idealFileName)}_{(isEncrypted ? "Decrypted" : "Encrypted")}{targetExtension}";
            }

            string outFileName = Path.Combine(fileInfo.DirectoryName, idealFileName);
            if (overwrite || !File.Exists(outFileName))
            {
                return outFileName;
            }

            int fileNumber = 1;
            while (File.Exists(outFileName))
            {
                outFileName = $"{outFileName.Substring(outFileName.Length - targetExtension.Length)}_{fileNumber++}{targetExtension}";
            }

            return outFileName;
        }

    }
}
