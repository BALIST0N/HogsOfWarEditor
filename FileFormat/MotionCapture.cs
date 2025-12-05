using System;
using System.Collections.Generic;
using System.IO;

namespace hogs_gameEditor_wpf.FileFormat
{
    public class MotionCapture //mcap.mad aka animations, 
    {
        public int DataOffset { get; set; } //within this file.
        public int DataSize { get; set; }

        public List<Keyframe> Keyframes { get; set; }

        public MotionCapture(byte[] header)
        {
            DataOffset = BitConverter.ToInt32(header, 0);
            DataSize = BitConverter.ToInt32(header, 4);

            Keyframes = [];
        }

        public void SetKeyframesFromData(byte[] data)
        {
            for (int i = 0; i < DataSize; i++)
            {
                Keyframes.Add(new Keyframe(data[i..(i + 272)]));
                i += 271;
            }
        }

        public static List<MotionCapture> GetMotionCaptureAnimations()
        {
            List<MotionCapture> motionCaptures = [];

            byte[] mapdata = File.ReadAllBytes(GlobalVars.gameFolder + "chars/mcap.mad");
            int endContenTable = BitConverter.ToInt32(mapdata, 0); //the first item offset define table content size ! 

            for (int i = 0; i <= endContenTable; i++)
            {
                int endblockContentTable = i + 8;

                if (endblockContentTable <= endContenTable)
                {
                    MotionCapture mcap = new(mapdata[i..(i + 8)]);
                    mcap.SetKeyframesFromData(mapdata[mcap.DataOffset..(mcap.DataOffset + mcap.DataSize)]);
                    motionCaptures.Add(mcap);
                }
                i += 7;
            }


            return motionCaptures;
        }

        public static List<MotionCapture> GetMotionCaptureAnimations(string path)
        {
            List<MotionCapture> motionCaptures = [];

            byte[] mapdata = File.ReadAllBytes(path);
            int endContenTable = BitConverter.ToInt32(mapdata, 0); //the first item offset define table content size ! 

            for (int i = 0; i <= endContenTable; i++)
            {
                int endblockContentTable = i + 8;

                if (endblockContentTable <= endContenTable)
                {
                    MotionCapture mcap = new(mapdata[i..(i + 8)]);
                    mcap.SetKeyframesFromData(mapdata[mcap.DataOffset..(mcap.DataOffset + mcap.DataSize)]);
                    motionCaptures.Add(mcap);
                }
                i += 7;
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

        public List<(float x,float y, float z,float w) > BoneRotation { get; set; }

        public Keyframe(byte[] data)
        {
            using MemoryStream ms = new(data);
            using BinaryReader reader = new(ms);

            rootTransformX = reader.ReadInt32();
            rootTransformY = reader.ReadInt32();
            rootTransformZ = reader.ReadInt32();
            rootTransformW = reader.ReadInt32();
            objectTransformX = reader.ReadInt32();
            objectTransformY = reader.ReadInt32();
            objectTransformZ = reader.ReadInt32();
            objectTransformW = reader.ReadInt32();

            BoneRotation = [];
            while (ms.Position < ms.Length)
            {
                BoneRotation.Add( new(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()) );
                
            }


        }



    }


}
