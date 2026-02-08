using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DoomEternalFontConverter
{
    public class FontProcessor
    {
        public static FontInfo? GenerateFontFromBmFont(string fntFilePath, string materialPath)
        {
            if (!File.Exists(fntFilePath))
            {
                Console.WriteLine($"Error: file {fntFilePath} not found.");
                return null;
            }

            var fontData = new FontInfo();
            fontData.EdgeExpand = 3;
            fontData.MaterialName = materialPath.Replace('\\', '/'); // Ensure consistent path format for Doom Eternal
            
            var parsedGlyphs = new List<GlyphInfo>();

            var fntLines = File.ReadAllLines(fntFilePath, Encoding.UTF8);
            var baseLine = 0;
            foreach (var line in fntLines)
            {
                if (line.StartsWith("info"))
                {
                    
                }
                else if (line.StartsWith("common"))
                {
                    baseLine = GetIntValue(line, "base");
                    fontData.PointSize = (short)GetIntValue(line, "lineHeight");
                    fontData.Ascender = (short)baseLine;
                    fontData.Descender = (short)(baseLine - GetIntValue(line, "lineHeight"));
                }
                else if (line.StartsWith("char ") && !line.StartsWith("chars count"))
                {
                    int charCode = GetIntValue(line, "id");
                    var glyph = new GlyphInfo
                    {
                        Char = (uint)charCode,
                        // Data from BMFont
                        Width = (byte)GetIntValue(line, "width"),
                        Height = (byte)GetIntValue(line, "height"),
                        X = (ushort)GetIntValue(line, "x"),
                        Y = (ushort)GetIntValue(line, "y"),
                        Left = (sbyte)GetIntValue(line, "xoffset"),
                        Top = (sbyte)(baseLine - GetIntValue(line, "yoffset")),
                        XSkip = (byte)GetIntValue(line, "xadvance"),
                        Padding = 0,
                    };
                   
                    parsedGlyphs.Add(glyph);
                }
            }
            if (parsedGlyphs.Count == 0)
            {
                Console.WriteLine("Warning: No glyphs found in the .fnt file.");
                return null;
            }

            //parsedGlyphs.Sort((pair1, pair2) => pair1.Key.CompareTo(pair2.Key));

            fontData.Glyphs = parsedGlyphs;

            Console.WriteLine($"Successfully parsed {fontData.Glyphs.Count} glyphs from file {Path.GetFileName(fntFilePath)}.");

            return fontData;
        }
        private static string? GetValue(string line, string key)
        {
            var match = Regex.Match(line, $@"{key}=""?([^""\s]+)""?");
            return match.Success ? match.Groups[1].Value : null;
        }

        private static int GetIntValue(string line, string key)
        {
            var valueStr = GetValue(line, key);
            return valueStr != null ? int.Parse(valueStr, CultureInfo.InvariantCulture) : 0;
        }

        private static float GetFloatValue(string line, string key)
        {
            var valueStr = GetValue(line, key);
            return valueStr != null ? float.Parse(valueStr, CultureInfo.InvariantCulture) : 0f;
        }
    }
}