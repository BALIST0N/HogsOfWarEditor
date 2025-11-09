using hogs_gameEditor_wpf;
using hogs_gameEditor_wpf.FileFormat;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.System;
using Xceed.Wpf.Toolkit.PropertyGrid;


namespace hogs_gameManager_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, string> MapList;
        public List<POG> CurrentMap;
        string CurrentMapName;
        public bool mapObjectEdited = false;
        public Dictionary<string, List<string>> TableOfTextureAdded;

        public MainWindow()
        {
            InitializeComponent();

            #region MapList Fillings
            this.MapList = new Dictionary<string, string>
            {
                { "You Hillock", "ARCHI" },
                { "Doomed", "ARTGUN" },
                { "15: Fortified Swine", "BAY" },
                { "Dam Busters", "BHILL" },
                { "Friendly Fire", "BOOM" },
                { "17: Geneva Convention", "BRIDGE" },
                { "Moon Buttes", "BUTE" },
                { "00: Boot Camp", "CAMP" },
                { "Cratermass", "CMASS" },
                { "Graveyard Shift", "CREEPY2" },
                { "Death Bowl", "DBOWL" },
                { "04 Beta", "DEMO" },
                { "Ice Flow Beta", "DEMO2" },
                { "18: I Spy", "DESVAL" },
                { "04: Morning Glory", "DEVI" },
                { "Death Valley Beta", "DVAL" },
                { "Death Valley", "DVAL2" },
                { "15 Beta", "EASY" },
                { "20: Achilles Heal", "EMPLACE" },
                { "01: The War Foundation", "ESTU" },
                { "14: Battle Stations", "EYRIE" },
                { "25: Well, Well, Well!", "FINAL" },
                { "13: Glacier Guns", "FJORDS" },
                { "24: Hamburger Hill", "FOOT" },
                { "10: Bangers 'N' Mash", "GUNS" },
                { "Pigin' Hell", "HELL2" },
                { "Skulduggery", "HELL3" },
                { "Completely unused", "HILLBASE" },
                { "Chill Hill", "ICE" },
                { "Ice Flow", "ICEFLOW" },
                { "Island Hopper", "ISLAND" },
                { "22: Assassination", "KEEP" },
                { "The Lake", "LAKE" },
                { "Bridge The Gap", "LECPROD" },
                { "11: Saving Private Rind", "LIBERATE" },
                { "Pigs in Space", "LUNAR1" },
                { "09: The Village People", "MASHED" },
                { "Hedge Maze", "MAZE" },
                { "16: Over The Top", "MEDIX" },
                { "Frost Fight", "MLAKE" },
                { "12: Just Deserts", "OASIS" },
                { "One Way System", "ONEWAY" },
                { "Play Pen", "PLAY1" },
                { "Duvet Fun", "PLAY2" },
                { "Ridge Back", "RIDGE" },
                { "02: Routine Patrol", "ROAD" },
                { "05: Island Invasion", "RUMBLE" },
                { "Square Off", "SEPIA1" },
                { "19: Chemical Compound", "SNAKE" },
                { "08: The Spying Game", "SNIPER" },
                { "21: High And Dry", "SUPLINE" },
                { "23: Hero Warship", "TESTER" },
                { "03: Trench Warfare", "TRENCH" },
                { "07: Communication Breakdown", "TWIN" },
                { "06: Under Siege", "ZULUS" }
            };

            this.MapList = MapList.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            #endregion

            #region instance texure/models

            this.TableOfTextureAdded = new Dictionary<string, List<string>>()
            {
                {"AMLAUNCH", new List<string>()
                    {"AM-SIDE3.TIM",
                    "AM-WEELS.TIM",
                    "AM-SIDE2.TIM",
                    "AM-TOPA1.TIM",
                    "AM-BACK.TIM",
                    "AM-TOP3.TIM",
                    "AM-SIDE.TIM",
                    "AM-TOP2.TIM",
                    "AM-TOP1.TIM",
                    "AM-UNDA2.TIM",
                    "AM-TOP4.TIM",
                    "AM-FNTL.TIM",
                    "AM-UNDA1.TIM",
                    "AM-HACH2.TIM",
                    "AM-HACH1.TIM",
                    "AM-FNTR.TIM"}
                },
                {"TANK", new List<string>()
                    {"TANK1000.TIM",
                    "TANK1001.TIM",
                    "TANK1002.TIM",
                    "TANK1003.TIM",
                    "TANK1004.TIM",
                    "TANK1005.TIM",
                    "TANK1006.TIM",
                    "TANK1008.TIM",
                    "TANK1007.TIM",
                    "TANK1009.TIM",
                    "TANK1010.TIM",
                    "TANK1011.TIM",
                    "TANK1012.TIM",
                    "TANK1013.TIM"}
                },
                {"PILLBOX", new List<string>()
                    {"L-FRONT2.TIM",
                    "L-TOP4.TIM",
                    "L-SIDE3.TIM",
                    "L-TOP5.TIM",
                    "L-TOP2.TIM",
                    "L-SIDE4.TIM",
                    "L-FRONT1.TIM",
                    "L-SIDE2.TIM",
                    "L-SIDE1.TIM",
                    "L-TUR1.TIM",
                    "L-TUR2.TIM",
                    "L-BACK1.TIM",
                    "L-TOP6.TIM",
                    "L-TOP3.TIM"}
                },

                {"BIG_GUN", new List<string>()
                    {"BIGN002.TIM",
                    "BIGN005.TIM",
                    "BIGN006.TIM",
                    "BIGN001.TIM",
                    "BIGN006.TIM",
                    "BIGN005.TIM",
                    "BIGN006.TIM",
                    "BIGN005.TIM",
                    "BIGN013.TIM",
                    "BIGN015.TIM",
                    "BIGN012.TIM",
                    "BIGN010.TIM",
                    "BIGN011.TIM",
                    "BIGN004.TIM",
                    "BIGN003.TIM",
                    "BIGN016.TIM",
                    "BIGN009.TIM",
                    "BIGN008.TIM",
                    "BIGN007.TIM",
                    "BIGN000.TIM"}
                },
                {"CARRY", new List<string>()
                    {"AM-SIDE3.TIM",
                    "AM-WEELS.TIM",
                    "AM-SIDE2.TIM",
                    "AM-TOPA1.TIM",
                    "AM-BACK.TIM",
                    "AM-TOP3.TIM",
                    "AM-SIDE.TIM",
                    "AM-TOP2.TIM",
                    "AM-TOP1.TIM",
                    "AM-UNDA2.TIM",
                    "AM-TOP4.TIM",
                    "AM-FNTL.TIM",
                    "AM-UNDA1.TIM",
                    "AM-HACH2.TIM",
                    "AM-HACH1.TIM",
                    "AM-FNTR.TIM"}
                },
                {"DRUM", new List<string>()
                    {"DRUM000.TIM",
                    "DRUM001.TIM",
                    "DRUM002.TIM"}
                },
                {"DRUM2", new List<string>()
                    {"DRUM000.TIM",
                    "DRUM001.TIM",
                    "DRUM003.TIM"}
                },
                {"M_TENT1", new List<string>()
                    {"T_M002.TIM",
                    "T_M000.TIM",
                    "T_M001.TIM",
                    "T_M005.TIM"}
                },
                {"M_TENT2", new List<string>()
                    {"T_M002.TIM",
                    "T_M000.TIM",
                    "T_M001.TIM",
                    "T_M005A.TIM"
                    }
                },
                {"TENT_S", new List<string>()
                    {"T_S001.TIM",
                    "T_S002.TIM",
                    "T_S000.TIM"
                    }
                },
                {"SHELTER", new List<string>()
                    {"SHEL_1.TIM",
                    "SHEL_2.TIM",
                    "SHEL_5.TIM",
                    "SHEL_3.TIM",
                    "SHEL_4.TIM"}
                },
                {"AM_TANK", new List<string>()
                    {"AMTA007.TIM",
                    "AMTA006.TIM",
                    "AMTA005.TIM",
                    "AMTA004.TIM",
                    "AMTA000.TIM",
                    "AMTA003.TIM",
                    "AMTA002.TIM",
                    "AMTA001.TIM"}
                }
            };

            #endregion

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.mapListComboBox.ItemsSource = MapList.Keys;
            this.MapObjectsListView.KeyUp += MapObjectsListView_KeyUp;

        }

        public void MapListComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if( this.mapListComboBox.SelectedIndex != -1 )
            {
                if (e != null && e.RemovedItems.Count > 0 && mapObjectEdited)
                {
                    if (MessageBox.Show("would You like to save your data on this map ?", "Attention", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                    {
                        POG.ExportMapPOG(this.CurrentMap, this.CurrentMapName);
                    }
                }

                //clear to avoid Exceptions
                this.MapObjectPropertiesControl.SelectedObject = null;
                this.MapObjectsListView.Items.Clear();
                this.CanvasImageMap.Children.Clear();
                this.mapObjectEdited = false;
                this.CurrentMapName = MapList.ElementAt(mapListComboBox.SelectedIndex).Value;
                this.buttonExportEntity.Content = "";
                this.buttonMapExport.Content = "Export " + this.CurrentMapName;

                //Read the File
                this.CurrentMap = POG.GetAllMapObject(this.CurrentMapName);
                foreach ( POG mo in this.CurrentMap)
                {
                    this.MapObjectsListView.Items.Add(newItem: new MapObjectsListViewItem { Name = GlobalVars.Name_Converter(mo.name), Id = Convert.ToString(mo.index), Team = Convert.ToString(mo.team) });  //this is just adding a row on the listbox
                }

                this.MapImageControl.Source = new BitmapImage(new Uri("file://" + GlobalVars.mapsViewsFolder + CurrentMapName + ".png")); //loading the center map

                //generate buttons with icons in the minimap
                LoadMapObjects();

                //MadMtdModdingTool();

            }
        }

        private void MapObjectsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)    //click on a different map object
        {
            if(this.MapObjectsListView.SelectedIndex != -1)
            {
                this.MapObjectPropertiesControl.SelectedObject = CurrentMap[this.MapObjectsListView.SelectedIndex];
                this.MapObjectPropertiesControl.SelectedObjectName = new string( CurrentMap[this.MapObjectsListView.SelectedIndex].name);  //"new string" cuz char[]
                this.MapObjectPropertiesControl.SelectedObjectTypeName = "Object n°" + this.MapObjectsListView.SelectedIndex.ToString();
                this.MapObjectPropertiesControl.Update();
                this.MapObjectPropertiesControl.ExpandAllProperties();
                this.buttonExportEntity.Content = "Export " + this.MapObjectPropertiesControl.SelectedObjectName;
            }
        }

        private void MapObjectsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && MapObjectsListView.SelectedIndex != -1)
            {
                MapObjectsListViewItem molv =  (MapObjectsListViewItem)MapObjectsListView.SelectedItem;
                MessageBoxResult res = MessageBox.Show("are you sure you want to delete Object n°" + molv.Id + " (" + molv.Name + ") ","A T T E N T I O N ",MessageBoxButton.YesNo);
                if(res == MessageBoxResult.Yes)
                {
                    CurrentMap.Remove( CurrentMap.Find(x => x.index == Convert.ToInt16( molv.Id) ) );
                    this.MapObjectsListView.Items.Remove(molv);

                    this.CanvasImageMap.Children.Clear();
                    LoadMapObjects();
                    
                    this.mapObjectEdited = true;
                }
            }
        }

        public void LoadMapObjects()
        {
            //generate buttons with icons in the minimap
            foreach (POG mo in CurrentMap)
            {
                LoadMapObject(mo);
            }
        }

        public void LoadMapObject(POG mo)
        {
            switch (mo.GetName())    //check the mapobject name and draw it differently accoring to his name
            {
                case "AC_ME": //ace except its legend
                case "CO_ME": //commando ?
                case "GR_ME": //grunt 
                case "HV_ME": //heavy
                case "LE_ME": //legend except is hero
                case "ME_ME": //medic
                case "SA_ME": //sapper
                case "SB_ME": //spy
                case "SN_ME": //scout
                case "SP_ME": //sniper
                    GenerateAndSpawnCharacterMapButton(mo);
                    break;

                case "DRUM":
                    GenerateObjectMapButton(mo, Brushes.DarkOrange, Brushes.Crimson);
                    break;
                case "DRUM2":
                    GenerateObjectMapButton(mo, Brushes.GreenYellow, Brushes.LawnGreen);
                    break;

                case "CRATE1":
                case "CRATE4":
                    GenerateObjectMapButton(mo, Brushes.DarkGoldenrod);

                    break;

                case "CRATE2":
                    GenerateObjectMapButton(mo, Brushes.DeepPink, Brushes.Indigo);

                    break;

                case "PROPOINT":
                    GenerateObjectMapButton(mo, Brushes.Yellow, Brushes.Gold);
                    break;

                case "AM_TANK":
                case "CARRY":
                case "TANK":
                case "AMLAUNCH":
                    GenerateObjectMapButton(mo, Brushes.Gray, Brushes.Blue);
                    break;

                case "BIG_GUN":
                    GenerateObjectMapButton(mo, Brushes.Maroon, Brushes.DarkBlue);
                    break;

                case "PILLBOX":
                    GenerateObjectMapButton(mo, Brushes.Wheat, Brushes.White);
                    break;

                case "M_TENT1":
                case "M_TENT2":
                case "TENT_S":
                    GenerateObjectMapButton(mo, Brushes.Green, Brushes.Pink);
                    break;

                case "SHELTER":
                    GenerateObjectMapButton(mo, Brushes.Gray, Brushes.Orange);
                    break;

                default:
                    GenerateObjectMapButton(mo, new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)), Brushes.Transparent);
                    break;
            }
        }

        private Rectangle GenerateObjectMapButton(POG mo)
        {
            //MessageBox.Show(CurrentMap.IndexOf(mo).ToString() + " '\n\r" + mo.position[0] + " " + mo.position[1] + " '\n\r" + Math.Round(mo.position[0] / 72.81, 2) + " " + Math.Round(mo.position[1] / 72.81, 2));

            Rectangle b = new Rectangle
            {
                Name = "n" + CurrentMap.IndexOf(mo).ToString(),
                Width = 9,
                Height = 9,
                StrokeThickness = 1.2,
                Stroke = Brushes.Black,
                VerticalAlignment = VerticalAlignment.Center,
            };
            b.MouseDown += B_Click;
            return b;
        }

        private void GenerateAndSpawnCharacterMapButton(POG mo)
        {
            var X = new System.Windows.Shapes.Path
            {
                StrokeThickness = 1,
                Stroke = Brushes.Black,
                Name = "n" + CurrentMap.IndexOf(mo).ToString(),
                Data = Geometry.Parse("M 2.4,0 L 6,3.6 L 9.6,0 L 12,2.4 L 8.4,6 L 12,9.6 L 9.6,12 L 6,8.4 L 2.4,12 L 0,9.6 L 3.6,6 L 0,2.4 Z"),
                Width = 12,
                Height = 12,
                VerticalAlignment = VerticalAlignment.Center,
            };
            switch(mo.team)
            {
                case POG.PigTeam.Team01:
                    X.Fill = new SolidColorBrush(Color.FromArgb(255,0,255,0));
                    break;

                case POG.PigTeam.Team02:
                    X.Fill = new SolidColorBrush(Color.FromArgb(255, 0,0,255));
                    break;

                case POG.PigTeam.Team03:
                    X.Fill = Brushes.Turquoise;
                    break;

                case POG.PigTeam.Team04:
                    X.Fill = Brushes.Red;
                    break;

                case POG.PigTeam.Team05:
                    X.Fill = Brushes.Yellow;
                    break;

                case POG.PigTeam.Team06:
                    X.Fill = Brushes.White;
                    break;

                case POG.PigTeam.Team07:
                    X.Fill = Brushes.Purple;
                    break;
                case POG.PigTeam.Team08:
                    X.Fill = Brushes.Magenta;
                    break;
            }

            X.MouseDown += B_Click;

            this.CanvasImageMap.Children.Add(X);
            Canvas.SetLeft(X, (mo.position[0] / 64) + 251);
            Canvas.SetTop(X, -(mo.position[2] / 64) + 251);

        }
        private void GenerateObjectMapButton(POG mo,Brush backColor)
        {
            Rectangle b = GenerateObjectMapButton(mo);
            b.Fill = backColor;
            SpawnObjectMapRectangle(b, mo);

        }
        private void GenerateObjectMapButton(POG mo, Brush backColor,Brush bordercolor)
        {
            
            Rectangle b = GenerateObjectMapButton(mo);
            b.Fill = backColor;
            b.Stroke = bordercolor;
            SpawnObjectMapRectangle(b, mo);
        }

        private void SpawnObjectMapRectangle(Rectangle R, POG mo)
        {
            double x = mo.position[0]/64;
            double y = -mo.position[2]/64;

            this.CanvasImageMap.Children.Add(R);
            Canvas.SetLeft(R, x+251);
            Canvas.SetTop(R, y+251);
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            Shape b = (Shape)sender;
            this.MapObjectPropertiesControl.PropertyValueChanged -= MapObjectPropertiesControl_PropertyValueChanged;
            this.MapObjectsListView.SelectedIndex = Convert.ToInt32( b.Name.Replace("n", String.Empty) );
            this.MapObjectsListView.ScrollIntoView(this.MapObjectsListView.Items[this.MapObjectsListView.SelectedIndex]);
            this.MapObjectPropertiesControl.PropertyValueChanged += MapObjectPropertiesControl_PropertyValueChanged;
        }

        private void MapObjectPropertiesControl_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            //no need to do anything, looks like wpf Propertygrid manage that by itself
            //just need to update the visual position 
            
            this.CanvasImageMap.Children.Clear();
            LoadMapObjects();
            this.mapObjectEdited = true;
            
        }

        private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.mapListComboBox.SelectedIndex != -1)
            {
                AddObjectWindow a = new AddObjectWindow(CurrentMapName);
                a.Show();
            }
        }


        private void buttonViewMap_Click(object sender, RoutedEventArgs e)
        {

            if (this.mapListComboBox.SelectedIndex != -1)
            {
                new MapView3DRender(this.CurrentMapName, this.CurrentMap).Show();
            }

        }

        private void buttonExportEntity_Click(object sender, RoutedEventArgs e)
        {
            if (this.mapListComboBox.SelectedIndex != -1 && this.MapObjectsListView.SelectedIndex != -1)
            {
                POG pog = CurrentMap[this.MapObjectsListView.SelectedIndex];

                string entityName = pog.GetName();

                var tg = new string[] { "AC_ME","CO_ME","GR_ME","HV_ME","LE_ME","ME_ME","SA_ME","SB_ME","SN_ME","SP_ME" };

                if (tg.Contains(entityName))
                {
                    //its a character
                    
                    MAD characterModel = MAD.GetCharacter(entityName, "british" , HIR.GetSkeletonList(), MotionCapture.GetMotionCaptureAnimations() );
                    characterModel.Name = entityName; //replace the wrong name with current entity name
                    GlobalVars.ExportCharacterWithTexture_GLB( characterModel, Mtd.LoadTexturesFromMTD(characterModel.facData,  GlobalVars.gameFolder + "Chars/TEAMLARD.MTD", true) );
                }
                else
                {
                    MAD model = new MAD();
                    model = MAD.GetModelFromMAD(entityName, this.CurrentMapName);

                    model.textures = Mtd.LoadTexturesFromMTD(model.facData, this.CurrentMapName);

                    GlobalVars.ExportModelWithTexture_GLB(model);

                }
            }
        }

        private void buttonMapExport_Click(object sender, RoutedEventArgs e)
        {

            PMG mapTerrain = new PMG(GlobalVars.mapsFolder + this.CurrentMapName);
            PTG mapTiles = new PTG(GlobalVars.mapsFolder + this.CurrentMapName);

            string loc = GlobalVars.exportFolder + "/" + this.CurrentMapName + "/" + this.CurrentMapName;

            File.WriteAllText( loc + ".json", JsonSerializer.Serialize(this.CurrentMap.Select(p => p.POG2JSON()), new JsonSerializerOptions { WriteIndented = true })  );
            mapTiles.CreateIMG(mapTerrain).Save(loc+".png",ImageFormat.Png);

            GlobalVars.ExportTerrain_GLB(mapTerrain,mapTiles,loc+".glb");
        }

        private async void buttonMapExportALL_Click(object sender, RoutedEventArgs e)
        {
            if (this.mapListComboBox.SelectedIndex != -1)
            {
                if (MessageBox.Show("You gonna export all models and textures of this map !,are you sure ? \n\r (Charaters are not exported) ", "/!\\", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    buttonMapExport_Click(null, null);
                    
                    MAD.GetMapEntitiesList(this.CurrentMapName).ForEach(entityName =>
                    {
                        MAD model = MAD.GetModelFromMAD(entityName, this.CurrentMapName);
                        if (model.facData != null)
                        {
                            model.textures = Mtd.LoadTexturesFromMTD(model.facData, this.CurrentMapName);
                            // export
                            GlobalVars.ExportModelWithTexture_GLB(model, GlobalVars.exportFolder +"models/"+ entityName+".glb");
                        }
                    });
                    
                }
                
            }
            else
            {

                //GlobalVars.MadMtdModdingTool();

                /*
                
                //test if the tool worked : load all.mad and all.mtd -> done
                List<MAD> wesh_alors = new List<MAD>();

                MAD.GetModelListFromMad(GlobalVars.exportFolder+"all.MAD").ForEach(entityName =>
                {
                    MAD model = MAD.GetModelFromFullMAD(entityName, GlobalVars.exportFolder + "all.MAD" );
                    if (model.facData != null)
                    {
                        model.textures = Mtd.LoadTexturesFromMTD(model.facData, GlobalVars.exportFolder + "all.MTD",true);
                    }
                    wesh_alors.Add(model);

                });
                */


                /*
                foreach (string fileName in Directory.GetFiles(GlobalVars.mapsFolder,"*.POG"))
                {
                    foreach (POG pog in POG.GetAllMapObject(System.IO.Path.GetFileNameWithoutExtension(fileName)))
                    {
                        if(pog.short0 != 32512 &&  pog.short0 != 16128)
                        {
                            MessageBox.Show("unusual short0 flags : " + pog.short0 + "\n\r Map : " + fileName + "\n\r entity : " + pog.GetName() + " , ID -> " + pog.index);
                        }
                    }
                    
                }*/


                if (MessageBox.Show("STOP ! YOU GONNA EXPORT EVERYTHING !!! , PLEASE CONFIRM ?", "/!\\", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //open a window with export status ...
                    
                    new ExporterWindow().Show();

                }
                

            }

        }



            


    }

    class MapObjectsListViewItem
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Team { get; set; }
    }
}
