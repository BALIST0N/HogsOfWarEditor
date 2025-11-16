using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace hogs_gameEditor_wpf.FileFormat
{
    public class MAD
    {
        public string Name { get; set; }  //[16]
        public int DataOffset { get; set; }  // 4 bytes 
        public int DataSize { get; set; }  // 4 bytes
        public int[] DataSizes { get; set; }
        public FAC facData { get; set; }
        public VTX vtxData { get; set; }
        public NO2 no2Data { get; set; }

        public List<HIR> skeleton{ get; set; }
        public List<MotionCapture> animations {  get; set; }
        public List<Mtd> textures {  get; set; }



        public MAD(byte[] hexblock)
        {
            this.Name = Encoding.ASCII.GetString(hexblock[0..16]);
            this.DataOffset = BitConverter.ToInt32(hexblock, 16);
            this.DataSize = BitConverter.ToInt32(hexblock, 20);
        }

        public MAD()
        {

        }

        public string GetName()
        {
            return Path.GetFileNameWithoutExtension(this.Name) ;
        }

        public static MAD GetModelFromMAD(string modelToFind, string mad) //return VTX NO2 FAC of a model
        {
            string fldr = GlobalVars.mapsFolder + mad + ".MAD";

            MAD model = new MAD();
            model.DataSizes = new int[3];

            byte[] mapdata = File.ReadAllBytes(fldr);
            int endContenTable = BitConverter.ToInt32(mapdata, 16); //the first item offset define table content size ! 

            for (int i = 0; i <= endContenTable; i++)
            {
                int endblockContentTable = i + 24;

                if (endblockContentTable <= endContenTable)
                {
                    string nameInsideMad = Encoding.ASCII.GetString(mapdata[i..(i + 16)]).TrimEnd();
                    if (nameInsideMad.Contains(modelToFind))
                    {
                        model.Name = modelToFind;

                        int DataOffset = BitConverter.ToInt32(mapdata[(i + 16)..(i + 20)]);
                        int DataSize = BitConverter.ToInt32(mapdata[(i + 20)..(i + 24)]);

                        int endDataBlock = DataOffset + DataSize;

                        switch (nameInsideMad)
                        {
                            case string a when a.Contains(".FAC"):
                                try
                                {
                                    model.facData = new FAC(mapdata[DataOffset..endDataBlock]);
                                    model.DataSizes[2] = DataSize;
                                }
                                catch { }
                                break;

                            case string b when b.Contains(".VTX"):
                                model.vtxData = new VTX(mapdata[DataOffset..endDataBlock]);
                                model.DataSizes[0] = DataSize;
                                break;

                            case string c when c.Contains(".NO2"):
                                model.no2Data = new NO2(mapdata[DataOffset..endDataBlock]);
                                model.DataSizes[1] = DataSize;
                                break;
                        }
                    }

                }
                i += 23;
            }

            return model;
            
        }

        public static MAD GetCharacter(string type,string team,List<HIR> skeleton, List<MotionCapture> anims)
        {
            //LE_ME = hero
            //AC_ME = legend

            //var list = GetModelListFromMad(GlobalVars.gameFolder + "Chars/british.mad");

            switch(type) //wtf is this mess the names are wong inside the game 
            {
                case "AC_ME":
                    type = "pcace_hi";
                    break;

                case "CO_ME":
                    type = "pcsab_hi";
                    break;

                case "GR_ME":
                    type = "pcgru_hi";
                    break;

                case "HV_ME":
                    type = "pchvy_hi";
                    break;

                case "LE_ME":
                    type = "pcleg_hi";
                    break;

                case "ME_ME":
                    type = "pcmed_hi";
                    break;

                case "SA_ME":
                    type = "pcsap_hi";  
                    break;

                case "SN_ME":
                    type = "pcgru_hi";
                    break;

                case "SP_ME":
                    type = "pcsni_hi";
                    break;

                case "SB_ME":
                    type = "pcspy_hi";
                    break;
            }

            MAD mad = GetModelFromFullMAD(type, GlobalVars.gameFolder + "Chars/british.mad");

            mad.skeleton = skeleton;
            mad.animations = anims;
            return mad;
        }


        public static List<string> GetMapEntitiesList(string mapName)
        {
            string fldr = GlobalVars.mapsFolder + mapName + ".MAD";
            List<string> entities = new List<string>();

            byte[] mapdata = File.ReadAllBytes(fldr);
            int endContenTable = BitConverter.ToInt32(mapdata, 16); //the first item offset define table content size ! 

            for (int i = 0; i <= endContenTable; i++)
            {
                int endblockContentTable = i + 24;

                if (endblockContentTable <= endContenTable)
                {
                    string a = new String(Encoding.ASCII.GetString(mapdata[i..(i + 16)])).Trim('\0');
                    a = a.Substring(0, a.Length - 4);
                    if(entities.Contains(a) == false && GlobalVars.entityFilterList.Contains(a) == false) 
                    {
                        entities.Add(a);
                    }
                        
                }
                i += 23;
            }
            
            //need to apply a filter to remove dupplicates
            return entities;
        }

        public static List<string> GetMapEntitiesList(string mapName,bool filter = false)
        {
            string fldr = GlobalVars.mapsFolder + mapName + ".MAD";
            List<string> entities = new List<string>();

            byte[] mapdata = File.ReadAllBytes(fldr);
            int endContenTable = BitConverter.ToInt32(mapdata, 16); //the first item offset define table content size ! 

            for (int i = 0; i <= endContenTable; i++)
            {
                int endblockContentTable = i + 24;

                if (endblockContentTable <= endContenTable)
                {
                    string a = new String(Encoding.ASCII.GetString(mapdata[i..(i + 16)])).Trim('\0');
                    a = a.Substring(0, a.Length - 4);
                    if (entities.Contains(a) == false )
                    {
                        entities.Add(a);
                    }

                }
                i += 23;
            }
            
            return entities;
        }

        public static List<string> GetModelListFromMad(string fldr)
        {

            List<string> entities = new List<string>();

            byte[] mapdata = File.ReadAllBytes(fldr);
            int endContenTable = BitConverter.ToInt32(mapdata, 16); //the first item offset define table content size ! 

            for (int i = 0; i <= endContenTable; i++)
            {
                int endblockContentTable = i + 24;

                if (endblockContentTable <= endContenTable)
                {
                    string a = new String(Encoding.ASCII.GetString(mapdata[i..(i + 16)])).Trim('\0');
                    a = a.Substring(0, a.Length - 4);
                    if (entities.Contains(a) == false)
                    {
                        entities.Add(a);
                    }

                }
                i += 23;
            }

            return entities;
        }

        public static MAD GetModelFromFullMAD(string modelToFind, string fldr) //return VTX NO2 FAC of a model
        {
            MAD model = new MAD();

            byte[] mapdata = File.ReadAllBytes(fldr);

            int endContenTable = BitConverter.ToInt32(mapdata, 16); //the first item offset define table content size ! 

            for (int i = 0; i <= endContenTable; i++)
            {
                int endblockContentTable = i + 24;

                if (endblockContentTable <= endContenTable)
                {
                    string nameInsideMad = Encoding.ASCII.GetString(mapdata[i..(i + 16)]).TrimEnd();
                    if (nameInsideMad.Contains(modelToFind))
                    {
                        model.Name = modelToFind;

                        int DataOffset = BitConverter.ToInt32(mapdata[(i + 16)..(i + 20)]);
                        int DataSize = BitConverter.ToInt32(mapdata[(i + 20)..(i + 24)]);

                        int endDataBlock = DataOffset + DataSize;

                        switch (nameInsideMad)
                        {
                            case string a when a.Contains(".FAC"):
                            case string a2 when a2.Contains(".fac"):
                                try
                                {
                                    model.facData = new FAC(mapdata[DataOffset..endDataBlock]);
                                }
                                catch { }
                                break;

                            case string b when b.Contains(".VTX"):
                            case string b2 when b2.Contains(".vtx"):
                                model.vtxData = new VTX(mapdata[DataOffset..endDataBlock]);
                                break;

                            case string c when c.Contains(".NO2"):
                            case string c2 when c2.Contains(".no2"):
                                model.no2Data = new NO2(mapdata[DataOffset..endDataBlock]);
                                break;
                        }
                    }

                }
                i += 23;
            }

            return model;
            
        }


        /// <summary>
        /// reindex from 0 all the textures, also modify fac indexes according to new textures indexes 
        /// </summary>
        public void ReIndexFacWithTextures()
        {
            int count = 0;
            Dictionary<int, int> indexRealloc = new Dictionary<int, int>();
            for (int i = 0; i < this.textures.Count; i++)
            {
                if (indexRealloc.ContainsKey(this.textures[i].indexNumber) == false)
                {
                    indexRealloc.Add(this.textures[i].indexNumber, count);
                    this.textures[i].indexNumber = count;
                    count++;
                }
                else
                {
                    this.textures[i].indexNumber = indexRealloc[this.textures[i].indexNumber];
                }
            }

            foreach( FAC.Triangle t in this.facData.triangleList)
            {
                t.TextureIndex = indexRealloc[t.TextureIndex];
            }

            foreach (FAC.Plane p in this.facData.planeList)
            {
                p.TextureIndex = indexRealloc[p.TextureIndex];
            }

        }


        /// <summary>
        /// transform actual vtx,NO2 and Fac into into a byte array
        /// </summary>
        public byte[] GoBackToMonke()
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                foreach (Vertice vertx in this.vtxData.verticesList)
                {
                    writer.Write(vertx.XOffset);
                    writer.Write(vertx.YOffset);
                    writer.Write(vertx.ZOffset);
                    writer.Write(vertx.BoneIndex);
                }

                if(ms.Position > this.DataSizes[0] )
                {

                }
                
                foreach (Normal no2 in this.no2Data.normalList)
                {
                    writer.Write(no2.X);
                    writer.Write(no2.Y);
                    writer.Write(no2.Z);
                    writer.Write(no2.BoneIndex);
                }

                if (ms.Position - this.DataSizes[0] > this.DataSizes[1] )
                {

                }

                writer.Write(this.facData.name);
                writer.Write(this.facData.triangleCount);

                foreach (FAC.Triangle t in this.facData.triangleList)
                {
                    writer.Write(t.U_A);
                    writer.Write(t.V_A);
                    writer.Write(t.U_B);
                    writer.Write(t.V_B);
                    writer.Write(t.U_C);
                    writer.Write(t.V_C);
                    writer.Write(t.Vertex_A);
                    writer.Write(t.Vertex_B);
                    writer.Write(t.Vertex_C);
                    writer.Write(t.Normal_A);
                    writer.Write(t.Normal_B);
                    writer.Write(t.Normal_C);
                    writer.Write(t.Short0);
                    writer.Write(t.TextureIndex);
                    writer.Write(t.Short1);
                    writer.Write(t.Short2);
                    writer.Write(t.Short3);
                    writer.Write(t.Short4);

                }

                writer.Write(this.facData.planeCount);
                foreach (FAC.Plane p in this.facData.planeList)
                {
                    writer.Write(p.U_A);
                    writer.Write(p.V_A);
                    writer.Write(p.U_B);
                    writer.Write(p.V_B);
                    writer.Write(p.U_C);
                    writer.Write(p.V_C);
                    writer.Write(p.U_D);
                    writer.Write(p.V_D);
                    writer.Write(p.Vertex_A);
                    writer.Write(p.Vertex_B);
                    writer.Write(p.Vertex_C);
                    writer.Write(p.Vertex_D);
                    writer.Write(p.Normal_A);
                    writer.Write(p.Normal_B);
                    writer.Write(p.Normal_C);
                    writer.Write(p.Normal_D);
                    writer.Write(p.TextureIndex);
                    writer.Write(p.Short1);
                    writer.Write(p.Short2);
                    writer.Write(p.Short3);
                    writer.Write(p.Short4);
                }

                if (ms.Position - this.DataSizes[0] - this.DataSizes[1] > this.DataSizes[2] )
                {

                }

                return ms.ToArray();
            }

        }


    }


}
