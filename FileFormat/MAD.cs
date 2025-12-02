using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        public List<HIR> skeleton { get; set; }
        public List<MotionCapture> animations { get; set; }
        public List<Mtd> textures { get; set; }



        public MAD(byte[] hexblock)
        {
            Name = Encoding.ASCII.GetString(hexblock[0..16]);
            DataOffset = BitConverter.ToInt32(hexblock, 16);
            DataSize = BitConverter.ToInt32(hexblock, 20);
        }

        public MAD()
        {

        }

        public string GetName()
        {
            return Path.GetFileNameWithoutExtension(Name);
        }

        public static MAD GetModelFromMAD(string modelToFind, string mad) //return VTX NO2 FAC of a model
        {
            string fldr = GlobalVars.mapsFolder + mad + ".MAD";

            MAD model = new()
            {
                DataSizes = new int[3]
            };

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

        public static MAD GetCharacter(string type, POG.PigTeam team)
        {
            //LE_ME = hero
            //AC_ME = legend

            //var list = GetModelListFromMad(GlobalVars.gameFolder + "Chars/british.mad");

            switch (type) //wtf is this mess the names are wong inside the game 
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
                    type = "pcsni_hi";
                    break;

                case "SP_ME":
                    type = "pcsni_hi";
                    break;

                case "SB_ME":
                    type = "pcspy_hi";
                    break;
            }

            MAD mad = GetModelFromFullMAD(type, GlobalVars.gameFolder + "Chars/british.mad");

            mad.textures = Mtd.LoadTexturesFromMTD(mad.facData, GlobalVars.gameFolder + "Chars/"+team+".MTD", true);

            List<Mtd> faces = Mtd.LoadAlltextures(GlobalVars.gameFolder + "Chars/FACES.MTD");

            if( type == "pcsab_hi") //commando
            {
                mad.textures.Find(x => x.Name == "eyes000.tim").textureTim = faces.Find(x => x.Name == "eyes006.tim").textureTim;
                mad.textures.Find(x => x.Name == "gobs000.tim").textureTim = faces.Find(x => x.Name == "gobs007.tim").textureTim;
            }
            else
            {
                mad.textures.Find(x => x.Name == "eyes000.tim").textureTim = faces.Find(x => x.Name == "eyes001.tim").textureTim;
                mad.textures.Find(x => x.Name == "gobs000.tim").textureTim = faces.Find(x => x.Name == "gobs002.tim").textureTim;
            }

            mad.skeleton = HIR.GetSkeletonList();
            mad.animations = MotionCapture.GetMotionCaptureAnimations();
            return mad;
        }


        public static List<string> GetMapEntitiesList(string mapName)
        {
            string fldr = GlobalVars.mapsFolder + mapName + ".MAD";
            List<string> entities = [];

            byte[] mapdata = File.ReadAllBytes(fldr);
            int endContenTable = BitConverter.ToInt32(mapdata, 16); //the first item offset define table content size ! 

            for (int i = 0; i <= endContenTable; i++)
            {
                int endblockContentTable = i + 24;

                if (endblockContentTable <= endContenTable)
                {
                    string a = new string(Encoding.ASCII.GetString(mapdata[i..(i + 16)])).Trim('\0');
                    a = a[..^4]; 
                    if (entities.Contains(a) == false && GlobalVars.entityFilterList.Contains(a) == false)
                    {
                        entities.Add(a);
                    }

                }
                i += 23;
            }

            //need to apply a filter to remove duplicates
            return entities;
        }

        public static List<string> GetMapEntitiesList(string mapName, bool filter = false)
        {
            string fldr = GlobalVars.mapsFolder + mapName + ".MAD";
            List<string> entities = [];

            byte[] mapdata = File.ReadAllBytes(fldr);
            int endContenTable = BitConverter.ToInt32(mapdata, 16); //the first item offset define table content size ! 

            for (int i = 0; i <= endContenTable; i++)
            {
                int endblockContentTable = i + 24;

                if (endblockContentTable <= endContenTable)
                {
                    string a = new string(Encoding.ASCII.GetString(mapdata[i..(i + 16)])).Trim('\0');
                    a = a[..^4];
                    if (entities.Contains(a) == false)
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

            List<string> entities = [];

            byte[] mapdata = File.ReadAllBytes(fldr);
            int endContenTable = BitConverter.ToInt32(mapdata, 16); //the first item offset define table content size ! 

            for (int i = 0; i <= endContenTable; i++)
            {
                int endblockContentTable = i + 24;

                if (endblockContentTable <= endContenTable)
                {
                    string a = new string(Encoding.ASCII.GetString(mapdata[i..(i + 16)])).Trim('\0');
                    a = a[..^4];
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
            MAD model = new();

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
            Dictionary<int, int> indexRealloc = [];
            for (int i = 0; i < textures.Count; i++)
            {
                if (indexRealloc.ContainsKey(textures[i].indexNumber) == false)
                {
                    indexRealloc.Add(textures[i].indexNumber, count);
                    textures[i].indexNumber = count;
                    count++;
                }
                else
                {
                    textures[i].indexNumber = indexRealloc[textures[i].indexNumber];
                }
            }

            foreach (FAC.Triangle t in facData.triangleList)
            {
                t.TextureIndex = indexRealloc[t.TextureIndex];
            }

            foreach (FAC.Plane p in facData.planeList)
            {
                p.TextureIndex = indexRealloc[p.TextureIndex];
            }

        }


        /// <summary>
        /// transform actual vtx,NO2 and Fac into into a byte array
        /// </summary>
        public byte[] GoBackToMonke()
        {
            using MemoryStream ms = new();
            using BinaryWriter writer = new(ms);
            foreach (Vertice vertx in vtxData.verticesList)
            {
                writer.Write(vertx.XOffset);
                writer.Write(vertx.YOffset);
                writer.Write(vertx.ZOffset);
                writer.Write(vertx.BoneIndex);
            }

            if (ms.Position > DataSizes[0])
            {

            }

            foreach (Normal no2 in no2Data.normalList)
            {
                writer.Write(no2.X);
                writer.Write(no2.Y);
                writer.Write(no2.Z);
                writer.Write(no2.BoneIndex);
            }

            if (ms.Position - DataSizes[0] > DataSizes[1])
            {

            }

            writer.Write(facData.name);
            writer.Write(facData.triangleCount);

            foreach (FAC.Triangle t in facData.triangleList)
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

            writer.Write(facData.planeCount);
            foreach (FAC.Plane p in facData.planeList)
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

            if (ms.Position - DataSizes[0] - DataSizes[1] > DataSizes[2])
            {

            }

            return ms.ToArray();

        }


    }


}
