using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.SharpDX.Core.Assimp;
using hogs_gameEditor_wpf.FileFormat;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;




namespace hogs_gameEditor_wpf
{
    /// <summary>
    /// Interaction logic for MapView3DRender.xaml
    /// </summary>
    public partial class MapView3DRender : Window
    {


        public MapView3DRender(string mapName, List<POG> currentMap)
        {
            InitializeComponent();

            string location = GlobalVars.exportFolder + "/"+mapName+".glb";

            //if file.exists location then load file
            if (File.Exists(location) == false ) 
            {
                PMG mapTerrain = new PMG(GlobalVars.mapsFolder + mapName);
                PTG mapTexture = new PTG(GlobalVars.mapsFolder + mapName);
                GlobalVars.ExportTerrain_GLB(mapTerrain, mapTexture, mapName);
            }

            HelixToolkitScene scene = new Importer().Load(location);

            //viewport.Items.Add( scene );


        }





    }






    /*
    private void LoadEntities(List<POG> currentMap)
    {
        foreach (POG entity in currentMap)
        {
            //todo : use a file where all models exists instead of reading all models

            string target = entitiesPath + GlobalVars.Name_Converter(entity.name) + ".obj";
            try
            {
                Model3DGroup model = new ModelImporter().Load(target);

                Transform3DGroup transformGroup = new Transform3DGroup();
                transformGroup.Children.Add( new RotateTransform3D(new AxisAngleRotation3D( new Vector3D(1, 0, 0), GlobalVars.ScaleDownAngles(entity.angles[0])-180 )) );   // X
                transformGroup.Children.Add( new RotateTransform3D(new AxisAngleRotation3D( new Vector3D(0, 0, 1), GlobalVars.ScaleDownAngles(entity.angles[1]) )) );   // Z
                transformGroup.Children.Add( new RotateTransform3D(new AxisAngleRotation3D( new Vector3D(0, 1, 0), GlobalVars.ScaleDownAngles(entity.angles[2])+180 )) );   // Y
                transformGroup.Children.Add( new TranslateTransform3D(entity.position[0], entity.position[2], entity.position[1]));

                model.Freeze();

                helixViewport.Children.Add(new ModelVisual3D
                {
                    Content = model,
                    Transform = transformGroup
                });

            }
            catch
            {
                //just ignore
            }


        }
    }*/






}
    
    

