using System.Collections.Generic;
using System.IO;

namespace hogs_gameEditor_wpf.FileFormat
{

    public class HIR //character skeleton, source : https://github.com/TalonBraveInfo/OpenHoW/blob/master/doc/file-formats/HIR.md
    {

        public int boneParentIndex { get; set; }
        public string boneName { get; private set; }
        public short TransformX { get; set; }
        public short TransformY { get; set; }
        public short TransformZ { get; set; }
        public short TransformW { get; set; }  //padding

        public short RotationX { get; set; }
        public short RotationY { get; set; }
        public short RotationZ { get; set; }
        public short RotationW { get; set; }


        public HIR()
        {

        }

        public void SetBoneName()
        {
            boneName = GlobalVars.BoneNames[boneParentIndex];
        }

        public static List<HIR> GetSkeletonList(string path = null)
        {
            List<HIR> skeletons = [];
            int counter = 0;

            using (MemoryStream ms = new(File.ReadAllBytes(path ?? (GlobalVars.gameFolder + "Chars/pig.hir"))))
            using (BinaryReader reader = new(ms))
            {
                while (ms.Position < ms.Length)
                {
                    skeletons.Add(new HIR
                    {
                        boneParentIndex = reader.ReadInt32(),
                        boneName = GlobalVars.BoneNames[counter],
                        TransformX = reader.ReadInt16(),
                        TransformY = reader.ReadInt16(),
                        TransformZ = reader.ReadInt16(),
                        TransformW = reader.ReadInt16(),
                        RotationX = reader.ReadInt16(),
                        RotationY = reader.ReadInt16(),
                        RotationZ = reader.ReadInt16(),
                        RotationW = reader.ReadInt16(),
                    });
                    counter++;
                }
            }

            return skeletons;
        }





    }


}
