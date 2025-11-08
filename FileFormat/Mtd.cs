using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hogs_gameEditor_wpf.FileFormat
{
    public class Mtd //one texture aka .TIM or .png 
    {
        public string Name { get; set; }  // xxx.TIM or .png
        public int DataOffset { get; set; }  // 4 bytes 
        public int DataSize { get; set; }  // 4 bytes
        public byte[] textureData { get; set; } // the image in .tim format or in .png
        
        public TIM textureTim {  get; set; }

        public int indexNumber { get; set; }
        public int width { get; set; }
        public int height { get; set; }


        public Mtd(byte[] hexblock)
        {
            this.Name = Encoding.ASCII.GetString(hexblock[0..16]).Trim('\0');
            this.DataOffset = BitConverter.ToInt32(hexblock, 16);
            this.DataSize = BitConverter.ToInt32(hexblock, 20);
        }

        public Mtd()
        {

        }


        /// <summary>
        /// load textures from map folder mtd file
        /// </summary>
        public static List<Mtd> LoadTexturesFromMTD(FAC model, string mapMTDFileName)
        {
            List<Mtd> textures = new List<Mtd>();

            string fldr = GlobalVars.mapsFolder + mapMTDFileName + ".mtd";

            byte[] mtdData = File.ReadAllBytes(fldr);
            int endContenTable = BitConverter.ToInt32(mtdData, 16); //the first item offset define table content size ! 

            var indexes = model.GetIndexes();

            int counter = 0;
            for (int i = 0; i <= endContenTable; i++)
            {
                if( indexes.Contains(counter) )
                {
                    int endblockContentTable = i + 24;
                    if (endblockContentTable <= endContenTable)
                    {
                        Mtd tg = new Mtd
                        {
                            Name = Encoding.ASCII.GetString(mtdData[i..(i + 16)]).Trim('\0'),
                            DataOffset = BitConverter.ToInt32(mtdData[(i + 16)..(i + 20)]),
                            DataSize = BitConverter.ToInt32(mtdData[(i + 20)..(i + 24)]),
                            indexNumber = counter
                        };
                        //tg.textureData = mtdData[tg.DataOffset..(tg.DataOffset + tg.DataSize)];
                        tg.textureTim = new TIM(mtdData[tg.DataOffset..(tg.DataOffset + tg.DataSize)]);
                        textures.Add(tg);
                    }
                }

                i += 23;
                counter++;
            }          

            return textures;
        }

        public static List<Mtd> LoadTexturesFromMTD(FAC model, string path, bool unused)
        {
            List<Mtd> textures = new List<Mtd>();

            byte[] mtdData = File.ReadAllBytes(path);
            int endContenTable = BitConverter.ToInt32(mtdData, 16); //the first item offset define table content size ! 
            var indexes = model.GetIndexes();

            int counter = 0;
            for (int i = 0; i <= endContenTable; i++)
            {
                if (indexes.Contains(counter))
                {
                    int endblockContentTable = i + 24;
                    if (endblockContentTable <= endContenTable)
                    {
                        Mtd tempTex = new Mtd
                        {
                            Name = Encoding.ASCII.GetString(mtdData[i..(i + 16)]).Trim('\0'),
                            DataOffset = BitConverter.ToInt32(mtdData[(i + 16)..(i + 20)]),
                            DataSize = BitConverter.ToInt32(mtdData[(i + 20)..(i + 24)]),
                            indexNumber = counter
                        };
                        tempTex.textureTim = new TIM( mtdData[tempTex.DataOffset..(tempTex.DataOffset + tempTex.DataSize)] );
                        textures.Add(tempTex);
                    }
                }

                i += 23;
                counter++;
            }
            

            return textures;
        }

    }
}
