using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MCDSaveEdit.Save.Models.Mapping;
#nullable enable

namespace MCDSaveEdit.Save.Models.Profiles
{
    public static class ProfileParser
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions();

        static ProfileParser()
        {
            _options.Converters.Add(new AttributeBasedConverterFactory());
            _options.Converters.Add(new GuidConverterFactory());
            _options.Converters.Add(new JsonStringEnumConverter());
            //_options.Converters.Add(new TextDoubleJsonConverter());
            _options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            _options.WriteIndented = true;
            _options.AllowTrailingCommas = true;
        }

        public static async ValueTask<ProfileSaveFile?> Read(Stream stream)
        {
            using BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, false);
            using MemoryStream sanitized = new MemoryStream();
            int bracketCount = 0;
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                byte b = reader.ReadByte();
                char c = (char)b;
                if (char.IsControl(c))
                {
                    continue;
                }

                // PS4 and Switch files have a bunch of extra duplicate data at the end
                // which makes them invalid json files. This takes the first full JSON
                // object found in the file and drops everything after that.
                if (c == '{')
                {
                    bracketCount++;
                }
                else if (c == '}')
                {
                    bracketCount--;
                    if (bracketCount == 0)
                    {
                        // This is the end of the first full JSON object
                        sanitized.WriteByte(b);
                        break;
                    }
                }

                sanitized.WriteByte(b);
            }

            sanitized.Seek(0, SeekOrigin.Begin);
            return await JsonSerializer.DeserializeAsync<ProfileSaveFile>(sanitized, _options);
        }

        public static async ValueTask<Stream> Write(ProfileSaveFile settings)
        {
            Stream stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, settings, _options);
            return stream;
        }
    }
}
