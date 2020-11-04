using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MCDSaveEdit
{
    public static class StreamExtensions
    {
        public static IEnumerable<string> readAllLines(this Stream stream)
        {
            var reader = new StreamReader(stream);
            var list = new List<string>();
            var whitespaceChars = Environment.NewLine.ToCharArray().Concat(new char[] { ' ' }).ToArray();
            string line = reader.ReadLine()?.Trim(whitespaceChars);
            while (!reader.EndOfStream && line != null)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    list.Add(line);
                }
                line = reader.ReadLine()?.Trim(whitespaceChars);
            }
            return list;
        }

        public static string readAllText(this Stream stream)
        {
            var reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            return text;
        }

        public static IEnumerable<string> readAllLines(this TextReader reader)
        {
            var list = new List<string>();
            var whitespaceChars = Environment.NewLine.ToCharArray().Concat(new char[] { ' ' }).ToArray();
            string line = reader.ReadLine()?.Trim(whitespaceChars);
            while (line != null)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    list.Add(line);
                }
                line = reader.ReadLine()?.Trim(whitespaceChars);
            }
            return list;
        }
    }
}
