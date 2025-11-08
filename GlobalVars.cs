using BitmapToVector;
using hogs_gameEditor_wpf.FileFormat;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Memory;
using SharpGLTF.Scenes;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using Windows.Devices.Radios;
using Path = System.IO.Path;



namespace hogs_gameEditor_wpf
{
    /// <summary>
    /// some utility static class, provide various converters and exporters entry points, aswell with default folder locations
    /// </summary>
    public static class GlobalVars
    {
        public static string gameFolder = "E:/Games/IGG-HogsofWar/";
        public static string mapsFolder = gameFolder + "Maps/";
        public static string mapsViewsFolder = gameFolder + "devtools/mapview/";
        public static string exportFolder = gameFolder + "devtools/EXPORT/";
            
        public static string[] BoneNames =
        {
           "Hip",
           "Spine",
           "Head",
           "UpperArmL",
           "LowerArmL",
           "HandL",
           "UpperArmR",
           "LowerArmR",
           "HandR",
           "UpperLegL",
           "LowerLegL",
           "FootL",
           "UpperLegR",
           "LowerLegR",
           "FootR"

        };

        public static string[] entityFilterList = {
            "WE_BAZZ",
            "WE_BOMB",
            "WE_CHUT2",
            "WE_GRE2",
            "WE_SMOK2",
            "WE_SUPAB",
            "WE_TNT2",
            "BULL1",
            "RPACK",
            "WE_APMIN",
            "WE_FIRB",
            "WE_REMO",
            "WE_ROLL",
            "WESUBB",
            "WE_SU_GR",
            "WE_BANG",
            "WE_BALL",
            "WE_ROCKT",
            "PROPOINT" };

        public static Dictionary<string,string> modelsWithChilds = new Dictionary<string,string>()
        {
            {"SWILL2","SW2ARM"},
            {"PILLBOX","PILLBAR"},
            {"AM_TANK","AMPHGUN"},
            {"BIG_GUN","BIGBAR"},
            {"TANK","TANBAR"}  ,
            //{"WINDM","WIN_SH" }
        };


        public static List<string> modelsWithMultipleSkins = new List<string>()
        {
            {"PILLBOX"},
            {"AM_TANK"},
            {"BIG_GUN"},
            {"TANK"},
            {"TENT_S"},
            {"M_TENT2"},
            {"M_TENT1"},
            {"SHELTER"}
        };


        public static double ScaleDownAngles(short value4096)
        {
            return (Convert.ToDouble(value4096) / 4096.0) * 360.0;
        }

        public static double ScaleUpAngles(double value360)
        {
            return (value360 / 360.0) * 4096.0;
        }

        public static string Name_Converter(char[] arreteTonChar)
        {
            return new String(arreteTonChar).Trim('\0');
        }

        public static void ExportModelWithOutTexture_GLB(MAD model, string path = null)
        {

            // Création des matériaux correspondant à chaque texture
            Dictionary<int, (MaterialBuilder Material, int Width, int Height)> materialDict = new Dictionary<int, (MaterialBuilder Material, int Width, int Height)>();

            materialDict[0] = (new MaterialBuilder().WithBaseColor(new Vector4(1, 1, 1, 1)) , 64, 64);

            

            // 2. Crée un MeshBuilder
            Dictionary<int, MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>> meshDict = new Dictionary<int, MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>>();

            foreach (var texIdx in materialDict.Keys)
            {
                meshDict[texIdx] = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>($"Mesh_{texIdx}");
            }


            foreach (var tri in model.facData.triangleList)
            {
                var mesh = meshDict[0];
                var prim = mesh.UsePrimitive(materialDict[0].Material);

                Vertice vA = model.vtxData.verticesList[tri.Vertex_A];
                Vertice vB = model.vtxData.verticesList[tri.Vertex_B];
                Vertice vC = model.vtxData.verticesList[tri.Vertex_C];

                // Construction des positions des sommets
                Vector3 pA = new Vector3(vA.XOffset, vA.YOffset, vA.ZOffset);
                Vector3 pB = new Vector3(vB.XOffset, vB.YOffset, vB.ZOffset);
                Vector3 pC = new Vector3(vC.XOffset, vC.YOffset, vC.ZOffset);

                // Calcul des vecteurs de bord pour la normale du triangle
                Vector3 normal = Vector3.Normalize(Vector3.Cross(pB - pA, pC - pA));

                // Construction des UVs (à normaliser si nécessaire !)
                float textureWidth = (float)materialDict[0].Width;
                float textureHeight = (float)materialDict[0].Height;

                Vector2 uvA = new Vector2(tri.U_A / textureWidth, tri.V_A / textureHeight);
                Vector2 uvB = new Vector2(tri.U_B / textureWidth, tri.V_B / textureHeight);
                Vector2 uvC = new Vector2(tri.U_C / textureWidth, tri.V_C / textureHeight);

                prim.AddTriangle(
                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pA, normal),
                        new VertexTexture1(uvA)
                        ),
                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pB, normal),
                        new VertexTexture1(uvB)
                        ),
                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pC, normal),
                        new VertexTexture1(uvC)
                        )
                );

            }

            foreach (var quad in model.facData.planeList)
            {
                var mesh = meshDict[0];
                var prim = mesh.UsePrimitive(materialDict[0].Material);

                Vertice vA = model.vtxData.verticesList[quad.Vertex_A];
                Vertice vB = model.vtxData.verticesList[quad.Vertex_B];
                Vertice vC = model.vtxData.verticesList[quad.Vertex_C];
                Vertice vD = model.vtxData.verticesList[quad.Vertex_D];

                Vector3 pA = new Vector3(vA.XOffset, vA.YOffset, vA.ZOffset);
                Vector3 pB = new Vector3(vB.XOffset, vB.YOffset, vB.ZOffset);
                Vector3 pC = new Vector3(vC.XOffset, vC.YOffset, vC.ZOffset);
                Vector3 pD = new Vector3(vD.XOffset, vD.YOffset, vD.ZOffset);

                float textureWidth = (float)materialDict[0].Width;
                float textureHeight = (float)materialDict[0].Height;

                Vector2 uvA = new Vector2(quad.U_A / textureWidth, quad.V_A / textureHeight);
                Vector2 uvB = new Vector2(quad.U_B / textureWidth, quad.V_B / textureHeight);
                Vector2 uvC = new Vector2(quad.U_C / textureWidth, quad.V_C / textureHeight);
                Vector2 uvD = new Vector2(quad.U_D / textureWidth, quad.V_D / textureHeight);

                // Calcul des vecteurs de bord
                Vector3 normal = Vector3.Normalize(Vector3.Cross(pB - pA, pC - pA));
                Vector3 normal2 = Vector3.Normalize(Vector3.Cross(pC - pA, pD - pA));


                prim.AddQuadrangle(
                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pA, normal),
                        new VertexTexture1(uvA)),

                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pB, normal),
                        new VertexTexture1(uvB)),

                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pC, normal),
                        new VertexTexture1(uvC)),

                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pD, normal2),
                        new VertexTexture1(uvD))
                );
            }


            // 5. Créer la scène
            var scene = new SceneBuilder();

            foreach (var kvp in meshDict)
            {
                scene.AddRigidMesh(kvp.Value, Matrix4x4.CreateScale(1, -1, 1));
            }

            //5. export
            if (path != null)
            {
                scene.ToGltf2().SaveGLB(path);
            }
            else
            {
                scene.ToGltf2().SaveGLB(exportFolder + model.GetName());
            }
        }

        public static void ExportModelWithTexture_GLB(MAD model,string path = null)
        {

            // Création des matériaux correspondant à chaque texture
            Dictionary<int, (MaterialBuilder Material, int Width, int Height)> materialDict = new Dictionary<int, (MaterialBuilder Material, int Width, int Height)>();

            foreach (Mtd texture in model.textures)
            {
                if(texture.textureData  != null)
                {
                    using var img = System.Drawing.Image.FromStream(new MemoryStream(texture.textureData));
                    texture.width = img.Width;
                    texture.height = img.Height;

                    var mat = new MaterialBuilder()
                        .WithChannelImage(KnownChannel.BaseColor, ImageBuilder.From(new MemoryImage(texture.textureData)))
                        .WithAlpha(SharpGLTF.Materials.AlphaMode.MASK);
                    mat.AlphaCutoff = 0.5f;
                    materialDict[texture.indexNumber] = (mat, texture.width, texture.height);
                }
                else
                {
                    var t = texture.textureTim.ToPngBytes();
                    texture.width = t.Item1;
                    texture.height = t.Item2;

                    var mat = new MaterialBuilder()
                        .WithChannelImage(KnownChannel.BaseColor, ImageBuilder.From(new MemoryImage( t.Item3 )))
                        .WithAlpha(SharpGLTF.Materials.AlphaMode.MASK);
                    mat.AlphaCutoff = 0.5f;

                    materialDict[texture.indexNumber] = (mat, texture.width, texture.height);
                }

            }

            // 2. Crée un MeshBuilder
            Dictionary<int, MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>> meshDict = new Dictionary<int, MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>>();

            foreach (var texIdx in materialDict.Keys)
            {
                meshDict[texIdx] = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>($"Mesh_{texIdx}");
            }


            foreach (var tri in model.facData.triangleList)
            {
                var mesh = meshDict[tri.TextureIndex];
                var prim = mesh.UsePrimitive(materialDict[tri.TextureIndex].Material);

                Vertice vA = model.vtxData.verticesList[tri.Vertex_A];
                Vertice vB = model.vtxData.verticesList[tri.Vertex_B];
                Vertice vC = model.vtxData.verticesList[tri.Vertex_C];

                // Construction des positions des sommets
                Vector3 pA = new Vector3(vA.XOffset, vA.YOffset, vA.ZOffset);
                Vector3 pB = new Vector3(vB.XOffset, vB.YOffset, vB.ZOffset);
                Vector3 pC = new Vector3(vC.XOffset, vC.YOffset, vC.ZOffset);

                // Calcul des vecteurs de bord pour la normale du triangle
                Vector3 normal = Vector3.Normalize(Vector3.Cross(pB - pA, pC - pA));

                // Construction des UVs (à normaliser si nécessaire !)
                float textureWidth = (float)materialDict[tri.TextureIndex].Width;
                float textureHeight = (float)materialDict[tri.TextureIndex].Height;

                Vector2 uvA = new Vector2(tri.U_A / textureWidth, tri.V_A / textureHeight);
                Vector2 uvB = new Vector2(tri.U_B / textureWidth, tri.V_B / textureHeight);
                Vector2 uvC = new Vector2(tri.U_C / textureWidth, tri.V_C / textureHeight);

                prim.AddTriangle(
                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pA, normal),
                        new VertexTexture1(uvA)
                        ),
                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pB, normal),
                        new VertexTexture1(uvB)
                        ),
                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pC, normal),
                        new VertexTexture1(uvC)
                        )
                );

            }

            foreach (var quad in model.facData.planeList)
            {
                var mesh = meshDict[quad.TextureIndex];
                var prim = mesh.UsePrimitive(materialDict[quad.TextureIndex].Material);

                Vertice vA = model.vtxData.verticesList[quad.Vertex_A];
                Vertice vB = model.vtxData.verticesList[quad.Vertex_B];
                Vertice vC = model.vtxData.verticesList[quad.Vertex_C];
                Vertice vD = model.vtxData.verticesList[quad.Vertex_D];

                Vector3 pA = new Vector3(vA.XOffset, vA.YOffset, vA.ZOffset);
                Vector3 pB = new Vector3(vB.XOffset, vB.YOffset, vB.ZOffset);
                Vector3 pC = new Vector3(vC.XOffset, vC.YOffset, vC.ZOffset);
                Vector3 pD = new Vector3(vD.XOffset, vD.YOffset, vD.ZOffset);

                float textureWidth = (float)materialDict[quad.TextureIndex].Width;
                float textureHeight = (float)materialDict[quad.TextureIndex].Height;

                Vector2 uvA = new Vector2(quad.U_A / textureWidth, quad.V_A / textureHeight);
                Vector2 uvB = new Vector2(quad.U_B / textureWidth, quad.V_B / textureHeight);
                Vector2 uvC = new Vector2(quad.U_C / textureWidth, quad.V_C / textureHeight);
                Vector2 uvD = new Vector2(quad.U_D / textureWidth, quad.V_D / textureHeight);

                // Calcul des vecteurs de bord
                Vector3 normal = Vector3.Normalize(Vector3.Cross(pB - pA, pC - pA));
                Vector3 normal2 = Vector3.Normalize(Vector3.Cross(pC - pA, pD - pA));
                

                prim.AddQuadrangle(
                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pA, normal),
                        new VertexTexture1(uvA)),

                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pB, normal),
                        new VertexTexture1(uvB)),

                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pC, normal),
                        new VertexTexture1(uvC)),

                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexEmpty>(
                        new VertexPositionNormal(pD, normal2),
                        new VertexTexture1(uvD))
                );
            }


            // 5. Créer la scène
            var scene = new SceneBuilder();

            foreach (var kvp in meshDict)
            {
                scene.AddRigidMesh(kvp.Value, Matrix4x4.CreateScale(1, -1, 1));
            }

            //5. export
            if(path != null)
            {
                scene.ToGltf2().SaveGLB( path);
            }
            else
            {
                scene.ToGltf2().SaveGLB(exportFolder + model.GetName());
            }
        }

        public static void ExportCharacterWithTexture_GLB(MAD charModel, List<Mtd> pngs)
        {

            string outputPath = gameFolder + "devtools/EXPORT/" + charModel.GetName();
            ModelRoot model = ModelRoot.CreateModel();
            List<MaterialBuilder> texList = new List<MaterialBuilder>();
            Dictionary<int, (MaterialBuilder Material, int Width, int Height)> materialDict = new Dictionary<int, (MaterialBuilder Material, int Width, int Height)>();
            var meshDict = new Dictionary<int, MeshBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>>();
            Dictionary<int, NodeBuilder> boneNodes = new Dictionary<int, NodeBuilder>();
            SceneBuilder scene = new SceneBuilder();


            //materials
            foreach (Mtd texture in pngs)
            {
                texture.textureTim = new TIM(texture.textureData);

                var png = texture.textureTim.ToPngBytes();
                texture.width = png.Item1;
                texture.height = png.Item2;

                MaterialBuilder mat = new MaterialBuilder()
                    .WithChannelImage(KnownChannel.BaseColor, png.Item3 )
                    .WithAlpha(SharpGLTF.Materials.AlphaMode.BLEND);

                materialDict[texture.indexNumber] = (mat, texture.width, texture.height);
            }

            //meshbuilder
            foreach (var texIdx in materialDict.Keys)
            {
                meshDict[texIdx] = new MeshBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>($"Mesh_{texIdx}");
            }

            //bones creation
            for (int i = 0; i < charModel.skeleton.Count; i++)
            {
                HIR bone = charModel.skeleton[i];

                // Crée une transformation locale
                var localTranslation = new Vector3(bone.TransformX, bone.TransformY, bone.TransformZ);

                Quaternion quat;

                if (bone.RotationX == 0 && bone.RotationY == 0 && bone.RotationZ == 0 && bone.RotationW == 0)
                {
                    quat = Quaternion.Identity;
                }
                else
                {
                    quat = Quaternion.Normalize(new Quaternion( bone.RotationX,bone.RotationY,bone.RotationZ,bone.RotationW ));
                }


                // Crée le node dans la hiérarchie
                NodeBuilder node = new NodeBuilder(BoneNames[i]);
                node.LocalMatrix = Matrix4x4.CreateFromQuaternion(quat) * Matrix4x4.CreateTranslation(localTranslation); ;              
                boneNodes[i] = node;
            }

            //bone hierarchy 
            for (int i = 0; i < charModel.skeleton.Count; i++)
            {
                int parentIndex = charModel.skeleton[i].boneParentIndex;

                if (parentIndex > 0)
                {
                    boneNodes[parentIndex].AddNode(boneNodes[i]); // Ajoute comme enfant
                }


            }


            //building vertex triangles and adding to mesh
            foreach (var tri in charModel.facData.triangleList)
            {
                var mesh = meshDict[tri.TextureIndex];
                var prim = mesh.UsePrimitive(materialDict[tri.TextureIndex].Material);

                Vertice vA = charModel.vtxData.verticesList[tri.Vertex_A];
                Vertice vB = charModel.vtxData.verticesList[tri.Vertex_B];
                Vertice vC = charModel.vtxData.verticesList[tri.Vertex_C];

                // Construction des positions des sommets
                Vector3 pA = new Vector3(vA.XOffset, vA.YOffset, vA.ZOffset);
                Vector3 pB = new Vector3(vB.XOffset, vB.YOffset, vB.ZOffset);
                Vector3 pC = new Vector3(vC.XOffset, vC.YOffset, vC.ZOffset);

                // Calcul des vecteurs de bord pour la normale du triangle
                Vector3 normal = Vector3.Normalize(Vector3.Cross(pB - pA, pC - pA));

                // Construction des UVs (à normaliser si nécessaire !)
                float textureWidth = (float)materialDict[tri.TextureIndex].Width;
                float textureHeight = (float)materialDict[tri.TextureIndex].Height;

                Vector2 uvA = new Vector2(tri.U_A / textureWidth, tri.V_A / textureHeight);
                Vector2 uvB = new Vector2(tri.U_B / textureWidth, tri.V_B / textureHeight);
                Vector2 uvC = new Vector2(tri.U_C / textureWidth, tri.V_C / textureHeight);

                prim.AddTriangle(
                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>(
                        new VertexPositionNormal(pA, normal),
                        new VertexTexture1(uvA),
                        new VertexJoints4(vA.BoneIndex) ),

                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>(
                        new VertexPositionNormal(pB, normal),
                        new VertexTexture1(uvB),
                        new VertexJoints4(vB.BoneIndex) ),

                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>(
                        new VertexPositionNormal(pC, normal),
                        new VertexTexture1(uvC),
                        new VertexJoints4(vC.BoneIndex) )
                );

            }

            foreach (var quad in charModel.facData.planeList)
            {
                var mesh = meshDict[quad.TextureIndex];
                var prim = mesh.UsePrimitive(materialDict[quad.TextureIndex].Material);

                Vertice vA = charModel.vtxData.verticesList[quad.Vertex_A];
                Vertice vB = charModel.vtxData.verticesList[quad.Vertex_B];
                Vertice vC = charModel.vtxData.verticesList[quad.Vertex_C];
                Vertice vD = charModel.vtxData.verticesList[quad.Vertex_D];

                Vector3 pA = new Vector3(vA.XOffset, vA.YOffset, vA.ZOffset);
                Vector3 pB = new Vector3(vB.XOffset, vB.YOffset, vB.ZOffset);
                Vector3 pC = new Vector3(vC.XOffset, vC.YOffset, vC.ZOffset);
                Vector3 pD = new Vector3(vD.XOffset, vD.YOffset, vD.ZOffset);

                float textureWidth = (float)materialDict[quad.TextureIndex].Width;
                float textureHeight = (float)materialDict[quad.TextureIndex].Height;

                Vector2 uvA = new Vector2(quad.U_A / textureWidth, quad.V_A / textureHeight);
                Vector2 uvB = new Vector2(quad.U_B / textureWidth, quad.V_B / textureHeight);
                Vector2 uvC = new Vector2(quad.U_C / textureWidth, quad.V_C / textureHeight);
                Vector2 uvD = new Vector2(quad.U_D / textureWidth, quad.V_D / textureHeight);

                // Calcul des vecteurs de bord
                Vector3 normal = Vector3.Normalize(Vector3.Cross(pB - pA, pC - pA));
                Vector3 normal2 = Vector3.Normalize(Vector3.Cross(pC - pA, pD - pA));

                prim.AddQuadrangle(
                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>(
                        new VertexPositionNormal(pA, normal2),
                        new VertexTexture1(uvA),
                        new VertexJoints4(vA.BoneIndex) ),

                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>(
                        new VertexPositionNormal(pB, normal),
                        new VertexTexture1(uvB),
                        new VertexJoints4(vB.BoneIndex) ),

                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>(
                        new VertexPositionNormal(pC, normal),
                        new VertexTexture1(uvC),
                        new VertexJoints4(vC.BoneIndex) ),

                    new VertexBuilder<VertexPositionNormal, VertexTexture1, VertexJoints4>(
                        new VertexPositionNormal(pD, normal),
                        new VertexTexture1(uvD),
                        new VertexJoints4(vD.BoneIndex) )
                );

            }


            foreach (var kvp in meshDict)
            {
                //find the bone number in the mesh ? 
                int boneindex = 0;
                scene.AddSkinnedMesh(kvp.Value, Matrix4x4.CreateScale(1, -1, 1), boneNodes[boneindex] );
            }
            

            /*
            // Créer la scène
            foreach (var kvp in meshDict)
            {
                scene.AddRigidMesh(kvp.Value, Matrix4x4.CreateScale(1, -1, 1));
            }

            */

            //export
            scene.ToGltf2().SaveGLB(outputPath);

        }

       
        //to do : add vertexColor1 for lighting but not mandatory ...
        public static void ExportTerrain_GLB(PMG mapTerrain, PTG mapTextures, string filepath)
        {
            ExportTerrain_GLB(mapTerrain, mapTextures).SaveGLB(filepath);
        }

        public static ModelRoot ExportTerrain_GLB(PMG mapTerrain, PTG mapTextures)
        {
            var material = new MaterialBuilder()
                .WithChannelImage(KnownChannel.BaseColor, TIM.ToPngBytes(mapTextures.CreateIMG(mapTerrain))).WithDoubleSide(true);

            var mesh = new MeshBuilder<VertexPosition, VertexTexture1, VertexEmpty>("TerrainMesh");
            var prim = mesh.UsePrimitive(material);
            //var prim = mesh.UsePrimitive( new MaterialBuilder().WithBaseColor( new Vector4(0.8f,0.8f,0.8f,1))  );


            for (int bx = 0; bx < 16; bx++)
            {
                for (int by = 0; by < 16; by++)
                {
                    int invBx = 15 - bx;
                    int invBy = 15 - by;

                    FileFormat.Block block = mapTerrain.blocks[invBx, invBy];

                    for (int tx = 0; tx < 4; tx++)
                    {
                        for (int ty = 0; ty < 4; ty++)
                        {
                            int globalTileX = invBx * 4 + tx;
                            int globalTileY = invBy * 4 + ty;

                            float x0 = globalTileX * 512;
                            float z0 = globalTileY * 512;
                            float x1 = x0 + 512;
                            float z1 = z0 + 512;

                            short h00 = block.HeightMap[tx, ty + 1].Height;
                            short h10 = block.HeightMap[tx + 1, ty + 1].Height;
                            short h11 = block.HeightMap[tx + 1, ty].Height;
                            short h01 = block.HeightMap[tx, ty].Height;

                            Vector3 vA = new Vector3(x0, h00, z1);
                            Vector3 vB = new Vector3(x1, h10, z1);
                            Vector3 vD = new Vector3(x1, h11, z0);
                            Vector3 vC = new Vector3(x0, h01, z0);


                            float uMax = ((63 - globalTileX) * 512f) / 32768f;
                            float uMin = ((64 - globalTileX) * 512f) / 32768f;
                            float vMin = ((63 - globalTileY) * 512f) / 32768f;
                            float vMax = ((64 - globalTileY) * 512f) / 32768f;

                            Vector2 uvA = new Vector2(uMax, vMin);
                            Vector2 uvB = new Vector2(uMin, vMin);
                            Vector2 uvC = new Vector2(uMax, vMax);
                            Vector2 uvD = new Vector2(uMin, vMax);

                            prim.AddQuadrangle(
                                new VertexBuilder<VertexPosition, VertexTexture1, VertexEmpty>(new VertexPosition(vA), new VertexTexture1(uvA)),
                                new VertexBuilder<VertexPosition, VertexTexture1, VertexEmpty>(new VertexPosition(vC), new VertexTexture1(uvC)),
                                new VertexBuilder<VertexPosition, VertexTexture1, VertexEmpty>(new VertexPosition(vD), new VertexTexture1(uvD)),
                                new VertexBuilder<VertexPosition, VertexTexture1, VertexEmpty>(new VertexPosition(vB), new VertexTexture1(uvB))
                            );

                        }
                    }
                }
            }


            var scene = new SceneBuilder();
            scene.AddRigidMesh(mesh, Matrix4x4.CreateScale(1, 1, -1));

            return scene.ToGltf2(); 
        }


        public static void ExportSkyboxes()
        {
            var filesList = Directory.GetFiles(gameFolder + "Skys/", "*", SearchOption.TopDirectoryOnly);
            Directory.CreateDirectory(exportFolder + "skys");

            foreach (string fichier in filesList)
            {
                string dest = exportFolder + "skys/" + Path.GetFileNameWithoutExtension(fichier) + ".png";
                if(File.Exists(dest) == false )
                {
                    if (fichier.Contains(".pmg") == true || fichier.Contains(".PMG") == true)
                    {
                        PMG pmg = new PMG(File.ReadAllBytes(fichier));

                        string fichier2 = Path.ChangeExtension(fichier, "ptg");

                        if (File.Exists(fichier2) == false)
                        {
                            fichier2 = filesList.FirstOrDefault(f => string.Equals(Path.GetFileName(f), fichier2, StringComparison.OrdinalIgnoreCase));
                        }

                        PTG ptg = new PTG(fichier2.Replace(".ptg", ""));
                        //File.WriteAllBytes( exportFolder + Path.GetFileNameWithoutExtension(fichier2) + ".glb", ExportSkybox_GLB(pmg, ptg));

                        ptg.CreateSkybox(pmg).Save(dest, ImageFormat.Png);
                    }
                }

            }

            /* //i think the pmg & ptg used in these folder are duplicate of /Skys/ pmg ptg files ?
            foreach( string dir in Directory.EnumerateDirectories(gameFolder + "Skys/", "*", SearchOption.AllDirectories))
            {
                var list = Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories);

                foreach (string fichier in list)
                {
                    if (fichier.Contains(".pmg") == true || fichier.Contains(".PMG") == true)
                    {
                        PMG pmg = new PMG(File.ReadAllBytes(fichier));

                        string fichier2 = Path.ChangeExtension(fichier, "ptg");

                        if(File.Exists(fichier2) == false)
                        {
                            fichier2 = list.FirstOrDefault(f => string.Equals(Path.GetFileName(f), fichier2, StringComparison.OrdinalIgnoreCase));
                        }

                        PTG ptg = new PTG(fichier2.Replace(".ptg","") );

                        ptg.CreateSkybox(pmg).Save(exportFolder +"skys/"+ Path.GetFileNameWithoutExtension(fichier) + ".png", ImageFormat.Png);

                    }
                }
            }*/


        }

        public static void ExportCharsFolder()
        {
            /*
             chars (special case in this folder ): 

             british.mad  <- base file containing all chars models (ace,sapper,spy etcc..)
             british.mtd  <- character textures (green)
             AMERICAN.MTD <- turquoise
             FRENCH.MTD   <- blue
             GERMAN.MTD   <- grey
             JAPANESE.MTD <- yellow
             RUSSIAN.MTD  <- red
             TEAMLARD.MTD <- purple
             
             FACES.MTD  <- chars faces for all chars models
             
             mcap.mad <- motion capture for all chars models
             pig.HIR <- model skeletion for all chars models

            //is SKYDOME.MAD 
            //thoses are mtd skyboxes files but names are in .mad (wtf i was so confused ?) 
             COLDSKY.MAD
             DESERT.MAD
             NIGHT1.MAD
             OMINOUS.MAD
             SPACE.MAD
             SUNNY.MAD
             SUNRISE.MAD
             SUNSET.MAD
             TOY.MAD
             WHITE.MAD
            */

            var list = Directory.EnumerateFiles(gameFolder + "Chars/", "*", SearchOption.AllDirectories);
            
            List<MAD> models = new List<MAD>();

            MAD skydome = new MAD();
            MAD skydomeU = new MAD();

            foreach (string modelName in MAD.GetModelListFromMad(gameFolder + "Chars/SKYDOME.MAD") )
            {
                if(modelName == "skydome")
                {
                    skydome = MAD.GetModelFromFullMAD(modelName, gameFolder + "Chars/SKYDOME.MAD");
                }
                if(modelName == "skydomeu")
                {
                    skydomeU = MAD.GetModelFromFullMAD(modelName, gameFolder + "Chars/SKYDOME.MAD");
                }
                
            }

            Directory.CreateDirectory(exportFolder + "/skydomes");
            Directory.CreateDirectory(exportFolder + "/characters");


            foreach (string fichier in list)
            {
                switch ( Path.GetFileName(fichier) ) 
                {
                    case "SKYDOME.MAD": //already added
                    case "british.mad": //character
                    
                    
                        //ignore for the moment
                        continue;


                    case "pig.HIR":
                        File.WriteAllText(exportFolder + "/characters/Pig.HIR.json",JsonSerializer.Serialize( HIR.GetSkeletonList(fichier) , new JsonSerializerOptions { WriteIndented = true } ));  ;
                        break;

                    case "mcap.mad":
                        File.WriteAllText(exportFolder + "/characters/motioncapture.json",JsonSerializer.Serialize( MotionCapture.GetMotionCaptureAnimations(fichier) , new JsonSerializerOptions { WriteIndented = true } ));  ;
                        break;

                    case "BRITHATS.MAD": //contains various classes hats without color 
                        
                        string destHats = exportFolder + "/characters/hats/";
                        Directory.CreateDirectory(destHats);
                        string hatsMtd = gameFolder + "/Chars/FHATS.MTD";

                        foreach (string modelName in MAD.GetModelListFromMad(fichier))
                        {
                            if (modelName == "br_med_h" || modelName == "br_sap_h"){ continue; }
                            string destHats2 = destHats + "/" + modelName + ".glb";

                            MAD m = MAD.GetModelFromFullMAD(modelName, fichier);
                            m.textures = Mtd.LoadTexturesFromMTD(m.facData, hatsMtd, true);

                            ExportModelWithOutTexture_GLB(m, destHats2);
                        }

                        continue;

                    case "FHATS.MAD": //contains heavy class hats of every teams
                        
                        destHats = exportFolder + "/characters/hats/";
                        hatsMtd = Path.ChangeExtension(fichier, "MTD");

                        foreach (string modelName in MAD.GetModelListFromMad(fichier))
                        {
                            string destHats2 = destHats + "/" + modelName + ".glb";

                            MAD m = MAD.GetModelFromFullMAD(modelName, fichier);
                            m.textures = Mtd.LoadTexturesFromMTD(m.facData, hatsMtd, true);

                            ExportModelWithTexture_GLB(m, destHats2);

                        }
                        break;


                    case "FACES.MTD":
                        
                        string faceFldr = exportFolder + "/characters/faces/";
                        Directory.CreateDirectory(faceFldr);

                        byte[] mtdData = File.ReadAllBytes(fichier);
                        int endContenTable = BitConverter.ToInt32(mtdData, 16); //the first item offset define table content size ! 

                        for (int i = 0; i <= endContenTable; i++)
                        {
                            int endblockContentTable = i + 24;
                            if (endblockContentTable <= endContenTable)
                            {
                                Mtd tempTex = new Mtd
                                {
                                    Name = Encoding.ASCII.GetString(mtdData[i..(i + 16)]).Trim('\0'),
                                    DataOffset = BitConverter.ToInt32(mtdData[(i + 16)..(i + 20)]),
                                    DataSize = BitConverter.ToInt32(mtdData[(i + 20)..(i + 24)]),
                                };
                                tempTex.textureTim = new TIM(mtdData[tempTex.DataOffset..(tempTex.DataOffset + tempTex.DataSize)]);
                                tempTex.textureTim.ToBitmap().Save(faceFldr + tempTex.Name+".png",ImageFormat.Png);
                            }
                            i += 23;
                        }
                        
                        break;




                    case "COLDSKY.MAD":
                    case "DESERT.MAD":
                    case "NIGHT1.MAD":
                    case "OMINOUS.MAD":
                    case "SPACE.MAD":
                    case "SUNNY.MAD":
                    case "SUNRISE.MAD":
                    case "SUNSET.MAD":
                    case "TOY.MAD":
                    case "WHITE.MAD":
                        
                        string destDome = exportFolder + "skydomes/skydome_" + Path.GetFileNameWithoutExtension(fichier) + ".glb";
                        if(File.Exists(destDome) ==false )
                        {
                            skydome.textures = Mtd.LoadTexturesFromMTD(skydome.facData, fichier, true);
                            skydomeU.textures = Mtd.LoadTexturesFromMTD(skydomeU.facData, fichier, true);
                            ExportModelWithTexture_GLB(skydome, destDome);
                            ExportModelWithTexture_GLB(skydomeU, destDome.Replace("skydome_", "skydomeu_") );
                        }
                        continue;


                    
                    case "WEAPONS.MAD":
                        
                        string dest = exportFolder + Path.GetFileNameWithoutExtension(fichier);
                        Directory.CreateDirectory(dest );
                        string weaponsMtd = Path.ChangeExtension(fichier, "MTD");

                        foreach (string modelName in  MAD.GetModelListFromMad(fichier) )
                        {
                            string dest2 = dest + "/" + modelName + ".glb";
                            if(File.Exists(dest2) == false)
                            {
                                MAD m = MAD.GetModelFromFullMAD(modelName, fichier);
                                m.textures = Mtd.LoadTexturesFromMTD(m.facData, weaponsMtd, true);

                                ExportModelWithTexture_GLB(m, dest2 );
                            }

                        }
                        break;

                    case "PROPOINT.MAD":
                    case "SIGHT.MAD":
                    case "TOP.MAD":
                        
                        string destFolder = exportFolder + "others/";

                        Directory.CreateDirectory(destFolder);

                        destFolder += Path.GetFileNameWithoutExtension(fichier);

                        foreach (string modelName in MAD.GetModelListFromMad(fichier))
                        {
                            MAD m = MAD.GetModelFromFullMAD(modelName, fichier);

                            string tg = list.FirstOrDefault(f => f.Equals(Path.ChangeExtension(fichier, "mtd"), StringComparison.OrdinalIgnoreCase));

                            m.textures = Mtd.LoadTexturesFromMTD(m.facData, tg, true);

                            ExportModelWithTexture_GLB(m, destFolder + ".glb");
                        }
                        break;

                }

            }
            
        }


        public static void ExportAudioAndSounds()
        {
            string dest = exportFolder + "Audio/";
            Directory.CreateDirectory(dest);
            
            foreach (string fileName in Directory.GetFiles(gameFolder+"Audio", "*.wav"))
            {
                string dest2 = dest + Path.GetFileNameWithoutExtension(fileName) + ".opus";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "D:\\projects devs\\hogs_gameManager_wpf/ffmpeg.exe",
                    Arguments = $"-y -i \"{fileName}\"  -c:a libopus -b:a 48k -ac 1  \"{dest2}\"",
                    UseShellExecute = false,     // obligatoire pour rediriger ou cacher
                    CreateNoWindow = true,       // aucune fenêtre cmd
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                };

                using (Process ffmpeg = Process.Start(psi))
                {
                    ffmpeg.WaitForExit(); // attend la fin de la conversion
                }
            }


            dest = exportFolder + "/Audio/ui/";
            Directory.CreateDirectory(dest);

            foreach (string fileName in Directory.GetFiles(gameFolder + "FESounds/", "*.wav"))
            {
                string dest2 = dest + Path.GetFileNameWithoutExtension(fileName) + ".opus";

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "D:\\projects devs\\hogs_gameManager_wpf/ffmpeg.exe",
                    Arguments = $"-y -i \"{fileName}\"  -c:a libopus -b:a 48k -ac 1  \"{dest2}\"",
                    UseShellExecute = false,     // obligatoire pour rediriger ou cacher
                    CreateNoWindow = true,       // aucune fenêtre cmd
                    RedirectStandardOutput = false,
                    RedirectStandardError = false
                };

                using (Process ffmpeg = Process.Start(psi))
                {
                    ffmpeg.WaitForExit(); // attend la fin de la conversion
                }
            }
            
            dest = exportFolder + "/Audio/speech/";
            Directory.CreateDirectory(dest);


            foreach (string dir in Directory.GetDirectories(gameFolder + "Speech/Sku1/"))
            {
                string dirToCreate = exportFolder + "/Audio/speech/" + Path.GetFileNameWithoutExtension(dir);
                Directory.CreateDirectory(dirToCreate);

                foreach (string fileName in Directory.GetFiles(dir, "*.wav"))
                {
                    string dest2 = dirToCreate + "/"  + Path.GetFileNameWithoutExtension(fileName) + ".opus";

                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "D:\\projects devs\\hogs_gameManager_wpf/ffmpeg.exe",
                        Arguments = $"-y -i \"{fileName}\"  -c:a libopus -b:a 48k -ac 1  \"{dest2}\"",
                        UseShellExecute = false,     // obligatoire pour rediriger ou cacher
                        CreateNoWindow = true,       // aucune fenêtre cmd
                        RedirectStandardOutput = false,
                        RedirectStandardError = false
                    };

                    using (Process ffmpeg = Process.Start(psi))
                    {
                        ffmpeg.WaitForExit(); // attend la fin de la conversion
                    }
                }

            }



        }

        public static void ExportMapsAndModels()
        {
            Directory.CreateDirectory(exportFolder + "models");
            Directory.CreateDirectory(exportFolder + "maps");
            //export every map 
            foreach (string fileName in Directory.GetFiles(mapsFolder, "*.MAD"))
            {
                string fileNameB = Path.GetFileNameWithoutExtension(fileName);

                var pogs = POG.GetAllMapObject(fileNameB);
                string loc;

                //exporting models of a map
                MAD.GetMapEntitiesList(fileNameB).ForEach(entityName =>
                {
                    loc = exportFolder + "models/" + entityName + ".glb";

                    if (File.Exists(loc) == false)
                    {
                        if (modelsWithMultipleSkins.Contains(entityName) == true )
                        {
                            POG pog = pogs.Find(x => x.GetName() == entityName);
                            if(pog != null)
                            {
                                loc = exportFolder + "models/" + pog.GetName() + "_" + pog.type + ".glb";
                            }
                        }
                    
                        MAD model = MAD.GetModelFromMAD(entityName, fileNameB);
                        if (model.facData != null)
                        {
                            model.textures = Mtd.LoadTexturesFromMTD(model.facData, fileNameB);
                            ExportModelWithTexture_GLB(model, loc);
                        }
                    }
                });

                //exporting maps
                if (fileNameB.Substring(0, 3) != "GEN") //ignore multiplayer generator maps they are not supported
                {
                    loc = exportFolder + "/maps/" + fileNameB;

                    PMG pmg = new PMG(mapsFolder + fileNameB);
                    PTG ptg = new PTG(mapsFolder + fileNameB);

                    ExportTerrain_GLB(pmg, ptg, loc+".glb");

                    File.WriteAllText(loc + ".json", JsonSerializer.Serialize(pogs.Select(p => p.POG2JSON()), new JsonSerializerOptions { WriteIndented = true }));

                    ptg.CreateIMG(pmg).Save(loc + ".png", ImageFormat.Png);
                    
                }
            }
        }

        public static void ConvertFontsToTTF() //apparently i am stuck here forever, there is no library that convert vectors to TTF ... 
        {

            Directory.CreateDirectory(exportFolder + "fonts");
            string loc = gameFolder + "FEText/";

            foreach (string fileName in Directory.GetFiles(loc, "*.tim")) 
            {
                Bitmap font = new TIM(File.ReadAllBytes(fileName) ).ToBitmap();
                TAB tab = new TAB(File.ReadAllBytes( fileName.Replace(".tim",".tab") ));

                int charcode = 65;
                foreach (var letter in tab.all_letters)
                {
                    using (Bitmap glyphBitmap = font.Clone(new Rectangle(letter.XOffset, letter.YOffset, letter.Width, letter.Height), font.PixelFormat))
                    {
                        var potraceBitmap = PotraceBitmap.Create(letter.Width, letter.Height);

                        // Remplissez le PotraceBitmap avec les données de votre lettre
                        for (int y = 0; y < letter.Height; y++)
                        {
                            for (int x = 0; x < letter.Width; x++)
                            {
                                potraceBitmap.SetColor(x, y, glyphBitmap.GetPixel(x, y).R < 128 ? true : false);
                                PotraceState trace = Potrace.Trace(new PotraceParam(), potraceBitmap);
                            }
                        }


                    }

                }
            }



        }


        public static void ExportLanguages()
        {

            string loc = exportFolder + "ui/";
            Directory.CreateDirectory(loc);

            var fichiers = Directory.GetFiles(gameFolder + "Language/Tims/","*.mtd",SearchOption.AllDirectories)
                   .Concat(Directory.GetFiles(gameFolder + "Language/Tims/", "*.mad", SearchOption.AllDirectories)).ToArray(); ;


            foreach (string fileName in fichiers )
            {

                byte[] mtdData = File.ReadAllBytes(fileName);
                int endContenTable = BitConverter.ToInt32(mtdData, 16); //the first item offset define table content size ! 

                for (int i = 0; i <= endContenTable; i++)
                {
                    int endblockContentTable = i + 24;
                    if (endblockContentTable <= endContenTable)
                    {
                        Mtd tempTex = new Mtd
                        {
                            Name = Encoding.ASCII.GetString(mtdData[i..(i + 16)]).Trim('\0'),
                            DataOffset = BitConverter.ToInt32(mtdData[(i + 16)..(i + 20)]),
                            DataSize = BitConverter.ToInt32(mtdData[(i + 20)..(i + 24)]),

                        };

                        if(tempTex.Name.Contains(".tim") || tempTex.Name.Contains(".TIM") )
                        {
                            tempTex.textureTim = new TIM(mtdData[tempTex.DataOffset..(tempTex.DataOffset + tempTex.DataSize)]);
                            tempTex.textureTim.ToBitmap().Save(loc + tempTex.Name + ".png", ImageFormat.Png);
                        }
                        else if (tempTex.Name.Contains(".bmp") || tempTex.Name.Contains(".TIM"))
                        {
                            new Bitmap(new MemoryStream(mtdData[tempTex.DataOffset..(tempTex.DataOffset + tempTex.DataSize)])).Save(loc + tempTex.Name, ImageFormat.Bmp);
                        }

                    }
                    i += 23;
                }
                

            }

            fichiers = Directory.GetFiles(gameFolder + "Language/Tims/", "*.tim", SearchOption.AllDirectories);

            foreach (string fileName in fichiers )
            {
                new TIM(File.ReadAllBytes(fileName)).ToBitmap().Save(loc+Path.GetFileNameWithoutExtension(fileName) + ".png",ImageFormat.Png);
            }

        }

        public static void Export_FEBmps() //a list of .mgl files , those are LZ77 compressed bmp 
        {
            Directory.CreateDirectory(exportFolder+"FEBMP/");

            byte[] febmps = File.ReadAllBytes(gameFolder + "FEBmps/FEBMP.MAD");

            int endContenTable = BitConverter.ToInt32(febmps, 16); 

            for (int i = 48; i <= endContenTable; i++)
            {
                int endblockContentTable = i + 24;
                if (endblockContentTable <= endContenTable)
                {
                    Mtd mtd =  new Mtd(febmps[i..(i + 24)]);
                    if(mtd.Name == "propoint.mgl")
                    {
                        //byte[] decompressed = MGL.DecompressAuto(febmps[mtd.DataOffset..(mtd.DataOffset + mtd.DataSize)]);
                        byte[] b = MGL.Decompress(febmps[mtd.DataOffset..(mtd.DataOffset + mtd.DataSize)]);
                        File.WriteAllBytes(exportFolder + mtd.Name + ".BMP", b);
                    }

                }
                i += 23;
            }
        }

        public static void MadMtdModdingTool()
        {
            /**
             * i have a theory, what happens if i replace every Mad+mtd file of each map by  
             * the same mad+mtd file, a verified working and build by hand tht contans absolutely every model of the game 
             * every map will load the same file, in that way, no need to recalculate specificly for a model + texture + fac index etcc 
             * i suppose when the game engine load a map pog, it will search for models in the mad but won't load everything ? 
             * otherwise we don't care, pc are powerfull enough nowadays i don't think it will impact something ?
            */

            /**
             * Step by step : building an AllInOne MAD & MTD 
             * 
             * foreach file in maps, load each model
             * when loading model, load textures 
             * assign a new "index number" to textures (1, 2 ,3 ,4...)
             * re-index a first time model fac according to textures new indexes
             * once every model is loaded in a list, re-loop all of them
             * build a MAD parallel to mtd 
             * before injecting textures into mtd, count how many texture exist in the modded mtd, 
             * apply count offset on both fac indexes and textures indexes
             * inject mtd and mad 
             * end loop
             * */

            //result , it worked absolutely fine, need to make two version, one for snow, one for not snow : ) 

            List<MAD> allModelsOfTheGame = new List<MAD>();

            foreach (string fileName in Directory.GetFiles(mapsFolder, "*.MAD"))
            {
                string fileNameB = Path.GetFileNameWithoutExtension(fileName);

                MAD.GetMapEntitiesList(fileNameB,false).ForEach(entityName =>
                {
                    MAD model = MAD.GetModelFromMAD(entityName, fileNameB);
                    if( allModelsOfTheGame.Exists( x=> x.Name == model.Name) ==false )
                    {
                        if (model.facData != null)
                        {
                            model.textures = Mtd.LoadTexturesFromMTD(model.facData, fileNameB);
                            model.ReIndexFacWithTextures();

                            allModelsOfTheGame.Add(model);
                        }
                    }

                });
            }
            //every model is now loaded in a list
            //construct both mad and mtd header ?

            byte[] finalMadBytes;
            byte[] finalMtdBytes;

            int texCounter = 0; //texture offset to apply on fac and mtd

            int modelOffset = 72 * allModelsOfTheGame.Count;
            int textureOffset =  24 * allModelsOfTheGame.Sum(model => model.textures.Count);

            using (var msMad = new MemoryStream())
            using (var msMtd = new MemoryStream())
            using (var writerMad = new BinaryWriter(msMad))
            using (var writerMtd = new BinaryWriter(msMtd))
            {
                foreach (MAD mad in allModelsOfTheGame)
                {
                    //need to filter if map is snow , then create another mad+mtd

                    // --- Flux 1 (MAD) ---
                    writerMad.Write(Encoding.ASCII.GetBytes((mad.Name + ".VTX").PadRight(16, '\0')));
                    writerMad.Write(modelOffset);
                    modelOffset += mad.DataSizes[0];
                    writerMad.Write(mad.DataSizes[0]);

                    writerMad.Write(Encoding.ASCII.GetBytes((mad.Name + ".NO2").PadRight(16, '\0')));
                    writerMad.Write(modelOffset);
                    modelOffset += mad.DataSizes[1];
                    writerMad.Write(mad.DataSizes[1]);
                    
                    writerMad.Write(Encoding.ASCII.GetBytes((mad.Name + ".FAC").PadRight(16, '\0')));
                    writerMad.Write(modelOffset);
                    modelOffset += mad.DataSizes[2];
                    writerMad.Write(mad.DataSizes[2]);
                    

                    // --- Flux 2 (MTD) ---
                    foreach (Mtd tex in mad.textures)
                    {
                        writerMtd.Write(Encoding.ASCII.GetBytes(tex.Name.PadRight(16, '\0')));
                        writerMtd.Write(textureOffset);
                        textureOffset += tex.DataSize;
                        writerMtd.Write(tex.DataSize);

                        // apply new TextureIndex with texCounter offset on fac and textures 
                        mad.facData.triangleList.ForEach(triangle =>
                        {
                            if (triangle.TextureIndex == tex.indexNumber)
                            {
                                triangle.TextureIndex = texCounter;
                            }
                        });

                        mad.facData.planeList.ForEach(plane =>
                        {
                            if (plane.TextureIndex == tex.indexNumber)
                            {
                                plane.TextureIndex = texCounter;
                            }
                        });

                        tex.indexNumber = texCounter;
                        texCounter++;
                    }
                }

                //reloop AGAIN to inject data on both mad and mtd , if we did things correctly, current BinaryWriters are right at the first model "dataoffset"
                foreach (MAD mad in allModelsOfTheGame)
                {
                    writerMad.Write( mad.GoBackToMonke() );

                    foreach (Mtd tex in mad.textures)
                    {
                        writerMtd.Write(tex.textureTim.SerializeToBinary());
                    }
                }

                //get the results of memorystreams
                finalMadBytes = msMad.ToArray();
                finalMtdBytes = msMtd.ToArray();

                msMad.Flush();
                msMtd.Flush();

            }

            File.WriteAllBytes(exportFolder + "all.MAD", finalMadBytes);
            File.WriteAllBytes(exportFolder + "all.MTD", finalMtdBytes);

        }


    }





}

