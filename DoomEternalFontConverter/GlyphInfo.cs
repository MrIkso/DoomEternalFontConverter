namespace DoomEternalFontConverter
{
    public class GlyphInfo
    {
        public uint Char { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public sbyte Top { get; set; }
        public sbyte Left { get; set; }
        public byte XSkip { get; set; }
        public byte Padding { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }

        public override string ToString()
        {
            return $"Pos:(X:{X},Y:{Y}), Size:(WxH):{Width}x{Height}, XOffset: {Left}, YOffset: {Top}, XAdvance:{XSkip}, Padding:{Padding} ";
        }
    }
}
