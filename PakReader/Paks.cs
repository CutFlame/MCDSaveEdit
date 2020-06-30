using PakReader.Parsers.Objects;
using System.Collections.Generic;
using System.IO;

namespace FModel.Utils
{
    static class Paks
    {
        public static void Merge(Dictionary<string, FPakEntry> tempFiles, out Dictionary<string, FPakEntry> files,
                string mount)
        {
            files = new Dictionary<string, FPakEntry>();
            foreach (FPakEntry entry in tempFiles.Values)
            {
                if (!entry.Name.Contains(".")) continue;
                if (files.ContainsKey(mount + entry.GetPathWithoutExtension()) ||
                    entry.GetExtension().Equals(".uptnl") ||
                    entry.GetExtension().Equals(".uexp") ||
                    entry.GetExtension().Equals(".ubulk"))
                    continue;

                if (entry.IsUE4Package()) // if .uasset
                {
                    if (!tempFiles.ContainsKey(Path.ChangeExtension(entry.Name, ".umap"))) // but not including a .umap
                    {
                        string e = Path.ChangeExtension(entry.Name, ".uexp");
                        FPakEntry uexp = tempFiles.ContainsKey(e) ? tempFiles[e] : null; // add its uexp
                        if (uexp != null)
                            entry.Uexp = uexp;

                        string u = Path.ChangeExtension(entry.Name, ".ubulk");
                        FPakEntry ubulk = tempFiles.ContainsKey(u) ? tempFiles[u] : null; // add its ubulk
                        if (ubulk != null)
                            entry.Ubulk = ubulk;
                        else
                        {
                            string f = Path.ChangeExtension(entry.Name, ".ufont");
                            FPakEntry ufont = tempFiles.ContainsKey(f) ? tempFiles[f] : null; // add its ufont
                            if (ufont != null)
                                entry.Ubulk = ufont;
                        }
                    }
                }
                else if (entry.IsUE4Map()) // if .umap
                {
                    string e = Path.ChangeExtension(entry.Name, ".uexp");
                    string u = Path.ChangeExtension(entry.Name, ".ubulk");
                    FPakEntry uexp = tempFiles.ContainsKey(e) ? tempFiles[e] : null; // add its uexp
                    if (uexp != null)
                        entry.Uexp = uexp;
                    FPakEntry ubulk = tempFiles.ContainsKey(u) ? tempFiles[u] : null; // add its ubulk
                    if (ubulk != null)
                        entry.Ubulk = ubulk;
                }

                files[mount + entry.GetPathWithoutExtension()] = entry;
            }
        }

        public static bool IsFileReadLocked(FileInfo PakFileInfo)
        {
            FileStream stream = null;
            try
            {
                stream = PakFileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

        public static bool IsFileWriteLocked(FileInfo PakFileInfo)
        {
            FileStream stream = null;
            try
            {
                stream = PakFileInfo.Open(FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }
    }
}
