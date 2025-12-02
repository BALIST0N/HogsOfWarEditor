using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
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
        public short[] position { get; set; }    // position in the world (0 = x, 1 = y, 2 = z) [3]
        public short index { get; set; }    // Id of the object on the map 
        public short[] angles { get; set; }    // angles in the world , expressed in & 2^12 (4096 = 360°) [3]
        public short type { get; set; }    //looks like its the skin/model number , also related to rank (a hv_me with type 3 = pyrotech)
        public short[] bounds { get; set; }    // collision bounds [3]
        public short bounds_type { get; set; }    // box, prism, sphere and none
        public short short0 { get; set; }    // bit14 is a "isIndestructible" flag and ignore byte0 value,
                                             // very rare times there is bit 10,11 and 12 that are unchecked 
                                             // bits 1 to 8 are never used (always 0)
        public byte byte0 { get; set; }    //health of the entity, 255 = default , otherwise 100 = 100hp (works wor walls, bridge buildings)
        public PigTeam team { get; set; }    // uk, usa, german, french, japanese, soviet
        public ScriptType objective { get; set; }
        public byte ScriptGroup { get; set; }    //script unique id (nothing related with this.index)
        public byte[] ScriptParameters { get; set; }    //[19] , only the two first bytes are used ! (what a waste of space for a binary file !)
        public short[] fallback_position { get; set; }    // X,Y,Z [3]
        public objectiveFlagEnum objectiveFlag { get; set; }
        public short short1 { get; set; }    //the number of turns (pig change) before the entity get dropped on the map. -> delayed flag must be active ! 
        public short short2 { get; set; }   //unused anywhere, always 0  

        #region DisplayProperty
        // Propriété uniquement pour le PropertyGrid
        [Browsable(true)]
        [DisplayName("Script Parameters 1")]
        public byte ScriptParametersPreview1
        {
            get => ScriptParameters[0];
            set
            {
                ScriptParameters[0] = value;
            }
        }

        [Browsable(true)]
        [DisplayName("Script Parameters 2")]
        public byte ScriptParametersPreview2
        {
            get => ScriptParameters[1];
            set
            {
                ScriptParameters[1] = value;
            }
        }

        [Browsable(true)]
        [DisplayName("Position X")]
        public short PositionX
        {
            get => position != null && position.Length > 0 ? position[0] : (short)0;
            set
            {
                if (position == null || position.Length < 3)
                    position = new short[3];
                position[0] = value;
            }
        }

        [Browsable(true)]
        [DisplayName("Position Y")]
        public short PositionY
        {
            get => position != null && position.Length > 1 ? position[1] : (short)0;
            set
            {
                if (position == null || position.Length < 3)
                    position = new short[3];
                position[1] = value;
            }
        }

        [Browsable(true)]
        [DisplayName("Position Z")]
        public short PositionZ
        {
            get => position != null && position.Length > 2 ? position[2] : (short)0;
            set
            {
                if (position == null || position.Length < 3)
                    position = new short[3];
                position[2] = value;
            }
        }


        [Browsable(true)]
        [DisplayName("Angle X")]
        public short AngleX
        {
            get => angles != null && angles.Length > 0 ? angles[0] : (short)0;
            set
            {
                if (angles == null || angles.Length < 3)
                    angles = new short[3];
                angles[0] = value;
            }
        }

        [Browsable(true)]
        [DisplayName("Angle Y")]
        public short AngleY
        {
            get => angles != null && angles.Length > 1 ? angles[1] : (short)0;
            set
            {
                if (angles == null || angles.Length < 3)
                    angles = new short[3];
                angles[1] = value;
            }
        }

        [Browsable(true)]
        [DisplayName("Angle Z")]
        public short AngleZ
        {
            get => angles != null && angles.Length > 2 ? angles[2] : (short)0;
            set
            {
                if (angles == null || angles.Length < 3)
                    angles = new short[3];
                angles[2] = value;
            }
        }

        [Browsable(true)]
        [DisplayName("Fallback Pos X")]
        public short FallbackPositionX
        {
            get => fallback_position != null && fallback_position.Length > 0 ? fallback_position[0] : (short)0;
            set
            {
                if (fallback_position == null || fallback_position.Length < 3){ fallback_position = new short[3]; }
                fallback_position[0] = value;
            }
        }

        [Browsable(true)]
        [DisplayName("Fallback Pos Y")]
        public short FallbackPositionY
        {
            get => fallback_position != null && fallback_position.Length > 1 ? fallback_position[1] : (short)0;
            set
            {
                if (fallback_position == null || fallback_position.Length < 3) { fallback_position = new short[3]; }
                fallback_position[1] = value;
            }
        }

        [Browsable(true)]
        [DisplayName("Fallback Pos Z")]
        public short FallbackPositionZ
        {
            get => fallback_position != null && fallback_position.Length > 2 ? fallback_position[2] : (short)0;
            set
            {
                if (fallback_position == null || fallback_position.Length < 3){ fallback_position = new short[3]; }
                fallback_position[2] = value;
            }
        }
        #endregion

        public POG(byte[] hexblock)
        {
            position = new short[3];
            angles = new short[3];
            bounds = new short[3];
            ScriptParameters = new byte[19];
            fallback_position = new short[3];

            using BinaryReader reader = new(new MemoryStream(hexblock));
            name = reader.ReadChars(16);
            unused0 = reader.ReadChars(16);

            position[0] = reader.ReadInt16();
            position[1] = reader.ReadInt16();
            position[2] = reader.ReadInt16();

            index = reader.ReadInt16();
            angles[0] = reader.ReadInt16();
            angles[1] = reader.ReadInt16();
            angles[2] = reader.ReadInt16();

            type = reader.ReadInt16();

            bounds[0] = reader.ReadInt16();
            bounds[1] = reader.ReadInt16();
            bounds[2] = reader.ReadInt16();

            bounds_type = reader.ReadInt16();
            byte0 = reader.ReadByte();

            short0 = reader.ReadInt16();

            team = (PigTeam)reader.ReadByte();
            objective = (ScriptType)reader.ReadInt16();
            ScriptGroup = reader.ReadByte();
            ScriptParameters = reader.ReadBytes(19);

            fallback_position[0] = reader.ReadInt16();
            fallback_position[1] = reader.ReadInt16();
            fallback_position[2] = reader.ReadInt16();

            short flags = reader.ReadInt16();
            objectiveFlag = flags == 0 ? 0 : (objectiveFlagEnum)flags;

            short1 = reader.ReadInt16();
            short2 = reader.ReadInt16();

        }

        public POG()
        {

        }

        public string GetName()
        {
            return GlobalVars.Name_Converter(name);
        }

        public static char[] NameToCharArray(string s)
        {
            return s.PadRight(16, '\0').ToCharArray();
        }

        public void SetName(string s)
        {
            name = s.PadRight(16, '\0').ToCharArray();
        }


        public static List<POG> GetAllMapObject(string mapName)
        {
            List<POG> mapEntities = [];

            byte[] mapdata = File.ReadAllBytes(GlobalVars.mapsFolder + mapName + ".POG");
            ushort blocks = BitConverter.ToUInt16(mapdata, 0); //get number of map objects

            for (int i = 1; i <= blocks; i++)
            {
                int endblock = (i * 94) + 2;
                int startblock = endblock - 94; //a map object is 94 bytes, so every 94 bytes, cut and create a mapobject

                if (endblock < mapdata.Length)  //if this is not the end of file
                {
                    mapEntities.Add(new POG(mapdata[startblock..endblock]));
                }
            }

            return mapEntities;
        }


        public static void ExportMapPOG(List<POG> mapEntities, string mapName)
        {
            string path = GlobalVars.mapsFolder + mapName + ".POG";

            using MemoryStream ms = new();
            using BinaryWriter bw = new(ms, Encoding.ASCII, leaveOpen: true);

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
            using MemoryStream ms = new();
            using BinaryWriter bw = new(ms, Encoding.ASCII, leaveOpen: true);

            bw.Write(Encoding.ASCII.GetBytes(name));
            bw.Write(Encoding.ASCII.GetBytes(unused0));

            bw.Write(position[0]);
            bw.Write(position[1]);
            bw.Write(position[2]);

            bw.Write(index);

            bw.Write(angles[0]);
            bw.Write(angles[1]);
            bw.Write(angles[2]);

            bw.Write(type);

            bw.Write(bounds[0]);
            bw.Write(bounds[1]);
            bw.Write(bounds[2]);

            bw.Write(bounds_type);
            bw.Write(byte0);
            bw.Write(short0);
            bw.Write((byte)team);

            bw.Write((short)objective);

            bw.Write(ScriptGroup);
            bw.Write(ScriptParameters);

            bw.Write(fallback_position[0]);
            bw.Write(fallback_position[1]);
            bw.Write(fallback_position[2]);

            bw.Write((short)objectiveFlag);

            bw.Write(short1);
            bw.Write(short2);

            bw.Flush();
            return ms.ToArray();
        }

        public object POG2JSON()
        {
            return new
            {
                ID = index,
                name = GlobalVars.Name_Converter(name),
                unused0 = GlobalVars.Name_Converter(unused0),
                position,
                angles,
                type,
                bounds,
                bounds_type,
                short0,
                byte0,
                team = (short)team,
                objective = (short)objective,
                ScriptGroup,
                ScriptParameters = ScriptParameters.Select(x => (byte)x).ToArray(),
                fallback_position,
                objectiveFlag = (short)objectiveFlag,
                short1,
                short2
            };
        }

        public enum PigTeam : byte
        {
            // PigTeam:
            british = 0x01,
            FRENCH = 0x02,
            AMERICAN = 0x04,
            RUSSIAN = 0x08,
            JAPANESE = 0x10,
            GERMAN = 0x20,
            TEAMLARD = 0x40,
            unused = 0x80
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
        public enum objectiveFlagEnum
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
    public class Vec3s
    {
        public short X { get; set; }
        public short Y { get; set; }
        public short Z { get; set; }
    }

}
