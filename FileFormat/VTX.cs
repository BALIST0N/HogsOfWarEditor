using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hogs_gameEditor_wpf.FileFormat
{
    public class VTX //model verticies : https://github.com/TalonBraveInfo/OpenHoW/blob/master/doc/file-formats/VTX.md
    {
        public List<Vertice> verticesList;

        public VTX(byte[] data )
        {
            using (MemoryStream ms = new MemoryStream(data))
            using(BinaryReader reader = new BinaryReader(ms)) 
            {
                verticesList = new List<Vertice>();

                while(ms.Position < ms.Length)
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
    }

    public class Vertice
    {
        public short XOffset;
        public short YOffset;
        public short ZOffset;
        public short BoneIndex;
    }


}
