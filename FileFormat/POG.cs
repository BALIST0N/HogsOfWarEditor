using hogs_gameManager_wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Windows.ApplicationModel.Store;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace hogs_gameEditor_wpf.FileFormat
{
    [CategoryOrder("General", 1)]
    [CategoryOrder("Script", 2)]
    [CategoryOrder("unused", 3)]
    public class POG // .pog file
    {
        //new reference  : https://github.com/TalonBraveInfo/OpenHoW/blob/master/doc/file-formats/POG.md

        public char[] name { get; set; }    // entity name [16]
        public char[] unused0 { get; set; }    //NULL[16]
        public short[] position { get; set; }    // position in the world (0 = x, 1 = z, 2 = y) [3]
        public short index { get; set; }    // Id of the object on the map 
        public short[] angles { get; set; }    // angles in the world , expressed in & 2^12 (4096 = 360°) [3]
        public short type { get; set; }    //looks like its the skin/model number , also related to rank (a hv_me with type 3 = pyrotech)
        public short[] bounds { get; set; }    // collision bounds [3]
        public short bounds_type { get; set; }    // box, prism, sphere and none
        public short short0 { get; set; }    // bit14 is a "isIndestructible" flag and ignore byte0 value,
                                             // very rare times there is bit 10,11 and 12 that are unchecked 
                                             // bits 1 to 8 are never used (always 0)
        public byte byte0 { get; set; }    //health of the entity, 255 = 100% , otherwise 100 = 100hp (works wor walls, bridge buildings)
        public PigTeam team { get; set; }    // uk, usa, german, french, japanese, soviet
        public ScriptType objective { get; set; }
        public byte ScriptGroup { get; set; }    //script unique id (nothing related with this.index)
        public byte[] ScriptParameters { get; set; }    //[19] , only the two first bytes are used ! (what a waste of space for a binary file !)
        public short[] fallback_position { get; set; }    // X,Y,Z [3]
        public objectiveFlagEnum objectiveFlag { get; set; }
        public short short1 { get; set; }    //the number of turns (pig change) before the entity get dropped on the map. -> delayed flag must be active ! 
        public short short2 { get; set; }   //unused anywhere, always 0  


        // Propriété uniquement pour le PropertyGrid
        [Browsable(true)]
        [DisplayName("Script Parameters")]
        public byte[] ScriptParametersPreview
        {
            get => ScriptParameters.Take(2).ToArray();
            set
            {
                if (value.Length >= 2)
                {
                    ScriptParameters[0] = value[0];
                    ScriptParameters[1] = value[1];
                }
            }
        }



        public POG(byte[] hexblock)
        {
            this.position = new short[3];
            this.angles = new short[3];
            this.bounds = new short[3];
            this.ScriptParameters = new byte[19];
            this.fallback_position = new short[3];

            using (BinaryReader reader = new BinaryReader(new MemoryStream(hexblock)))
            {
                this.name = reader.ReadChars(16);
                this.unused0 = reader.ReadChars(16);

                this.position[0] = reader.ReadInt16();
                this.position[1] = reader.ReadInt16();
                this.position[2] = reader.ReadInt16();

                this.index = reader.ReadInt16();
                this.angles[0] = reader.ReadInt16();
                this.angles[1] = reader.ReadInt16();
                this.angles[2] = reader.ReadInt16();

                this.type = reader.ReadInt16();

                this.bounds[0] = reader.ReadInt16();
                this.bounds[1] = reader.ReadInt16();
                this.bounds[2] = reader.ReadInt16();

                this.bounds_type = reader.ReadInt16();
                this.byte0 = reader.ReadByte();

                this.short0 = reader.ReadInt16();

                this.team = (PigTeam)reader.ReadByte();
                this.objective = (ScriptType)reader.ReadInt16();
                this.ScriptGroup = reader.ReadByte();
                this.ScriptParameters = reader.ReadBytes(19);

                this.fallback_position[0] = reader.ReadInt16();
                this.fallback_position[1] = reader.ReadInt16();
                this.fallback_position[2] = reader.ReadInt16();
                
                short flags = reader.ReadInt16();
                this.objectiveFlag = flags == 0 ? 0 : (objectiveFlagEnum)flags;

                this.short1 = reader.ReadInt16();
                this.short2 = reader.ReadInt16();

            }

        }

        public POG()
        {

        }

        public string GetName()
        {
            return GlobalVars.Name_Converter(this.name);
        }

        public static char[] NameToCharArray(string s)
        {
            return  s.PadRight(16,'\0').ToCharArray();
        }

        public void SetName(string s)
        {
            this.name = s.PadRight(16, '\0').ToCharArray();
        }


        public static List<POG> GetAllMapObject(string mapName)
        {
            List<POG> mapEntities = new List<POG>();

            byte[] mapdata = File.ReadAllBytes(GlobalVars.mapsFolder + mapName + ".POG");
            ushort blocks = BitConverter.ToUInt16(mapdata, 0); //get number of map objects

            for (int i = 1; i <= blocks; i++)
            {
                int endblock = i * 94 + 2;
                int startblock = endblock - 94; //a map object is 94 bytes, so every 94 bytes, cut and create a mapobject

                if (endblock < mapdata.Length)  //if this is not the end of file
                {
                    mapEntities.Add(new POG(mapdata[startblock..endblock]));
                }
            }
            
            return mapEntities;
        }


        public static void ExportMapPOG(List<POG> mapEntities,string mapName)
        {
            string path = GlobalVars.mapsFolder + mapName + "_edited.POG";

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms, Encoding.ASCII, leaveOpen: true);

            // Écrire le nombre d'entités
            bw.Write((ushort)mapEntities.Count);

            // Écrire chaque entité POG directement
            foreach (POG mo in mapEntities)
            {
                bw.Write(mo.ConvertToByteArray());
            }

            // Écrire le marqueur de fin (0, 0)
            bw.Write((ushort)0);

            // Sauvegarder le flux dans le fichier
            File.WriteAllBytes(path, ms.ToArray());
        }

        public byte[] ConvertToByteArray()
        {
            using MemoryStream ms = new MemoryStream();
            using BinaryWriter bw = new BinaryWriter(ms, Encoding.ASCII, leaveOpen: true);

            bw.Write(Encoding.ASCII.GetBytes(this.name));
            bw.Write(Encoding.ASCII.GetBytes(this.unused0));

            bw.Write(this.position[0]);
            bw.Write(this.position[1]);
            bw.Write(this.position[2]);

            bw.Write(this.index);

            bw.Write(this.angles[0]);
            bw.Write(this.angles[1]);
            bw.Write(this.angles[2]);

            bw.Write(this.type);

            bw.Write(this.bounds[0]);
            bw.Write(this.bounds[1]);
            bw.Write(this.bounds[2]);

            bw.Write(this.bounds_type);
            bw.Write(this.byte0);
            bw.Write((short)this.short0);
            bw.Write((byte)this.team);

            bw.Write((short)this.objective);

            bw.Write(this.ScriptGroup);
            bw.Write(this.ScriptParameters);

            bw.Write(this.fallback_position[0]);
            bw.Write(this.fallback_position[1]);
            bw.Write(this.fallback_position[2]);

            bw.Write((short)this.objectiveFlag);

            bw.Write(this.short1);
            bw.Write(this.short2);

            bw.Flush();
            return ms.ToArray();
        }

        public object POG2JSON() => new
        {
            ID = this.index,
            name = GlobalVars.Name_Converter(this.name),
            unused0 = GlobalVars.Name_Converter(this.unused0),
            position = this.position,
            angles = this.angles,
            type = this.type,
            bounds = this.bounds,
            bounds_type = this.bounds_type,
            short0 = this.short0,
            byte0 = this.byte0,
            team = (short)this.team,
            objective = (short)this.objective,
            ScriptGroup = this.ScriptGroup,
            ScriptParameters = this.ScriptParameters.Select(x => (short)x).ToArray(),
            fallback_position = this.fallback_position,
            objectiveFlag = (short)this.objectiveFlag,
            short1 = this.short1,
            short2 = this.short2
        };




        public enum PigTeam : byte
        {
            // PigTeam:
            Team01 = 0x01,
            Team02 = 0x02,
            Team03 = 0x04,
            Team04 = 0x08,
            Team05 = 0x10,
            Team06 = 0x20,
            Team07 = 0x40,
            Team08 = 0x80
        }

        public enum ScriptType : short
        {
            // ScriptType:
            None = 00,
            DESTROY_ITEM = 01,
            DESTROY_PROPOINT = 02,
            PROTECT_ITEM = 03,
            PROTECT_PROPOINT = 04,

            DROPZONE_ITEM = 07,
            DROPZONE_PROPOINT = 08,

            DESTROY_GROUP_ITEM = 13,
            DESTROY_GROUP_PROPOINT = 14,

            PICKUP_ITEM = 19,
            TUTORIAL_DESTROY = 20,
            TUTORIAL_END = 21,
            TUTORIAL_BLAST = 22,
            TUTORIAL_DESTROY_GROUP = 23
        }

        [Flags]
        public enum objectiveFlagEnum : short
        {
            Player = 1 << 0,    // 0000 0001
            Bit1 = 1 << 1,      // 0000 0010
            Bit2 = 1 << 2,      // 0000 0100
            Bit3 = 1 << 3,      // 0000 1000
            ScriptObj = 1 << 4, // 0001 0000
            Inside = 1 << 5,    // 0010 0000
            Delayed = 1 << 6,   // 0100 0000
            Bit7 = 1 << 7       // 1000 0000
        }



    }
}
