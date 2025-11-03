using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace hogs_gameEditor_wpf.FileFormat
{
    public class TIM
    {
        public byte ID { get; set; }
        public byte version { get; set; }
        public ushort reserved { get; set; }
        public int bpp { get; set; }
        public bool hasClut { get; set; }
        public byte[] reserved2 { get; set; }
        public uint clutSectionDataSize { get; set; }
        public uint destinationXY { get; set; }
        public uint widthHeight { get; set; }
        public byte[] palette { get; set; }
        public uint pixelDataSize { get; set; }
        public uint destinationCoord { get; set; }
        public uint pixelPosition { get; set; }
        public byte[] pixelData { get; set; }


        public TIM(byte[] file)
        {
            using (MemoryStream ms = new MemoryStream(file))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                this.ID = reader.ReadByte();        // 0x10
                this.version = reader.ReadByte();   // 0x00
                this.reserved = reader.ReadUInt16();

                uint flags = reader.ReadUInt32();   // Lire 4 octets (uint)

                // --- Extraire les infos depuis le champ flags ---
                this.bpp = (int)(flags & 0b111);          // Bits 1-3 : mode bpp
                this.hasClut = (flags & 0b1000) != 0;     // Bit 4 : présence CLUT


                if (this.hasClut)
                {
                    // --- Lire section CLUT ---
                    this.clutSectionDataSize = reader.ReadUInt32();
                    this.destinationXY = reader.ReadUInt32();
                    this.widthHeight = reader.ReadUInt32();

                    // Lecture de la palette
                    switch (this.bpp)
                    {
                        case 0: // 4bpp = 16 couleurs * 2 octets
                            this.palette = reader.ReadBytes(16 * 2);
                            break;

                        case 1: // 8bpp = 256 couleurs * 2 octets
                            this.palette = reader.ReadBytes(256 * 2);
                            break;

                        default:
                            this.palette = null; // Pas de CLUT nécessaire pour 16/24bpp
                            break;
                    }
                }

                // --- Lire section pixel ---
                this.pixelDataSize = reader.ReadUInt32();
                this.destinationCoord = reader.ReadUInt32();
                this.pixelPosition = reader.ReadUInt32();

                // Taille des données restantes = taille du bloc - 12 (en-tête de section)
                this.pixelData = reader.ReadBytes( (int)this.pixelDataSize - 12 );
            }
        }



        public Bitmap ToBitmap()
        {
            ushort widthUnit = (ushort)(this.pixelPosition & 0xFFFF);
            ushort height = (ushort)(this.pixelPosition >> 16);

            int width = this.bpp switch
            {
                0 => widthUnit * 4,  // 4 bpp
                1 => widthUnit * 2,  // 8 bpp
                2 => widthUnit,      // 16 bpp
                _ => throw new NotSupportedException()
            };

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            Color[] decodedPalette = null;

            if (this.hasClut && this.palette != null)
            {
                int colorCount = this.palette.Length / 2;
                decodedPalette = new Color[colorCount];

                using (var paletteReader = new BinaryReader(new MemoryStream(this.palette)))
                {
                    for (int i = 0; i < colorCount; i++)
                    {
                        ushort raw = paletteReader.ReadUInt16();
                        int r = (raw & 0x1F);
                        int g = (raw >> 5) & 0x1F;
                        int b = (raw >> 10) & 0x1F;
                        //int a = (raw >> 15) & 0x1;

                        r = (r << 3) | (r >> 2);
                        g = (g << 3) | (g >> 2);
                        b = (b << 3) | (b >> 2);

                        decodedPalette[i] = Color.FromArgb(255, r, g, b);
                    }
                }
            }

            // Buffer de pixels en BGRA (4 octets par pixel)
            byte[] pixelBuffer = new byte[width * height * 4];

            switch (this.bpp)
            {
                case 0: // 4bpp, palette 16 couleurs
                {
                    int stride = widthUnit * 2; // octets par ligne dans pixelData
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int byteIndex = y * stride + (x / 2);
                            if (byteIndex >= pixelData.Length) continue;

                            byte b = pixelData[byteIndex];
                            int index = (x % 2 == 0) ? (b & 0x0F) : (b >> 4);
                            Color c = decodedPalette?[index] ?? Color.Magenta;

                            // Si couleur noire, alpha = 0 (transparent)
                            byte alpha = (c.R == 0 && c.G == 0 && c.B == 0) ? (byte)0 : c.A;

                            int i = (y * width + x) * 4;
                            pixelBuffer[i + 0] = c.B;
                            pixelBuffer[i + 1] = c.G;
                            pixelBuffer[i + 2] = c.R;
                            pixelBuffer[i + 3] = alpha;
                        }
                    }
                    break;
                }
                case 1:
                {
                    int stride = widthUnit * 2; // chaque unité = 2 octets de données
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int byteIndex = y * stride + x;
                            if (byteIndex >= pixelData.Length) continue;

                            byte index = pixelData[byteIndex];
                            Color c = decodedPalette?[index] ?? Color.Magenta;

                            byte alpha = (c.R == 0 && c.G == 0 && c.B == 0) ? (byte)0 : (byte)255;

                            int i = (y * width + x) * 4;
                            pixelBuffer[i + 0] = c.B;
                            pixelBuffer[i + 1] = c.G;
                            pixelBuffer[i + 2] = c.R;
                            pixelBuffer[i + 3] = alpha;
                        }
                    }
                    break;
                }

                default:
                    throw new NotSupportedException($"Unsupported bpp mode: {this.bpp}");
            }


            // Copier le buffer dans le Bitmap
            var bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, 0, bmpData.Scan0, pixelBuffer.Length);

            bmp.UnlockBits(bmpData);

            return bmp;
        }



        public (int,int,byte[]) ToPngBytes()
        {
            using (var bmp = this.ToBitmap())
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                return (bmp.Width,bmp.Height,ms.ToArray());
            }
        }

        public static byte[] ToPngBytes(Bitmap bmp)
        {
            using (var ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public byte[] SerializeToBinary()
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                // --- En-tête ---
                writer.Write(this.ID);             // 1 byte
                writer.Write(this.version);        // 1 byte
                writer.Write(this.reserved);       // 2 bytes

                // --- Flags ---
                uint flags = 0;
                flags |= (uint)(this.bpp & 0b111);     // Bits 0-2 = bpp
                if (this.hasClut)
                    flags |= 0b1000;                  // Bit 3 = CLUT présent
                writer.Write(flags);

                // --- Section CLUT (si présente) ---
                if (this.hasClut)
                {
                    writer.Write(this.clutSectionDataSize);
                    writer.Write(this.destinationXY);
                    writer.Write(this.widthHeight);

                    if (this.palette != null)
                        writer.Write(this.palette);
                }

                // --- Section pixel ---
                writer.Write(this.pixelDataSize);
                writer.Write(this.destinationCoord);
                writer.Write(this.pixelPosition);

                if (this.pixelData != null)
                    writer.Write(this.pixelData);

                // Retourner le tableau complet
                return ms.ToArray();
            }

        }

    }


}
