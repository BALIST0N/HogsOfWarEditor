using System;
using System.Collections.Generic;
using System.IO;

namespace hogs_gameEditor_wpf.FileFormat
{
    public class NO2  // model Normal , source : https://github.com/TalonBraveInfo/OpenHoW/blob/master/doc/file-formats/NO2.md
    {
        public List<Normal> normalList;

        public NO2(byte[] data)
        {
            using MemoryStream ms = new(data);
            using BinaryReader reader = new(ms);
            normalList = [];

            while (ms.Position < ms.Length)
            {
                Normal n = new()
                {
                    X = BitConverter.ToInt16(reader.ReadBytes(4)) / 32768f,
                    Y = BitConverter.ToInt16(reader.ReadBytes(4)) / 32768f,
                    Z = BitConverter.ToInt16(reader.ReadBytes(4)) / 32768f,

                    BoneIndex = reader.ReadSingle()
                };

                //FixNormals(n);

                normalList.Add(n);
            }
        }

        public void FixNormals(Normal n)
        {
            float len = MathF.Sqrt((n.X * n.X) + (n.Y * n.Y) + (n.Z * n.Z));
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
