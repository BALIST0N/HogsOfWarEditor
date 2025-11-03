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
       public int boneParentIndex {  get; set; }

       public short TransformX    { get; set; }
       public short TransformY    { get; set; }
       public short TransformZ    { get; set; }
       public short TransformW    { get; set; }  //padding

       public short RotationX     { get; set; }
       public short RotationY     { get; set; }
       public short RotationZ     { get; set; }
       public short RotationW     { get; set; }


        public static List<HIR> GetSkeletonList()
        {
            List<HIR> skeletons = new List<HIR>();

            using (FileStream fs = File.Open(GlobalVars.gameFolder + "/devtools/EXTRACTED/chars/pig.hir", FileMode.Open, FileAccess.Read, FileShare.ReadWrite) ) 
            using (BinaryReader reader = new BinaryReader(fs))
            {
                while (fs.Position < fs.Length)
                {
                    skeletons.Add(new HIR
                    {
                        boneParentIndex = reader.ReadInt32(),
                        TransformX = reader.ReadInt16(),
                        TransformY = reader.ReadInt16(),
                        TransformZ = reader.ReadInt16(),
                        TransformW = reader.ReadInt16(),
                        RotationX = reader.ReadInt16(),
                        RotationY = reader.ReadInt16(),
                        RotationZ = reader.ReadInt16(),
                        RotationW = reader.ReadInt16(),
                    });
                }

            }

            return skeletons;
        }



    }


}
