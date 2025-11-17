using System.Drawing;
using System.IO;



namespace hogs_gameEditor_wpf.FileFormat
{
    /// <summary>
    /// class conversion from .pmg file into a c# object&
    /// </summary>
    public class PMG // Map Mesh data
    {
        //PMG structure reference : https://github.com/TalonBraveInfo/OpenHoW/blob/master/doc/file-formats/PMG.md

        public Block[,] blocks = new Block[16, 16];

        public PMG(string filepath)
        {
            using MemoryStream ms = new(File.ReadAllBytes(filepath + ".PMG"));
            using BinaryReader reader = new(ms); //using a binaryreader make the offset advance by itself

            while (ms.Position < ms.Length)
            {
                for (int yBlock = 0; yBlock < 16; yBlock++)
                {
                    for (int xBlock = 0; xBlock < 16; xBlock++)
                    {
                        Block b = new()
                        {
                            XOffset = reader.ReadInt16(),
                            YOffset = reader.ReadInt16(),
                            ZOffset = reader.ReadInt16(),
                            Field0 = reader.ReadInt16()
                        };

                        for (int yVertex = 0; yVertex < 5; yVertex++)
                        {
                            for (int xVertex = 0; xVertex < 5; xVertex++)
                            {
                                Vertex v = new()
                                {
                                    Height = reader.ReadInt16(),
                                    Lighting = reader.ReadInt16()
                                };
                                b.HeightMap[yVertex, xVertex] = v;

                                //if (v.Height < minHeight) minHeight = v.Height;
                                //if (v.Height > maxHeight) maxHeight = v.Height;
                            }
                        }

                        b.Field1 = reader.ReadInt32();

                        for (int yTile = 0; yTile < 4; yTile++)
                        {
                            for (int xTile = 0; xTile < 4; xTile++)
                            {
                                Tile t = new()
                                {
                                    field0 = reader.ReadInt16(),
                                    field1 = reader.ReadInt16(),
                                    field2 = reader.ReadInt16()
                                };

                                byte typeAndFlags = reader.ReadByte();
                                t.flag = (TileFlag)((typeAndFlags >> 5) & 0b00000111); //don't ask me what is that magic, its chatGPT 
                                t.Type = (TileType)(typeAndFlags & 0b00011111); // extract the last 5-bits for the tile Type

                                t.Slip = (TileSlip)reader.ReadByte();

                                t.Field3 = reader.ReadInt16();

                                t.RotationFlip = reader.ReadByte();

                                t.TextureIndex = reader.ReadInt32();
                                t.Field4 = reader.ReadByte();

                                b.TileMap[yTile, xTile] = t;

                            }
                        }
                        blocks[yBlock, xBlock] = b;
                    }
                }
            }


        }


        public PMG(byte[] data) //parsing skyboxes 
        {
            blocks = new Block[8, 8];

            using MemoryStream ms = new(data);
            using BinaryReader reader = new(ms); //using a binaryreader make the offset advance by itself

            while (ms.Position < ms.Length)
            {
                for (int yBlock = 0; yBlock < 8; yBlock++)
                {
                    for (int xBlock = 0; xBlock < 8; xBlock++)
                    {
                        Block b = new()
                        {
                            XOffset = reader.ReadInt16(),
                            YOffset = reader.ReadInt16(),
                            ZOffset = reader.ReadInt16(),
                            Field0 = reader.ReadInt16()
                        };

                        for (int yVertex = 0; yVertex < 5; yVertex++)
                        {
                            for (int xVertex = 0; xVertex < 5; xVertex++)
                            {
                                Vertex v = new()
                                {
                                    Height = reader.ReadInt16(),
                                    Lighting = reader.ReadInt16()
                                };
                                b.HeightMap[yVertex, xVertex] = v;

                                //if (v.Height < minHeight) minHeight = v.Height;
                                //if (v.Height > maxHeight) maxHeight = v.Height;
                            }
                        }

                        b.Field1 = reader.ReadInt32();

                        for (int yTile = 0; yTile < 4; yTile++)
                        {
                            for (int xTile = 0; xTile < 4; xTile++)
                            {
                                Tile t = new()
                                {
                                    field0 = reader.ReadInt16(),
                                    field1 = reader.ReadInt16(),
                                    field2 = reader.ReadInt16()
                                };

                                byte typeAndFlags = reader.ReadByte();
                                t.flag = (TileFlag)((typeAndFlags >> 5) & 0b00000111); //don't ask me what is that magic, its chatGPT 
                                t.Type = (TileType)(typeAndFlags & 0b00011111); // extract the last 5-bits for the tile Type

                                t.Slip = (TileSlip)reader.ReadByte();

                                t.Field3 = reader.ReadInt16();

                                t.RotationFlip = reader.ReadByte();

                                t.TextureIndex = reader.ReadInt32();
                                t.Field4 = reader.ReadByte();

                                b.TileMap[yTile, xTile] = t;

                            }
                        }
                        blocks[yBlock, xBlock] = b;
                    }
                }

            }


        }
    }



    public class Block
    {
        public short XOffset;        // -16384 ~ 16384
        public short YOffset;        // unreliable?
        public short ZOffset;        // -16384 ~ 16384
        public short Field0;         // Unknown

        public Vertex[,] HeightMap = new Vertex[5, 5];

        public int Field1;           // always 0?

        public Tile[,] TileMap = new Tile[4, 4];
    }

    public class Vertex
    {
        public short Height;         // Height sample
        public short Lighting;       // <= 255
    }

    public class Tile
    {
        public short field0;        // always 0
        public short field1;        // always 0
        public short field2;        // always 0

        public TileFlag flag;       // the 3 first bits is shared tile type (0010 0100 = water) -> must combine like this : "byte combined = (byte)(flag << 5) | Type"
        public TileType Type;       // 5-bits format tile type
        public TileSlip Slip;       // Slip type (0-8)
        public short Field3;        // always 0

        public byte RotationFlip;
        public int TextureIndex;     // Into PTG
        public byte Field4;          // always 0

        public Bitmap FlipRotationSwitcher(Bitmap tileImage)
        {
            switch (RotationFlip)
            {
                case 0:
                    break;

                case 1:
                    tileImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;

                case 2:
                    tileImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;

                case 3:
                    tileImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    tileImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;

                case 4:
                    tileImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;

                case 5:
                    tileImage.RotateFlip(RotateFlipType.Rotate180FlipX);
                    break;

                case 6:
                    tileImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    tileImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;

                case 7:
                    tileImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    tileImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    tileImage.RotateFlip(RotateFlipType.Rotate180FlipNone);

                    break;
            }
            return tileImage;
        }

    }

    public enum TileFlag : byte
    {
        IsWater = 8,
        isMine = 2,
        IsWall = 4, //slippery tiles
    }

    public enum TileType : byte
    {
        Mud = 0,
        Grass = 1,
        Metal = 2,
        Wood = 3,
        Water = 4,
        Stone = 5,
        Rock = 6,
        Sand = 7,
        Ice = 8,
        Snow = 9,
        Quag = 10,
        Lava = 11,
    }

    public enum TileSlip : byte
    {
        Full = 0,
        Bottom = 1,
        Left = 2,
        Top = 3,
        Right = 4,
        BottomRight = 5,
        BottomLeft = 6,
        TopLeft = 7,
        TopRight = 8,
    }

}
