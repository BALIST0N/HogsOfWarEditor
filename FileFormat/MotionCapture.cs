using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace hogs_gameEditor_wpf.FileFormat
{
    public class MotionCapture //mcap.mad aka animations, 
    {
        public int DataOffset { get; set; } //within this file.
        public int DataSize { get; set; }

        public List<Keyframe> Keyframes { get; set; }

        public MotionCapture(byte[] header)
        {
            this.DataOffset = BitConverter.ToInt32(header, 0);
            this.DataSize = BitConverter.ToInt32(header, 4);

            this.Keyframes = new List<Keyframe>();
        }

        public void SetKeyframesFromData(byte[] data)
        {
            for (int i = 0; i < DataSize; i++)
            {
                this.Keyframes.Add(new Keyframe(data[i..(i + 272)]));
                i = i + 271;
            }
        }

        public static List<MotionCapture> GetMotionCaptureAnimations()
        {
            List<MotionCapture> motionCaptures = new List<MotionCapture>();

            using (FileStream fs = File.Open(GlobalVars.gameFolder + "chars/mcap.mad", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] mapdata = new byte[fs.Length];
                fs.Read(mapdata, 0, Convert.ToInt32(fs.Length));
                int endContenTable = BitConverter.ToInt32(mapdata, 0); //the first item offset define table content size ! 

                for (int i = 0; i <= endContenTable; i++)
                {
                    int endblockContentTable = i + 8;

                    if (endblockContentTable <= endContenTable)
                    {
                        MotionCapture mcap = new MotionCapture(mapdata[i..(i + 8)]);
                        mcap.SetKeyframesFromData(mapdata[mcap.DataOffset..(mcap.DataOffset + mcap.DataSize)]);
                        motionCaptures.Add(mcap);
                    }
                    i += 7;
                }
            }

            return motionCaptures;
        }



    }


    public class Keyframe
    {
        public int rootTransformX { get; set; }
        public int rootTransformY { get; set; }
        public int rootTransformZ { get; set; }
        public int rootTransformW { get; set; }
        public int objectTransformX { get; set; }
        public int objectTransformY { get; set; }
        public int objectTransformZ { get; set; }
        public int objectTransformW { get; set; }
        public List<Vector4> BoneRotation { get; set; }

        public Keyframe(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(ms))
            {

                this.rootTransformX = reader.ReadInt32();
                this.rootTransformY = reader.ReadInt32();
                this.rootTransformZ = reader.ReadInt32();
                this.rootTransformW = reader.ReadInt32();
                this.objectTransformX = reader.ReadInt32();
                this.objectTransformY = reader.ReadInt32();
                this.objectTransformZ = reader.ReadInt32();
                this.objectTransformW = reader.ReadInt32();

                this.BoneRotation = new List<Vector4>();
                while (ms.Position < ms.Length)
                {
                    this.BoneRotation.Add(new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
                }

            }


        }

    }


}
