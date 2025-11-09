using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace hogs_gameEditor_wpf.FileFormat
{
    public class TAB
    {
        public List<Letter> all_letters = new List<Letter>();
        public List<Bitmap> all_glyphs = new List<Bitmap>();

        public TAB(byte[] file) 
        {
            using (MemoryStream ms = new MemoryStream(file))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                reader.BaseStream.Position += 16;

                while (ms.Position < ms.Length)
                {
                    Letter l = new Letter();
                    l.XOffset = reader.ReadUInt16();
                    l.YOffset = reader.ReadUInt16();
                    l.Width = reader.ReadUInt16();
                    l.Height = reader.ReadUInt16();
                    this.all_letters.Add(l);
                }

            }
        }

        public  class Letter
        {
            public ushort XOffset   {get;set;}
            public ushort YOffset   {get;set;}
            public ushort Width     {get;set;}
            public ushort Height { get; set; }
        }
    }

}
