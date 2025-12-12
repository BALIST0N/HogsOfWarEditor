using hogs_gameEditor_wpf.FileFormat;
using hogs_gameManager_wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace hogs_gameEditor_wpf
{

    public partial class AddObjectWindow : Window
    {
        private readonly string mapName;
        private readonly int newId;
        private List<(string, string, short)> charactersTypes;
        private Dictionary<string, byte> weaponList;
        private Dictionary<string, string[]> json;

        public AddObjectWindow(string mapName)
        {
            MainWindow main = (MainWindow)Application.Current.MainWindow;
            this.mapName = mapName;
            newId = main.CurrentMap.Max(x => x.index) + 1;
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            charactersTypes =
            [
                ( "Grunt","GR_ME", 0 ),
                ( "Gunner","HV_ME", 1 ),
                ( "Bombardier","HV_ME", 2 ),
                ( "Pyrotechnic","HV_ME", 3 ), 
                ( "Commando","CO_ME", 4 ),
                ( "Sapper","SA_ME", 5 ),
                ( "Engineer","SA_ME", 6 ),
                ( "Saboteur","SA_ME", 7 ),
                ( "Scout","SN_ME", 8 ),
                ( "Sniper","SP_ME", 9 ),
                ( "Spy","SB_ME", 10 ),
                ( "Orderly","ME_ME", 11 ),
                ( "Medic","ME_ME", 12 ),
                ( "Surgeon","ME_ME", 13 ),
                ( "Hero","LE_ME", 14 ),
                ( "Ace","AC_ME", 15 ),
                ( "Legend","AC_ME", 16 ),
            ];


            weaponList = new Dictionary<string, byte>
            {
                { "Random ?", 72 },
                { "Cattle Prod", 5 },
                { "Pistol", 6 },
                { "Rifle Burst", 8 },
                { "MG", 9 },
                { "Heavy MG", 10 },
                { "Sniper Rifle", 11 },
                { "Shotgun", 12 },
                { "Flamethrower", 13 },
                { "Rocket Launcher", 14 },
                { "Guided Missile", 15 },
                { "Medicine Dart", 16 },
                { "Tranquiliser", 17 },
                { "Grenade", 18 },
                { "Clustergrenade", 19 },
                { "HX-Grenade", 20 },
                { "Roller Grenade", 21 },
                { "Confusion Gas", 22 },
                { "Freeze Gas", 23 },
                { "Madness Gas", 24 },
                { "Poison Gas", 25 },
                { "Mortar", 26 },
                { "Bazooka", 27 },
                { "Airburst", 28 },
                { "Super Airburst", 29 },
                { "Medicine Ball", 30 },
                { "Homing Missile", 31 },
                { "Mine", 32 },
                { "Anti-P Mine", 33 },
                { "TNT", 34 },
                { "Jetpack", 51 },
                { "Suicide", 52 },
                { "Healing Hands", 53 },
                { "Self Heal", 54 },
                { "Pick Pocket", 55 },
                { "Shockwave", 56 },
                { "Spec-Ops", 57 },
                { "Airstrike", 58 },
                { "Fire Rain Airstrike", 59 },
                { "HX-TNT", 67 },
                { "Hide", 68 },
                { "Super Shotgun", 69 },
                { "Shrapnel Grenade", 70 },
                { "Grenade Launcher", 71 }
            };

            //load JSON:
            json = JsonSerializer.Deserialize<Dictionary<string, string[]>>(File.ReadAllText("D:/projects devs/hogs_gameManager_wpf/models_category.json"));

            objectTypeToAddComboBox.ItemsSource = json.Keys;

            //this.objectTypeToAddComboBox.ItemsSource = entityTypesList.Select(x => x.type ).ToList();

            weaponComboBox.ItemsSource = weaponList.Select(x => x.Key);
            weaponComboBox.IsEnabled = false;
            amountUpDown.Value = 1;

            amountUpDown.IsEnabled = false;
            isPlayerCheckBox.IsEnabled = false;

            mapImage.Source = new BitmapImage(new Uri("file://" + GlobalVars.editorRessourcesFolder + mapName + ".png"));

            Ellipse eli = new()
            {
                Width = 8,
                Height = 8,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };

            mapCanvas.Children.Add(eli);
            mapCanvas.MouseLeftButtonDown += (s, e) =>
            {
                Point pos = e.GetPosition(mapCanvas);

                // limite à 256x256
                double x = Math.Clamp(pos.X, 0, 256);
                double y = Math.Clamp(pos.Y, 0, 256);

                Canvas.SetLeft(eli, x-4);
                Canvas.SetTop(eli, y-4);

                label_Copy0.Content = $"{(x * 128) - 16384} | {-((y * 128) - 16384)}";
            };

            Canvas.SetLeft(eli, 124);
            Canvas.SetTop(eli, 124);
        }


        private void objectTypeToAddComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EntityListView.Items.Clear();

            if ((string)objectTypeToAddComboBox.SelectedItem == "Characters")
            {
                foreach ((string, string, short) character in charactersTypes)
                {
                    EntityListView.Items.Add(new
                    {
                        name = character.Item1,
                        img = new BitmapImage(),
                        type = character.Item3,
                    });
                }
            }
            else
            {
                foreach (string ent_name in json[(string)objectTypeToAddComboBox.SelectedItem])
                {
                    if (GlobalVars.modelsWithMultipleSkins.ContainsKey(ent_name) == true)
                    {
                        foreach (int model_type in GlobalVars.modelsWithMultipleSkins[ent_name])
                        {
                            EntityListView.Items.Add(new
                            {
                                name = ent_name,
                                img = new BitmapImage(new Uri("file://" + GlobalVars.editorRessourcesFolder  + ent_name + "_" + model_type + ".png")),
                                type = model_type
                            });
                        }
                    }
                    else
                    {

                        EntityListView.Items.Add(new
                        {
                            name = ent_name,
                            img = new BitmapImage(new Uri("file://" + GlobalVars.editorRessourcesFolder + ent_name + ".png")),
                            type = GlobalVars.models_uniqueids.TryGetValue(ent_name, out short id) ? (short)id : (short)0
                        });
                    }

                }
            }
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (objectTypeToAddComboBox.SelectedIndex != -1 && EntityListView.SelectedValue != null)
            {
                string[] numbers = label_Copy0.Content.ToString().Split('|');

                short angle = (short)GlobalVars.ScaleUpAngles(rotationSlider.Value);
                dynamic item = EntityListView.SelectedValue;

                char[] name;
                if (objectTypeToAddComboBox.SelectedIndex == 0 )
                {
                    name = POG.NameToCharArray(charactersTypes.Find(x => x.Item1 == item.name).Item2);
                }
                else
                {
                    name = POG.NameToCharArray(item.name);
                }

                POGL pogl = GlobalVars.wesh.Find(x => x.name == item.name) ;
                if (pogl == null)
                {
                    pogl = new POGL()
                    {
                        bounds = new short[] { 5, 5, 5 },
                        bounds_type = 0
                    };
                }
                POG mo = new POG();

                mo.name = name;
                mo.unused0 = POG.NameToCharArray("NULL");
                mo.position = new short[] { short.Parse(numbers[0]), 1500, short.Parse(numbers[1]) };
                mo.index = (short)newId;
                mo.angles = new short[] { 0, angle, 0 };
                mo.type = (short)item.type;
                mo.bounds = pogl.bounds;
                mo.bounds_type = pogl.bounds_type;
                mo.short0 = isPlayerCheckBox.IsChecked == true ? (short)32512 : (short)16128;
                mo.byte0 = 255;
                mo.team = POG.PigTeam.british;
                mo.objective = 0;
                mo.ScriptGroup = 0;
                mo.ScriptParameters = new byte[19];
                mo.fallback_position = new short[] { 0, 0, 0 };
                mo.objectiveFlag = isPlayerCheckBox.IsChecked == true ? POG.objectiveFlagEnum.Player : 0;
                mo.short1 = 0;
                mo.short2 = 0;
                

                if (item.name == "CRATE1" || item.name == "CRATE4" )
                {
                    if(this.weaponComboBox.SelectedIndex == -1)
                    {
                        return;
                    }
                    mo.byte0 = 0;
                    mo.ScriptParameters[0] = weaponList["" + weaponComboBox.SelectedItem];
                    mo.ScriptParameters[1] = (byte)amountUpDown.Value;
                    mo.objective = POG.ScriptType.PICKUP_ITEM;
                }

                if (item.name == "CRATE2")
                {
                    mo.byte0 = 0;
                    mo.ScriptParameters[0] = 255;
                    mo.ScriptParameters[1] = (byte)amountUpDown.Value;
                    mo.objective = POG.ScriptType.PICKUP_ITEM;
                }



                MainWindow main = (MainWindow)Application.Current.MainWindow;
                if(main.viewMode3D == true)
                {
                    string m_path = GlobalVars.modelsWithMultipleSkins.ContainsKey(mo.GetName()) == true
                                    ? GlobalVars.exportFolder + $"models/{mo.GetName()}_{mo.type}.glb"
                                    : GlobalVars.exportFolder + $"models/{mo.GetName()}.glb";

                    string rx = GlobalVars.ScaleDownAngles(mo.angles[0]).ToString(CultureInfo.InvariantCulture);
                    string ry = GlobalVars.ScaleDownAngles(mo.angles[1]).ToString(CultureInfo.InvariantCulture);
                    string rz = GlobalVars.ScaleDownAngles(mo.angles[2]).ToString(CultureInfo.InvariantCulture);

                    await main.webView.ExecuteScriptAsync($@"loadModel('{m_path}', {mo.index}, {mo.position[0]}, {mo.position[1]}, {mo.position[2]},{rx},{ry},{rz});");
                }

                main.CurrentMap.Add(mo);
                var newItem = new MapObjectsListViewItem { Name = mo.GetName(), Id = Convert.ToString(mo.index), Team = "" };
                if (this.objectTypeToAddComboBox.SelectedIndex == 0)
                {
                    newItem.Team = Convert.ToString(mo.team);
                }

                main.MapObjectsListView.Items.Add(newItem); 
                main.MapObjectsListView.ScrollIntoView(newItem);
                main.LoadMapObjects();
                main.mapObjectEdited = true;
                this.Close();
            }

        }

        private void EntityListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic item = EntityListView.SelectedValue;

            if (item != null)
            {
                switch (objectTypeToAddComboBox.SelectedItem)
                {
                    case "Characters":
                        weaponComboBox.IsEnabled = false;
                        amountUpDown.IsEnabled = false;
                        isPlayerCheckBox.IsEnabled = true;
                        break;

                    case "Crates":
                        if (item.name is "CRATE1" or "CRATE4")
                        {
                            weaponComboBox.IsEnabled = true;
                            amountUpDown.IsEnabled = true;
                            isPlayerCheckBox.IsEnabled = false;
                        }
                        else
                        {
                            weaponComboBox.IsEnabled = false;
                            amountUpDown.IsEnabled = true;
                            isPlayerCheckBox.IsEnabled = false;
                        }
                        break;

                    default:
                        weaponComboBox.IsEnabled = false;
                        amountUpDown.IsEnabled = false;
                        isPlayerCheckBox.IsEnabled = false;
                        break;

                }

            }

        }
    }
}
