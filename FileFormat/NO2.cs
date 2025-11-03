using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace hogs_gameEditor_wpf.FileFormat
{
    public class NO2  // model Normal , source : https://github.com/TalonBraveInfo/OpenHoW/blob/master/doc/file-formats/NO2.md
    {
        public List<Normal> normalList;

        public NO2(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                normalList = new List<Normal>();

                while (ms.Position < ms.Length)
                {
                    Normal n = new Normal();

                    n.X = BitConverter.ToInt16(reader.ReadBytes(4)) / 32768f;
                    n.Y = BitConverter.ToInt16(reader.ReadBytes(4)) / 32768f;
                    n.Z = BitConverter.ToInt16(reader.ReadBytes(4)) / 32768f;

                    n.BoneIndex = reader.ReadSingle();

                    //FixNormals(n);

                    normalList.Add(n);
                }
            }
        }

        public void FixNormals(Normal n)
        {
            float len = MathF.Sqrt(n.X * n.X + n.Y * n.Y + n.Z * n.Z);
            if (len > 0.0001f)
            {
                n.X /= len;
                n.Y /= len;
                n.Z /= len;
            }
        }

    }
 

    public class Normal
    {
        public float X;
        public float Y;
        public float Z;
        public float BoneIndex;
        
    }
}
