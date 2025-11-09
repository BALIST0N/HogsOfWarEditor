using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Media.Playlists;

namespace hogs_gameEditor_wpf.FileFormat
{

    public class HIR //character skeleton, source : https://github.com/TalonBraveInfo/OpenHoW/blob/master/doc/file-formats/HIR.md
    {
        
        public int boneParentIndex  { get; set; }
        public string boneName      { get; private set; }
        public short TransformX     { get; set; }
        public short TransformY     { get; set; }
        public short TransformZ     { get; set; }
        public short TransformW     { get; set; }  //padding

        public short RotationX      { get; set; }
        public short RotationY      { get; set; }
        public short RotationZ      { get; set; }
        public short RotationW      { get; set; }


        public HIR()
        {

        }

        public void SetBoneName()
        {
            this.boneName = GlobalVars.BoneNames[this.boneParentIndex];
        }

        public static List<HIR> GetSkeletonList(string path = null)
        {
            List<HIR> skeletons = new List<HIR>();
            int counter = 0;

            using (MemoryStream ms = new MemoryStream( File.ReadAllBytes( path == null ? GlobalVars.gameFolder+ "Chars/pig.hir" : path ) ))
            using (BinaryReader reader = new BinaryReader(ms))
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
