using System.Collections.Generic;
using System.IO;

namespace hogs_gameEditor_wpf.FileFormat
{
    public class VTX //model verticies : https://github.com/TalonBraveInfo/OpenHoW/blob/master/doc/file-formats/VTX.md
    {
        public List<Vertice> verticesList;

        public VTX(byte[] data)
        {
            using MemoryStream ms = new(data);
            using BinaryReader reader = new(ms);
            verticesList = [];

            while (ms.Position < ms.Length)
            {
                verticesList.Add(new Vertice
                {
                    XOffset = reader.ReadInt16(),
                    YOffset = reader.ReadInt16(),
                    ZOffset = reader.ReadInt16(),
                    BoneIndex = reader.ReadInt16()
                });
            }
        }
    }

    public class Vertice
    {
        public short XOffset;
        public short YOffset;
        public short ZOffset;
        public short BoneIndex;
    }


}
