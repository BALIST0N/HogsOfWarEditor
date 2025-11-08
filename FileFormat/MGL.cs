using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Storage.Compression;
using Windows.UI.ViewManagement;

namespace hogs_gameEditor_wpf.FileFormat
{
    class MGL
    {
        public byte[] data { get; set; }
        public Bitmap image { get; set; }

        public MGL(byte[] hex)
        {
            this.data = Decompress(hex);
        }

        public static byte[] Decompress(byte[] input)
        {
            return null; //c'est de la merde allez tous vous fairzmljfejmzfheklfjmzefsdvcvhmlpoîpo^m(-è(-47&
        }

        public static Bitmap RemoveMagenta(Bitmap b)
        {
            return null;
        }

        public void RemoveMagenta()
        {
            
        }
    }


}
