using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MCDSaveEdit.Save.Models.Mapping;

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
            _options.IgnoreNullValues = true;
            _options.WriteIndented = true;
        }

        public static async ValueTask<ProfileSaveFile> Read(Stream stream)
        {
            using BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, false);
            using MemoryStream sanitized = new MemoryStream();
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                byte b = reader.ReadByte();
                if (char.IsControl((char)b))
                {
                    continue;
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
