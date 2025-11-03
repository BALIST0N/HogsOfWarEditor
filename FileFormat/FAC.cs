using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace hogs_gameEditor_wpf.FileFormat
{
    //references : https://github.com/DummkopfOfHachtenduden/how-doc/blob/master/Model/FAC%20-%20Faces.cs

    public class FAC //model faces structure
    {
        public char[] name { get; set; }
        public int triangleCount { get; set; }
        public List<Triangle> triangleList { get; set; }
        public int planeCount { get; set; }
        public List<Plane> planeList { get; set; }

        public List<string> texturesNames { get; set; }

        public FAC(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                this.name = reader.ReadChars(16);
                this.triangleCount = reader.ReadInt32();
                this.triangleList = new List<Triangle>();
                
                for (int i = 0; i < this.triangleCount; i++)
                {
                    triangleList.Add(new Triangle
                    {
                        U_A = reader.ReadByte(),
                        V_A = reader.ReadByte(),
                                     
                        U_B = reader.ReadByte(),
                        V_B = reader.ReadByte(),
                                     
                        U_C = reader.ReadByte(),
                        V_C = reader.ReadByte(),

                        Vertex_A = reader.ReadInt16(),
                        Vertex_B = reader.ReadInt16(),  
                        Vertex_C = reader.ReadInt16(),

                        Normal_A = reader.ReadInt16(),
                        Normal_B = reader.ReadInt16(),
                        Normal_C = reader.ReadInt16(),

                        Short0 = reader.ReadInt16(),

                        TextureIndex = reader.ReadInt32(),

                        Short1 = reader.ReadInt16(),
                        Short2 = reader.ReadInt16(),
                        Short3 = reader.ReadInt16(),
                        Short4 = reader.ReadInt16()

                    });
                }

                this.planeCount = reader.ReadInt32();
                this.planeList = new List<Plane>();
                for (int i = 0; i < this.planeCount; i++)
                {
                    planeList.Add(new Plane
                    {
                        U_A = reader.ReadByte(),
                        V_A = reader.ReadByte(),
                                    
                        U_B = reader.ReadByte(),
                        V_B = reader.ReadByte(),
                                   
                        U_C = reader.ReadByte(),
                        V_C = reader.ReadByte(),
                                 
                        U_D = reader.ReadByte(),
                        V_D = reader.ReadByte(),

                        Vertex_A = reader.ReadInt16(),
                        Vertex_B = reader.ReadInt16(),
                        Vertex_C = reader.ReadInt16(),
                        Vertex_D = reader.ReadInt16(), 

                        Normal_A = reader.ReadInt16(),
                        Normal_B = reader.ReadInt16(),
                        Normal_C = reader.ReadInt16(),
                        Normal_D = reader.ReadInt16(),

                        TextureIndex = reader.ReadInt32(),

                        Short1 = reader.ReadInt16(),
                        Short2 = reader.ReadInt16(),
                        Short3 = reader.ReadInt16(),
                        Short4 = reader.ReadInt16()
                    });
                }

                if (ms.Length != ms.Position)
                {
                    //merci chatGPT encore hein
                    this.texturesNames = Regex.Split(Encoding.UTF8.GetString(data[(int)(ms.Position + 1)..(int)ms.Length]), @"\x00{5,8}")
                                         .Select(s => s.Trim('\0'))       
                                         .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    
                }

            }

        }

        public List<int> GetIndexes()
        {
            return this.triangleList.Select(x => x.TextureIndex).Concat(this.planeList.Select(x => x.TextureIndex)).Distinct().ToList();
        }


        public  class Triangle //32 bytes
        {
            public byte  U_A;             // U component for Vertex_A
            public byte  V_A;             // U component for Vertex_A
                   
            public byte  U_B;             // U component for Vertex_B
            public byte  V_B;             // V component for Vertex_B
                   
            public byte  U_C;             // U component for Vertex_C	
            public byte  V_C;             // V component for Vertex_C

            public short Vertex_A;        // index from .VTX
            public short Vertex_B;        // index from .VTX
            public short Vertex_C;        // index from .VTX

            public short Normal_A;        // index from .NO2
            public short Normal_B;        // index from .NO2
            public short Normal_C;        // index from .NO2

            public short Short0;

            public int TextureIndex;   // index from .MTD

            public short Short1;
            public short Short2;
            public short Short3;
            public short Short4;
        }

        public class Plane //36 bytes
        {
            public byte U_A;                // U component for Vertex_A
            public byte V_A;                // U component for Vertex_A
                   
            public byte U_B;                // U component for Vertex_B
            public byte V_B;                // V component for Vertex_B
                   
            public byte U_C;                // U component for Vertex_C 
            public byte V_C;                // V component for Vertex_C
                   
            public byte U_D;                // U component for Vertex_D
            public byte V_D;                // V component for Vertex_D

            public short Vertex_A;           // index from .VTX
            public short Vertex_B;           // index from .VTX
            public short Vertex_C;           // index from .VTX
            public short Vertex_D;           // index from .VTX

            public short Normal_A;           // index from .NO2
            public short Normal_B;           // index from .NO2
            public short Normal_C;           // index from .NO2
            public short Normal_D;           // index from .NO2    

            public int TextureIndex;       // index from .MTD

            public short Short1;
            public short Short2;
            public short Short3;
            public short Short4;
        }
    }


}