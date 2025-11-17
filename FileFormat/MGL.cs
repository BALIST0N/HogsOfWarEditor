using System.Drawing;

namespace hogs_gameEditor_wpf.FileFormat
{
    internal class MGL
    {
        public byte[] data { get; set; }
        public Bitmap image { get; set; }

        public MGL(byte[] hex)
        {
            data = Decompress(hex);
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
