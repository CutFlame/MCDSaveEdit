using DungeonTools.Save.File;
using MCDSaveEdit.Save.Models.Profiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
#nullable enable

namespace MCDSaveEdit
{
    public class MainViewModel
    {
        public Action<string>? showError;

        private List<FileInfo> _recentFilesInfos = new List<FileInfo>();
        public IReadOnlyCollection<FileInfo> recentFilesInfos { get { return _recentFilesInfos; } }

        public MainViewModel()
        {
            loadRecentFilesList();
        }

        #region Recent Files List

        private void loadRecentFilesList()
        {
            // Reload items from the registry.
            for (int i = 0; i < Constants.MAX_RECENT_FILES; i++)
            {
                string file_name = (string)RegistryTools.GetSetting(Constants.APPLICATION_NAME, "FilePath" + i.ToString(), string.Empty);
                if (!string.IsNullOrWhiteSpace(file_name))
                {
                    _recentFilesInfos.Add(new FileInfo(file_name));
                }
            }
        }

        // Save the current items in the Registry.
        private void saveRecentFilesList()
        {
            // Delete the saved entries.
            for (int i = 0; i < Constants.MAX_RECENT_FILES; i++)
            {
                RegistryTools.DeleteSetting(Constants.APPLICATION_NAME, "FilePath" + i.ToString());
            }

            // Save the current entries.
            int index = 0;
            foreach (FileInfo file_info in _recentFilesInfos)
            {
                RegistryTools.SaveSetting(Constants.APPLICATION_NAME,
                    "FilePath" + index.ToString(), file_info.FullName);
                index++;
            }
        }

        // Remove a file's info from the list.
        private void removeFileInfo(string file_name)
        {
            // Remove occurrences of the file's information from the list.
            for (int i = _recentFilesInfos.Count - 1; i >= 0; i--)
            {
                if (_recentFilesInfos[i].FullName == file_name) _recentFilesInfos.RemoveAt(i);
            }
        }

        // Add a file to the list, rearranging if necessary.
        public void addRecentFile(string file_name)
        {
            // Remove the file from the list.
            removeFileInfo(file_name);

            // Add the file to the beginning of the list.
            _recentFilesInfos.Insert(0, new FileInfo(file_name));

            // If we have too many items, remove the last one.
            if (_recentFilesInfos.Count > Constants.MAX_RECENT_FILES) _recentFilesInfos.RemoveAt(Constants.MAX_RECENT_FILES);

            // Update the Registry.
            saveRecentFilesList();
        }

        // Remove a file from the list, rearranging if necessary.
        public void removeRecentFile(string file_name)
        {
            // Remove the file from the list.
            removeFileInfo(file_name);

            // Update the Registry.
            saveRecentFilesList();
        }

        #endregion

        #region Open File

        public Task<ProfileSaveFile?> handleFileOpenAsync(string filePath)
        {
            Console.WriteLine("Reading file: {0}", filePath);
            if (Path.GetExtension(filePath) == Constants.DECRYPTED_FILE_EXTENSION)
            {
                return handleJsonFileOpen(filePath);
            }
            else
            {
                return handleDatFileOpen(filePath);
            }
        }

        private Task<ProfileSaveFile?> handleJsonFileOpen(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return tryParseFileStreamAsync(stream);
        }

        private async Task<ProfileSaveFile?> handleDatFileOpen(string filePath)
        {
            var file = new FileInfo(filePath);
            using FileStream inputStream = file.OpenRead();
            bool encrypted = SaveFileHandler.IsFileEncrypted(inputStream);
            if (!encrypted)
            {
                EventLogger.logError($"The file \"{file.Name}\" was in an unexpected format.");
                showError?.Invoke(R.formatFILE_IN_UNEXPECTED_FORMAT_ERROR_MESSAGE(file.Name));
                return null;
            }
            using Stream? processed = await FileProcessHelper.Decrypt(inputStream);
            if (processed == null)
            {
                EventLogger.logError($"Content of file \"{file.Name}\" could not be converted to a supported format.");
                showError?.Invoke(R.formatFILE_DECRYPT_ERROR_MESSAGE(file.Name));
                return null;
            }
            return await tryParseFileStreamAsync(processed!);
        }

        private async Task<ProfileSaveFile?> tryParseFileStreamAsync(Stream stream)
        {
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
                var profile = await ProfileParser.Read(stream);
                if(!profile.isValid())
                {
                    showError?.Invoke(R.CHARACTER_FILE_FORMAT_NOT_RECOGNIZED_ERROR_MESSAGE);
                    return null;
                }
                return profile;
            }
            catch (Exception e)
            {
                EventLogger.logError(e.ToString());
                showError?.Invoke(R.FAILED_TO_PARSE_FILE_ERROR_MESSAGE);
            }
            return null;
        }

        #endregion

        #region Save File

        public async Task handleFileSaveAsync(string? filePath, ProfileSaveFile profile)
        {
            if (filePath == null) { return; }
            profile.TotalGearPower = profile.computeCharacterPower();
            Console.WriteLine("Writing file: {0}", filePath!);
            if (Path.GetExtension(filePath!) == Constants.DECRYPTED_FILE_EXTENSION)
            {
                await handleJsonFileSave(filePath!, profile);
            }
            else
            {
                await handleDatFileSave(filePath!, profile);
            }
        }

        private async Task handleJsonFileSave(string filePath, ProfileSaveFile profile)
        {
            using var stream = await ProfileParser.Write(profile);
            await writeStreamToFileAsync(stream, filePath);
        }

        private async Task handleDatFileSave(string filePath, ProfileSaveFile profile)
        {
            using var inputStream = await ProfileParser.Write(profile);
            inputStream.Seek(0, SeekOrigin.Begin);
            using Stream? processed = await FileProcessHelper.Encrypt(inputStream);
            if (processed == null)
            {
                EventLogger.logError($"Failed to encrypt the json data.");
                showError?.Invoke(R.FAILED_TO_ENCRYPT_ERROR_MESSAGE);
                return;
            }
            await writeStreamToFileAsync(processed, filePath);
        }

        private async Task writeStreamToFileAsync(Stream stream, string filePath)
        {
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, string.Empty);
            }
            using var filestream = new FileStream(filePath, FileMode.Truncate, FileAccess.Write);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(filestream);
        }

        #endregion
    }
}
