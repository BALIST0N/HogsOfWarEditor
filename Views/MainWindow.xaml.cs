using hogs_gameEditor_wpf;
using hogs_gameEditor_wpf.FileFormat;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.PropertyGrid;


namespace hogs_gameManager_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, string> MapList;
        public List<POG> CurrentMap;
        private string CurrentMapName;
        public bool mapObjectEdited = false;
        public bool viewMode3D = false;

        Storyboard pulseStoryboard;

        public MainWindow() //the entry point of the application
        {
            InitializeComponent();
             
            #if !DEBUG
                ExtractRessources();
            #endif


            #region MapList Fillings
            MapList = new Dictionary<string, string>
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

            MapList = MapList.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            #endregion

        }

        private void ExtractRessources()
        {
            
            string fldr = System.IO.Path.GetDirectoryName(Environment.ProcessPath)!;
            //string fldr = "E:/Games/IGG-HogsofWar/";

            if (File.Exists( System.IO.Path.Combine(fldr,"warhogs.exe") ) == true && File.Exists(System.IO.Path.Combine(fldr, "FEProg.exe")) == true )
            {
                Directory.CreateDirectory(System.IO.Path.Combine(fldr, "EXPORT") );
                if( Directory.Exists(System.IO.Path.Combine(fldr, "EXPORT/editorRessources") ) == true )
                {
                    return;
                }

                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show("for using the 3D View, you must export models \n\r simply use that \"Export ALL button\" : ) ");
                });

                using ZipArchive archive = new ZipArchive( Assembly.GetExecutingAssembly().GetManifestResourceStream("hogs_gameEditor_wpf.editorRessources.zip") );
                archive.ExtractToDirectory( System.IO.Path.Combine(fldr, "EXPORT/editorRessources/") );
                
            }
            else
            {
                MessageBox.Show("are you sure you are in the warhogs game folder ? ( i can't find warhogs.exe ) ");
                Environment.Exit(1);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.pulseStoryboard = new();

            ColorAnimation animation = new()
            {
                From = Color.FromRgb(32, 32, 32),
                To = Colors.DarkOliveGreen,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard.SetTarget(animation, mapListComboBox);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(ComboBox.Background).(SolidColorBrush.Color)"));

            pulseStoryboard.Children.Add(animation);

            pulseStoryboard.Begin();
           
            this.button.Content = "";
            this.buttonViewMap.Content = "";
            mapListComboBox.ItemsSource = MapList.Keys;
            MapObjectsListView.KeyUp += MapObjectsListView_KeyUp;

        }

        public void MapListComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.button.Content = "Add New item to map";
            this.buttonViewMap.Content = "Switch to 3D View";
            this.pulseStoryboard.Stop();

            if (mapListComboBox.SelectedIndex != -1)
            {
                if (e != null && e.RemovedItems.Count > 0 && mapObjectEdited)
                {
                    if (MessageBox.Show("would You like to save your data on this map ?", "Attention", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                    {
                        POG.ExportMapPOG(CurrentMap, CurrentMapName);
                    }
                }

                //clear to avoid Exceptions
                MapObjectPropertiesControl.SelectedObject = null;
                MapObjectsListView.Items.Clear();
                CanvasImageMap.Children.Clear();
                mapObjectEdited = false;
                CurrentMapName = MapList.ElementAt(mapListComboBox.SelectedIndex).Value;
                buttonExportEntity.Content = "";
                buttonMapExport.Content = "Export " + CurrentMapName;

                //Read the File
                CurrentMap = POG.GetAllMapObject(CurrentMapName);
                string[] charsNames = JsonSerializer.Deserialize<Dictionary<string, string[]>>(File.ReadAllText(GlobalVars.editorRessourcesFolder +"models_category.json"))["Characters"];
                
                foreach (POG mo in CurrentMap)
                {
                    if( charsNames.Contains(mo.GetName() ) == true )
                    {
                        MapObjectsListView.Items.Add(newItem: new MapObjectsListViewItem { Name = GlobalVars.Name_Converter(mo.name), Id = Convert.ToString(mo.index), Team = Convert.ToString(mo.team) });  //this is just adding a row on the listbox
                    }
                    else
                    {
                        MapObjectsListView.Items.Add(newItem: new MapObjectsListViewItem { Name = GlobalVars.Name_Converter(mo.name), Id = Convert.ToString(mo.index), Team = "" });  

                    }
                }

                MapImageControl.Source = new BitmapImage(new Uri("file://" + GlobalVars.editorRessourcesFolder + CurrentMapName + ".png")); //loading the center map

                //generate buttons with icons in the minimap
                LoadMapObjects();
            }
        }

        private async void MapObjectsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)    //click on a different map object
        {
            if (MapObjectsListView.SelectedItem is MapObjectsListViewItem selectedItem) //what the hell is taht magic ? 
            {
                int a = MapObjectsListView.SelectedIndex;

                MapObjectPropertiesControl.SelectedObject = CurrentMap[a];
                MapObjectPropertiesControl.SelectedObjectName = CurrentMap[a].GetName();
                MapObjectPropertiesControl.SelectedObjectTypeName = "Object n°" + selectedItem.Id;
                MapObjectPropertiesControl.Update();
                MapObjectPropertiesControl.ExpandAllProperties();
                buttonExportEntity.Content = "Export " + MapObjectPropertiesControl.SelectedObjectName;

                if( this.viewMode3D == true)
                {
                    // Envoyer à Babylon.js
                    await webView.CoreWebView2.ExecuteScriptAsync($@"highlightById({selectedItem.Id});");
                }

            }
        }

        private async void MapObjectsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && MapObjectsListView.SelectedIndex != -1)
            {
                MapObjectsListViewItem molv = (MapObjectsListViewItem)MapObjectsListView.SelectedItem;
                MessageBoxResult res = MessageBox.Show("are you sure you want to delete Object n°" + molv.Id + " (" + molv.Name + ") ", "A T T E N T I O N ", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    CurrentMap.Remove(CurrentMap.Find(x => x.index == Convert.ToInt16(molv.Id)));
                    MapObjectsListView.Items.Remove(molv);

                    CanvasImageMap.Children.Clear();
                    LoadMapObjects();

                    if(this.viewMode3D == true)
                    {
                        await this.webView.ExecuteScriptAsync($@"DeleteModel({molv.Id});");
                    }

                    mapObjectEdited = true;
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
            if (GlobalVars.models_category["Characters"].Contains(mo.GetName()) == true)
            {
                GenerateAndSpawnCharacterMapButton(mo);
                return;
            }

            switch (mo.GetName())    //check the mapobject name and draw it differently accoring to his name
            {
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

            Rectangle b = new()
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
            System.Windows.Shapes.Path X = new()
            {
                StrokeThickness = 1,
                Stroke = Brushes.Black,
                Name = "n" + CurrentMap.IndexOf(mo).ToString(),
                Data = Geometry.Parse("M 2.4,0 L 6,3.6 L 9.6,0 L 12,2.4 L 8.4,6 L 12,9.6 L 9.6,12 L 6,8.4 L 2.4,12 L 0,9.6 L 3.6,6 L 0,2.4 Z"),
                Width = 12,
                Height = 12,
                VerticalAlignment = VerticalAlignment.Center,
            };
            switch (mo.team)
            {
                case POG.PigTeam.british:
                    X.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                    break;

                case POG.PigTeam.FRENCH:
                    X.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                    break;

                case POG.PigTeam.AMERICAN:
                    X.Fill = Brushes.Turquoise;
                    break;

                case POG.PigTeam.RUSSIAN:
                    X.Fill = Brushes.Red;
                    break;

                case POG.PigTeam.JAPANESE:
                    X.Fill = Brushes.Yellow;
                    break;

                case POG.PigTeam.GERMAN:
                    X.Fill = Brushes.White;
                    break;

                case POG.PigTeam.TEAMLARD:
                    X.Fill = Brushes.Purple;
                    break;
                case POG.PigTeam.unused:
                    X.Fill = Brushes.Black;
                    break;
            }

            X.MouseDown += B_Click;

            CanvasImageMap.Children.Add(X);
            Canvas.SetLeft(X, (mo.position[0] / 64) + 251);
            Canvas.SetTop(X, -(mo.position[2] / 64) + 251);

        }
        private void GenerateObjectMapButton(POG mo, Brush backColor)
        {
            Rectangle b = GenerateObjectMapButton(mo);
            b.Fill = backColor;
            SpawnObjectMapRectangle(b, mo);

        }
        private void GenerateObjectMapButton(POG mo, Brush backColor, Brush bordercolor)
        {

            Rectangle b = GenerateObjectMapButton(mo);
            b.Fill = backColor;
            b.Stroke = bordercolor;
            SpawnObjectMapRectangle(b, mo);
        }

        private void SpawnObjectMapRectangle(Rectangle R, POG mo)
        {
            double x = mo.position[0] / 64;
            double y = -mo.position[2] / 64;

            CanvasImageMap.Children.Add(R);
            Canvas.SetLeft(R, x + 251);
            Canvas.SetTop(R, y + 251);
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            Shape b = (Shape)sender;
            MapObjectPropertiesControl.PropertyValueChanged -= MapObjectPropertiesControl_PropertyValueChanged;
            MapObjectsListView.SelectedIndex = Convert.ToInt32(b.Name.Replace("n", string.Empty));
            MapObjectsListView.ScrollIntoView(MapObjectsListView.Items[MapObjectsListView.SelectedIndex]);
            MapObjectPropertiesControl.PropertyValueChanged += MapObjectPropertiesControl_PropertyValueChanged;
        }

        private async void MapObjectPropertiesControl_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            //no need to do anything, looks like wpf Propertygrid manage that by itself
            //just need to update the visual position 
            if( this.viewMode3D  == true )
            {
                string propertyName = e.OriginalSource is Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem item ? item.PropertyName : null;
                POG pgm = e.OriginalSource is Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem ? (POG)propertyItem.Instance : null;
                if (propertyName != null && pgm != null)
                {
                    if(propertyName == "PositionX" || propertyName == "PositionY" | propertyName == "PositionZ")
                    {
                        await webView.CoreWebView2.ExecuteScriptAsync($@"positionModel( {pgm.index},{pgm.position[0]},{pgm.position[1]},{pgm.position[2]} );");
                        Debug.WriteLine(" Id : "+ pgm.index+ " / pos : "+ pgm.position[0] + " - " + pgm.position[1] + " - " + pgm.position[2]);
                    }

                    if (propertyName == "AngleX" || propertyName == "AngleY" | propertyName == "AngleZ")
                    {
                        string rx = GlobalVars.ScaleDownAngles(pgm.angles[0]).ToString(CultureInfo.InvariantCulture);
                        string ry = GlobalVars.ScaleDownAngles(pgm.angles[1]).ToString(CultureInfo.InvariantCulture);
                        string rz = GlobalVars.ScaleDownAngles(pgm.angles[2]).ToString(CultureInfo.InvariantCulture);
                        await webView.CoreWebView2.ExecuteScriptAsync($@"RotateModel( {pgm.index},{rx},{ry},{rz} );");
                    }

                }

            }


            CanvasImageMap.Children.Clear();
            LoadMapObjects();
            mapObjectEdited = true;

        }

        private void AddNewObjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (mapListComboBox.SelectedIndex != -1)
            {
                AddObjectWindow a = new(CurrentMapName)
                {
                    Left = this.Left + 150,
                    Top = this.Top + 40
                };
                a.Show();
            }
        }


        private async void buttonViewMap_Click(object sender, RoutedEventArgs e)
        {
            if (mapListComboBox.SelectedIndex != -1)
            {
                //disabel the button for approx 1 sec

                if (viewMode3D == false)
                {
                    // --- UI ---
                    Width += 130;
                    mainGrid.Width = Width;

                    buttonViewMap.Content = "Switch to 2D View";
                    viewMode3D = true;

                    mapListComboBox.IsEnabled = false;
                    MapImageControl.Visibility = Visibility.Collapsed;
                    CanvasImageMap.Visibility = Visibility.Collapsed;

                    // --- CRÉER WEBVIEW PROPRE ---
                    webView = new Microsoft.Web.WebView2.Wpf.WebView2()
                    {
                        Height = 480,
                        Width = 640,
                        Margin = new Thickness(240, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                    };

                    mainGrid.Children.Add(webView);

                    // ENVIRONNEMENT
                    CoreWebView2Environment env = await CoreWebView2Environment.CreateAsync(null, null, new CoreWebView2EnvironmentOptions("--allow-file-access-from-files"));

                    await webView.EnsureCoreWebView2Async(env);

                    // --- ATTACHER L'ÉVÈNEMENT AVANT LA NAVIGATION ---
                    webView.CoreWebView2.NavigationCompleted += async (s, e2) =>
                    {
                        string m_path = GlobalVars.exportFolder + "maps/" + CurrentMapName + ".glb";
                        await webView.CoreWebView2.ExecuteScriptAsync($@"loadModel('{m_path}', 999, 0, 0, 0);");

                        int max = CurrentMap.Max(x => x.index);
                        foreach (POG p in CurrentMap)
                        {
                            if (GlobalVars.models_category["Characters"].Contains(p.GetName()) == false)
                            {
                                m_path = GlobalVars.modelsWithMultipleSkins.ContainsKey(p.GetName()) == true
                                    ? GlobalVars.exportFolder + $"models/{p.GetName()}_{p.type}.glb"
                                    : GlobalVars.exportFolder + $"models/{p.GetName()}.glb";

                                try
                                {
                                    string rx = GlobalVars.ScaleDownAngles(p.angles[0]).ToString(CultureInfo.InvariantCulture);
                                    string ry = GlobalVars.ScaleDownAngles(p.angles[1]).ToString(CultureInfo.InvariantCulture);
                                    string rz = GlobalVars.ScaleDownAngles(p.angles[2]).ToString(CultureInfo.InvariantCulture);

                                    if (webView != null) 
                                    {
                                        await webView.CoreWebView2.ExecuteScriptAsync($@"loadModel('{m_path}', {p.index}, {p.position[0]}, {p.position[1]}, {p.position[2]},{rx},{ry},{rz});");
                                    }

                                }
                                catch (Exception) { continue; }

                            }
                            else
                            {
                                try
                                {

                                    string rx = GlobalVars.ScaleDownAngles(p.angles[0]).ToString(CultureInfo.InvariantCulture);
                                    string ry = GlobalVars.ScaleDownAngles(p.angles[1]).ToString(CultureInfo.InvariantCulture);
                                    string rz = GlobalVars.ScaleDownAngles(p.angles[2]).ToString(CultureInfo.InvariantCulture);

                                    await webView.CoreWebView2.ExecuteScriptAsync($@"loadModel('{GlobalVars.editorRessourcesFolder}temp_dummy_team_{(short)p.team}.glb', {p.index}, {p.position[0]}, {p.position[1]}, {p.position[2]},{rx},{ry},{rz});");

                                }
                                catch (Exception) { continue; }
                            }


                        }

                        // Sky
                        m_path = GlobalVars.exportFolder + "skydomes/skydomeu_SUNNY.glb";
                        try
                        {
                            await webView.CoreWebView2.ExecuteScriptAsync($@"loadModel('{m_path}', 998, 0, 0, 0);");
                        }
                        catch (Exception){ }

                    };

                    webView.CoreWebView2.WebMessageReceived += (s, e) =>
                    {
                        handle3DWebViewMessages(e.TryGetWebMessageAsString());
                    };

                    webView.Source = new Uri("file:///"+GlobalVars.editorRessourcesFolder+"scene.html");



                }
                else
                {
                    // --- Retour en 2D ---

                    Width -= 130;
                    mainGrid.Width = Width;

                    buttonViewMap.Content = "Switch to 3D View";
                    viewMode3D = false;

                    MapImageControl.Visibility = Visibility.Visible;
                    CanvasImageMap.Visibility = Visibility.Visible;
                    mapListComboBox.IsEnabled = true;

                    // Détruire proprement
                    if (webView != null)
                    {
                        try { webView.CoreWebView2.Navigate("about:blank"); } catch { }
                        await Task.Delay(100);
                        webView.Dispose();
                        webView = null;
                    }
                }
            }

        }

        private void handle3DWebViewMessages(string message)
        {
            if (message.StartsWith("SELECT|"))
            {
                string id = message.Substring("SELECT|".Length);

                // On trouve l'objet qui correspond
                MapObjectsListViewItem selectedObject = MapObjectsListView.Items.OfType<MapObjectsListViewItem>().FirstOrDefault(x => x.Id == id);

                if (selectedObject != null)
                {
                    MapObjectsListView.SelectedItem = selectedObject;
                    MapObjectsListView.ScrollIntoView(selectedObject);
                }
                return;
            }

            if(message == "LOADFINISHED")
            {
                if (this.MapObjectsListView.SelectedIndex != -1) //if all models has been loaded and something is selected
                {
                    MapObjectsListViewItem tg = MapObjectsListView.SelectedItem as MapObjectsListViewItem;
                    webView.CoreWebView2.ExecuteScriptAsync($@"highlightById({tg.Id});");
                }
            }
        }

        private void buttonExportEntity_Click(object sender, RoutedEventArgs e)
        {
            if (mapListComboBox.SelectedIndex != -1 && MapObjectsListView.SelectedIndex != -1)
            {
                POG pog = CurrentMap[MapObjectsListView.SelectedIndex];

                string entityName = pog.GetName();

                if (GlobalVars.models_category["Characters"].Contains(entityName))
                {
                    //its a character

                    MAD characterModel = MAD.GetCharacter(entityName, pog.team);
                    characterModel.Name = entityName; //replace the wrong name with current entity name
                    GlobalVars.ExportCharacterWithTexture_GLB(characterModel,true);
                }
                else
                {
                    new MAD();
                    MAD model = MAD.GetModelFromMAD(entityName, CurrentMapName);

                    model.textures = Mtd.LoadTexturesFromMTD(model.facData, CurrentMapName);

                    GlobalVars.ExportModelWithTexture_GLB(model);

                }
            }
        }

        private void buttonMapExport_Click(object sender, RoutedEventArgs e)
        {
            PMG mapTerrain = new(GlobalVars.mapsFolder + CurrentMapName);
            PTG mapTiles = new(GlobalVars.mapsFolder + CurrentMapName);
            Directory.CreateDirectory(GlobalVars.exportFolder + "maps");

            string loc = GlobalVars.exportFolder + "maps/" + CurrentMapName;

            File.WriteAllText(loc + ".json", JsonSerializer.Serialize(CurrentMap.Select(p => p.POG2JSON()), new JsonSerializerOptions { WriteIndented = true }));
            //mapTiles.CreateIMG(mapTerrain).Save(loc + ".png", ImageFormat.Png);

            GlobalVars.ExportTerrain_GLB(mapTerrain, mapTiles, loc + ".glb");
        }

        private async void buttonMapExportALL_Click(object sender, RoutedEventArgs e)
        {
            if (mapListComboBox.SelectedIndex != -1)
            {
                if (MessageBox.Show("You gonna export all models and textures of this map !,are you sure ? ", "/!\\", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    buttonMapExport_Click(null, null);
                    Directory.CreateDirectory(GlobalVars.exportFolder + "models");

                    MAD.GetMapEntitiesList(CurrentMapName).ForEach(entityName =>
                    {
                        MAD model = MAD.GetModelFromMAD(entityName, CurrentMapName);
                        if (model.facData != null)
                        {
                            model.textures = Mtd.LoadTexturesFromMTD(model.facData, CurrentMapName);
                            // export
                            GlobalVars.ExportModelWithTexture_GLB(model, GlobalVars.exportFolder + "models/" + entityName + ".glb");
                        }
                    });

                }

            }
            else
            {


                ExporterWindow nw = new ExporterWindow()
                {
                    Top = this.Top + 70,
                    Left = this.Left + 300,
                };
                nw.Show();

                /*
                List<POGL> a = new();

                foreach (string filename in Directory.GetFiles(GlobalVars.mapsFolder, "*.MAD"))
                {
                    foreach (POG item in POG.GetAllMapObject( System.IO.Path.GetFileNameWithoutExtension(filename)))
                    {
                        if( GlobalVars.models_category["Characters"].Contains(item.GetName()) == true) { continue; }

                        POGL pogl = a.Find(x => x.name == item.GetName());

                        if ( pogl == null)
                        {
                            a.Add( new POGL()
                            {
                                name = item.GetName(),
                                type = new List<short>(){item.type},
                                bounds = item.bounds,
                                bounds_type = item.bounds_type,
                            });
                        }
                        else
                        {
                            if( pogl.type.Contains(item.type) == false )
                            {
                                a[a.IndexOf(pogl)].type.Add(item.type);
                            }
                            
                        }
                        
                    }
                }

                File.WriteAllText("D:\\projects devs\\hogs_gameManager_wpf\\test.json", JsonSerializer.Serialize(a.OrderBy(x => x.type.FirstOrDefault() ).ToList(), new JsonSerializerOptions { WriteIndented = true }) );
                */



                /*
                
                MAD dummy = MAD.GetModelFromFullMAD("DUMMY", GlobalVars.mapsFolder + "CAMP.MAD");

                GlobalVars.ExportModelWithOutTexture_GLB(dummy,Vector4.Normalize(new Vector4(9,155,46,255) ),   GlobalVars.exportFolder + "temp_dummy_team_1.glb");
                GlobalVars.ExportModelWithOutTexture_GLB(dummy,Vector4.Normalize(new Vector4(0,0,196,255) ),    GlobalVars.exportFolder + "temp_dummy_team_2.glb");
                GlobalVars.ExportModelWithOutTexture_GLB(dummy,Vector4.Normalize(new Vector4(51,166,217,255) ), GlobalVars.exportFolder + "temp_dummy_team_4.glb");
                GlobalVars.ExportModelWithOutTexture_GLB(dummy,Vector4.Normalize(new Vector4(202,0,0,255) ),    GlobalVars.exportFolder + "temp_dummy_team_8.glb");
                GlobalVars.ExportModelWithOutTexture_GLB(dummy,Vector4.Normalize(new Vector4(202,204,4,255) ),  GlobalVars.exportFolder + "temp_dummy_team_16.glb");
                GlobalVars.ExportModelWithOutTexture_GLB(dummy,Vector4.Normalize(new Vector4(90,80,70,255) ),   GlobalVars.exportFolder + "temp_dummy_team_32.glb");
                GlobalVars.ExportModelWithOutTexture_GLB(dummy,Vector4.Normalize(new Vector4(133,0,200,255) ),  GlobalVars.exportFolder + "temp_dummy_team_64.glb");
                GlobalVars.ExportModelWithOutTexture_GLB(dummy,Vector4.Normalize(new Vector4(133,0,200,255) ),  GlobalVars.exportFolder + "temp_dummy_team_128.glb");
                
                */



                /*
                List<MAD> wesh_alors = new List<MAD>();
                MAD bigbar = null;
                foreach(string filename in Directory.GetFiles(GlobalVars.mapsFolder,"*.MAD") )
                {
                    MAD.GetModelListFromMad(filename).ForEach(entityName =>
                    {
                        if (entityName == "SWILL2")
                        {
                            MAD model = MAD.GetModelFromFullMAD(entityName, filename);
                            if (model.facData != null)
                            {
                                model.textures = Mtd.LoadTexturesFromMTD(model.facData, filename.Remove(filename.Length-4) +  ".MTD", true);
                            }
                            wesh_alors.Add(model);

                        }

                        if (entityName == "SW2ARM")
                        {
                            bigbar = MAD.GetModelFromFullMAD(entityName, filename);
                            if (bigbar.facData != null)
                            {
                                bigbar.textures = Mtd.LoadTexturesFromMTD(bigbar.facData, filename.Remove(filename.Length - 4) + ".MTD", true);
                            }

                        }
                    });
                }
                
                GlobalVars.ExportCombinedModels_GLB(wesh_alors[0], bigbar, new Vector3(0, 550, 0), "E:/Games/IGG-HogsofWar/devtools/EXPORT/swill_with_arms.glb");
                */




                //test if the tool worked : load all.mad and all.mtd -> done
                /*
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
                var list = new Dictionary<string, List<short>>();

                string[] tgfdp = new string[] { "AC_ME", "CO_ME", "GR_ME", "HV_ME", "LE_ME", "ME_ME", "SA_ME", "SB_ME", "SN_ME", "SP_ME" };

                foreach (string fileName in Directory.GetFiles(GlobalVars.mapsFolder, "*.POG"))
                {
                    foreach (POG pog in POG.GetAllMapObject(System.IO.Path.GetFileNameWithoutExtension(fileName)))
                    {
                        if (tgfdp.Contains( pog.GetName() ) == false )
                        { 
                            if ( list.ContainsKey( pog.GetName() ) == false )
                            {
                                list.Add(pog.GetName(), new List<short> { pog.type });
                            }
                            else if (list[pog.GetName()].Exists(x => x == pog.type) == false )
                            {
                                list[pog.GetName()].Add(pog.type);
                            }
                        
                        }

                    }
                }

                File.WriteAllText("E:/Games/IGG-HogsofWar/devtools/EXPORT/models_multipleIds.json", JsonSerializer.Serialize( list.Where(x => x.Value.Count > 1).ToDictionary() ) );
                File.WriteAllText("E:/Games/IGG-HogsofWar/devtools/EXPORT/models_uniqueIds.json", JsonSerializer.Serialize(list.Where(x => x.Value.Count < 2).ToDictionary(x => x.Key, x => x.Value.FirstOrDefault() )) );
                */


            }

        }

    }



    internal class MapObjectsListViewItem
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Team { get; set; }
    }
}
