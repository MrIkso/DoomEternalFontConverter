namespace DoomEternalFontConverter
{
    public class FontInfo
    {
        public short PointSize { get; set; }
        public short Ascender { get; set; }
        public short Descender { get; set; }
        public short EdgeExpand { get; set; }
        public List<GlyphInfo> Glyphs { get; set; } = new();
        public string MaterialName { get; set; } = string.Empty;

    }
}
