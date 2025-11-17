using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace hogs_gameEditor_wpf.FileFormat
{
    public class TAB
    {
        public List<Letter> all_letters = [];
        public List<Bitmap> all_glyphs = [];

        public TAB(byte[] file)
        {
            using MemoryStream ms = new(file);
            using BinaryReader reader = new(ms);
            reader.BaseStream.Position += 16;

            while (ms.Position < ms.Length)
            {
                Letter l = new()
                {
                    XOffset = reader.ReadUInt16(),
                    YOffset = reader.ReadUInt16(),
                    Width = reader.ReadUInt16(),
                    Height = reader.ReadUInt16()
                };
                all_letters.Add(l);
            }
        }

        public class Letter
        {
            public ushort XOffset { get; set; }
            public ushort YOffset { get; set; }
            public ushort Width { get; set; }
            public ushort Height { get; set; }
        }
    }

}
