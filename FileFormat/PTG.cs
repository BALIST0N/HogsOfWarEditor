using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;



namespace hogs_gameEditor_wpf.FileFormat
{
    public class PTG
    {
        int textureCount;
        public List<TIM> textures = new List<TIM>();

        public PTG(string filepathWithoutExtension)
        {
            using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(filepathWithoutExtension + ".PTG")))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                this.textureCount = reader.ReadInt32();

                int texSize = (int)(ms.Length - 4) / this.textureCount;

                while (ms.Position < ms.Length)
                {
                    textures.Add(new TIM(reader.ReadBytes(texSize)));
                }

            }

            
        }

        public void DumpTiles()
        {
            string destination = GlobalVars.gameFolder + "devtools/EXPORT/map/"; 
            for (int i = 0; i < this.textureCount; i++)
            {
                File.WriteAllBytes(destination + i + ".png", this.textures[i].ToPngBytes().Item3 );
            }
            
        }

        public Bitmap CreateIMG(PMG mapTerrain) //PTG2PNG
        {
            Bitmap finalImage = new Bitmap(2048, 2048,System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(finalImage);

            for (int xb = 0; xb < 16; xb++)
            {
                for (int yb = 0; yb < 16; yb++)
                {
                    Block b = mapTerrain.blocks[xb, yb];

                    for (int xT = 0; xT < 4; xT++)
                    {
                        for (int yT = 0; yT < 4; yT++)
                        {
                            Tile t = b.TileMap[xT, yT];

                            Bitmap tileImage = this.textures[t.TextureIndex].ToBitmap();
                            
                            Graphics g2 = Graphics.FromImage(tileImage);

                            switch (t.RotationFlip)
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
                            /*
                            switch(t.Type)
                            {
                                case TileType.Water:
                                    g2.Clear(System.Drawing.Color.LightBlue);
                                    break;
                                case TileType.Grass:
                                    g2.Clear(System.Drawing.Color.Green);
                                    break;
                                case TileType.Ice:
                                    g2.Clear(System.Drawing.Color.LightCyan);
                                    break;
                                case TileType.Lava:
                                    g2.Clear(System.Drawing.Color.Orange);
                                    break;
                                case TileType.Metal:
                                    g2.Clear(System.Drawing.Color.Silver);
                                    break;
                                case TileType.Mud:
                                    g2.Clear(System.Drawing.Color.Brown);
                                    break;
                                case TileType.Quag:
                                    g2.Clear(System.Drawing.Color.OliveDrab);
                                    break;
                                case TileType.Rock:
                                    g2.Clear(System.Drawing.Color.Gray);
                                    break;
                                case TileType.Sand:
                                    g2.Clear(System.Drawing.Color.Salmon);
                                    break;
                                case TileType.Snow:
                                    g2.Clear(System.Drawing.Color.White);
                                    break;
                                case TileType.Stone:
                                    g2.Clear(System.Drawing.Color.DarkGray);
                                    break;
                                case TileType.Wood:
                                    g2.Clear(System.Drawing.Color.BurlyWood);
                                    break;
                            }

                            switch(t.flag)
                            {
                                case TileFlag.IsWater:
                                    g2.Clear(System.Drawing.Color.Blue); //its useless since water tiles are in tileType
                                    break;

                                case TileFlag.IsWall:
                                    g2.Clear(System.Drawing.Color.DimGray);
                                    break;

                                case TileFlag.isMine:
                                    g2.Clear(System.Drawing.Color.OrangeRed);
                                    break;
                            }*/
                            
                            tileImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
 

                            //int posX = ((15 - xb) * 4 + (3 - xT)) * 32;
                            int posX = (xb * 4 + xT) * 32;
                            int posY = (yb * 4 + yT) * 32;
                            g.DrawImage(tileImage, posX, posY);
                        }
                    }
                }
            }

            finalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
            return finalImage;
            //finalImage.Save(GlobalVars.gameFolder + "devtools/EXPORT/map.png", ImageFormat.Png);
            
        }


        public Bitmap CreateSkybox(PMG mapTerrain)
        {
            Bitmap bandImage = new Bitmap(512, 512, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bandImage);

            for (int xb = 0; xb < 8; xb++)
            {
                for (int yb = 0; yb < 8; yb++)
                {
                    Block b = mapTerrain.blocks[xb, yb];

                    for (int xT = 0; xT < 4; xT++)
                    {
                        for (int yT = 0; yT < 4; yT++)
                        {
                            Tile t = b.TileMap[xT, yT];

                            Bitmap tileImage = this.textures[t.TextureIndex].ToBitmap();
                            
                            Graphics g2 = Graphics.FromImage(tileImage);


                            switch (t.RotationFlip)
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
                            
                            tileImage.RotateFlip(RotateFlipType.Rotate90FlipX);

                            int posX = (xb * 4 + xT) * 32;
                            int posY = (yb * 4 + yT) * 32;
                            g.DrawImage(tileImage, posX, posY);
                        }
                    }
                }
            }

            bandImage.RotateFlip(RotateFlipType.Rotate270FlipNone);

            //the image is  stored in a special way for the game read lines by line, we need to swap lines to get a real view 
            Bitmap temp = bandImage.Clone(new Rectangle(0, 128, 512, 128), bandImage.PixelFormat);
            
            // Copy band 2 into band 1's place
            g.DrawImage(bandImage, new Rectangle(0, 128, 512, 128),new Rectangle(0, 2 * 128, 512, 128),GraphicsUnit.Pixel);

            // Copy temp (original band 1) into band 2's place
            g.DrawImage(temp, 0, 2 * 128, 512, 128);

            temp = bandImage.Clone(new Rectangle(0, 256, 512, 256), bandImage.PixelFormat);

            temp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            g.DrawImage(temp, 0, 256);


            temp.Dispose();


            return bandImage;
            //finalImage.Save(GlobalVars.gameFolder + "devtools/EXPORT/map.png", ImageFormat.Png);

        }

    }
}
