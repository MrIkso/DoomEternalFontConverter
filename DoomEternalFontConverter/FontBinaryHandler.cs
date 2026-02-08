using System.Buffers.Binary;
using System.Text;
using System.Text.Json;

namespace DoomEternalFontConverter
{
    public static class FontBinaryHandler
    {
        public static FontInfo Read(string filePath)
        {
            using var stream = File.OpenRead(filePath);
            using var reader = new BinaryReader(stream);
            var font = new FontInfo();

            byte[] headerBuf = reader.ReadBytes(10);
            font.PointSize = BinaryPrimitives.ReadInt16BigEndian(headerBuf.AsSpan(0, 2));
            font.Ascender = BinaryPrimitives.ReadInt16BigEndian(headerBuf.AsSpan(2, 2));
            font.Descender = BinaryPrimitives.ReadInt16BigEndian(headerBuf.AsSpan(4, 2));
            font.EdgeExpand = BinaryPrimitives.ReadInt16BigEndian(headerBuf.AsSpan(6, 2));
            short numGlyphs = BinaryPrimitives.ReadInt16BigEndian(headerBuf.AsSpan(8, 2));

            for (int i = 0; i < numGlyphs; i++)
            {
                font.Glyphs.Add(new GlyphInfo
                {
                    Width = reader.ReadByte(),
                    Height = reader.ReadByte(),
                    Top = reader.ReadSByte(),
                    Left = reader.ReadSByte(),
                    XSkip = reader.ReadByte(),
                    Padding = reader.ReadByte(),
                    X = reader.ReadUInt16(),
                    Y = reader.ReadUInt16()
                });
            }

            for (int i = 0; i < numGlyphs; i++)
            {
                font.Glyphs[i].Char = reader.ReadUInt32();
            }

            if (stream.Position <= stream.Length - 4)
            {
                uint stringSize = reader.ReadUInt32();
                if (stringSize > 0 && stringSize < 1024)
                {
                    byte[] stringBytes = reader.ReadBytes((int)stringSize);
                    font.MaterialName = Encoding.UTF8.GetString(stringBytes).TrimEnd('\0');
                }
            }

            return font;
        }

        public static void Write(string filePath, FontInfo font)
        {
            var orderedGlyphs = font.Glyphs.OrderBy(g => g.Char).ToList();

            using var stream = File.Create(filePath);
            using var writer = new BinaryWriter(stream);

            byte[] buf = new byte[2];
            BinaryPrimitives.WriteInt16BigEndian(buf, font.PointSize); writer.Write(buf);
            BinaryPrimitives.WriteInt16BigEndian(buf, font.Ascender); writer.Write(buf);
            BinaryPrimitives.WriteInt16BigEndian(buf, font.Descender); writer.Write(buf);
            BinaryPrimitives.WriteInt16BigEndian(buf, font.EdgeExpand); writer.Write(buf);
            BinaryPrimitives.WriteInt16BigEndian(buf, (short)orderedGlyphs.Count); writer.Write(buf);

            foreach (var g in orderedGlyphs)
            {
                writer.Write(g.Width);
                writer.Write(g.Height);
                writer.Write(g.Top);
                writer.Write(g.Left);
                writer.Write(g.XSkip);
                writer.Write(g.Padding);
                writer.Write(g.X);
                writer.Write(g.Y);
            }
            foreach (var g in orderedGlyphs)
            {
                writer.Write(g.Char);
            }
            byte[] nameBytes = Encoding.UTF8.GetBytes(font.MaterialName);
            writer.Write((uint)nameBytes.Length);
            writer.Write(nameBytes);
        }

        public static string ToJson(this FontInfo fontInfo) => JsonSerializer.Serialize(fontInfo, new JsonSerializerOptions { WriteIndented = true });

        public static FontInfo? FromJson(this string json)
        {
            var font = JsonSerializer.Deserialize<FontInfo>(json);
            return font;
        }
    }
}
