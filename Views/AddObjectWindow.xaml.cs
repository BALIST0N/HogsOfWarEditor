using hogs_gameEditor_wpf.FileFormat;
using hogs_gameManager_wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;


namespace hogs_gameEditor_wpf
{
    /// <summary>
    /// Interaction logic for AddObjectWindow.xaml
    /// </summary>
    public partial class AddObjectWindow : Window
    {
        string mapName;
        int  newId;
        List<(string, string, short)> charactersTypes;
        Dictionary<string, byte> weaponList;
        Dictionary<string, string[]> json;

        public AddObjectWindow(string mapName)
        {
            MainWindow main = (MainWindow)Application.Current.MainWindow;
            this.mapName = mapName;
            this.newId = main.CurrentMap[main.CurrentMap.Count -1 ].index +1;
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            charactersTypes = new List<(string,string,short)>
            {
                ( "Grunt","GR_ME", 0 ),
                ( "Gunner","HV_ME", 1 ),
                ( "Bombardier","HV_ME", 2 ),
                ( "Pyrotechnic","HV_ME", 3 ),
                ( "Sapper","SA_ME", 5 ),
                ( "Engineer","SA_ME", 6 ),
                ( "Saboteur","SA_ME", 7 ),
                ( "Scout","SN_ME", 8 ),
                ( "Sniper","SP_ME", 9 ),
                ( "Spy","SB_ME", 10 ),
                ( "Orderly","ME_ME", 11 ),
                ( "Medic","ME_ME", 12 ),
                ( "Surgeon","ME_ME", 13 ),
                ( "Commando","CO_ME", 4 ),
                ( "Hero","LE_ME", 14 ),
                ( "Ace","AC_ME", 15 ),
                ( "Legend","AC_ME", 16 ),
            };


            weaponList = new Dictionary<string, byte>();

            //load JSON:
            this.json = JsonSerializer.Deserialize<Dictionary< string,string[]>>(File.ReadAllText("D:/projects devs/hogs_gameManager_wpf/models_category.json"));

            this.objectTypeToAddComboBox.ItemsSource = this.json.Keys;

            //this.objectTypeToAddComboBox.ItemsSource = entityTypesList.Select(x => x.type ).ToList();

            this.weaponComboBox.ItemsSource = weaponList.Select(x => x.Key);
            this.weaponComboBox.IsEnabled = false;
            this.amountUpDown.Value = 1;
            
            this.amountUpDown.IsEnabled = false;
            this.isPlayerCheckBox.IsEnabled = false;

            this.mapImage.Source = new BitmapImage(new Uri("file://" + GlobalVars.mapsViewsFolder + mapName + ".png"));

            Ellipse eli = new Ellipse();
            eli.Width = 8;
            eli.Height = 8;
            eli.Fill = Brushes.Transparent;
            eli.Stroke = Brushes.Red;
            eli.StrokeThickness = 2;

            this.mapCanvas.Children.Add(eli);
            mapCanvas.MouseLeftButtonDown += (s, e) =>
            {
                Point pos = e.GetPosition(mapCanvas);

                double x = pos.X - eli.Width / 2;
                double y = pos.Y - eli.Height / 2;

                // limite à 256x256
                x = Math.Clamp(x, 0, 256 - eli.Width);
                y = Math.Clamp(y, 0, 256 - eli.Height);

                Canvas.SetLeft(eli, x);
                Canvas.SetTop(eli, y);

                label_Copy0.Content = $"{x * 128 - 16384} | {-(y * 128 - 16384)}";
            };

            Canvas.SetLeft(eli, 124);
            Canvas.SetTop(eli, 124);
        }

  

        private void objectTypeToAddComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.EntityListView.Items.Clear();

            if ((string)objectTypeToAddComboBox.SelectedItem == "Characters")
            {
                foreach (var character in  charactersTypes )
                {
                    this.EntityListView.Items.Add(new
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
                    if( GlobalVars.modelsWithMultipleSkins.ContainsKey(ent_name) == true )
                    {
                        foreach(int model_type in GlobalVars.modelsWithMultipleSkins[ent_name])
                        {
                            this.EntityListView.Items.Add(new
                            {
                                name = ent_name,
                                img = new BitmapImage(new Uri("file://" + GlobalVars.exportFolder + "thumbnails/" + ent_name + "_" + model_type + ".png")),
                                type = model_type
                            });
                        }
                    }
                    else 
                    {
                        this.EntityListView.Items.Add(new
                        {
                            name = ent_name,
                            img = new BitmapImage(new Uri("file://" + GlobalVars.exportFolder + "thumbnails/" + ent_name + ".png")),
                            type = GlobalVars.models_uniqueids[ent_name]
                        });
                    }

                }
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.objectTypeToAddComboBox.SelectedIndex != -1) 
            {
                FrameworkElement fe = (FrameworkElement)this.mapCanvas.Children[0];

                short top1 = (short)(Canvas.GetLeft(fe) * 128 - 16384);
                short left1 = (short)-((Canvas.GetTop(fe) * 128 - 16384));

                short angle = (short)GlobalVars.ScaleUpAngles(this.rotationSlider.Value);
                dynamic item = EntityListView.SelectedValue;


                POG mo = new POG
                {
                    name = POG.NameToCharArray( charactersTypes.Find(x => x.Item1 == item.name ).Item2 ),
                    unused0 = POG.NameToCharArray("NULL"),
                    position = new short[] { top1, 128, left1 },
                    index = (short)this.newId,
                    angles = new short[] { 0, angle, 0 },
                    type = item.type,
                    bounds = new short[] { 10, 10, 10 },
                    bounds_type = 0,
                    short0 = this.isPlayerCheckBox.IsChecked == true ? (short)32512 : (short)16128,
                    byte0 = 255,
                    team = POG.PigTeam.Team01,
                    objective = 0,
                    ScriptGroup = 0,
                    ScriptParameters = new byte[19],
                    fallback_position = new short[] { 0, 0, 0 },
                    objectiveFlag = this.isPlayerCheckBox.IsChecked == true ? POG.objectiveFlagEnum.Player : 0,
                    short1 = 0,
                    short2 = 0
                };

                if(this.objectTypeToAddComboBox.SelectedItem == "Weapon Crate")
                {
                    mo.ScriptParameters[0] = this.weaponList[""+this.weaponComboBox.SelectedItem];
                    mo.ScriptParameters[1] = (byte)this.amountUpDown.Value;
                    mo.objective = POG.ScriptType.PICKUP_ITEM;
                }

                if (this.objectTypeToAddComboBox.SelectedItem == "Health Crate")
                {
                    mo.ScriptParameters[0] = 255;
                    mo.ScriptParameters[1] = (byte)this.amountUpDown.Value;
                    mo.objective = POG.ScriptType.PICKUP_ITEM;
                }



                MainWindow main = (MainWindow)Application.Current.MainWindow;
                main.CurrentMap.Add(mo);
                main.MapObjectsListView.Items.Add(newItem: new MapObjectsListViewItem { Name = mo.GetName(), Id = Convert.ToString(mo.index), Team = Convert.ToString(mo.team) });  //this is just adding a row on the listbox
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
                        this.weaponComboBox.IsEnabled = false;
                        this.amountUpDown.IsEnabled = false;
                        this.isPlayerCheckBox.IsEnabled = true;
                        break;

                    case "Crates":
                        if(item.name == "CRATE1" || item.name == "CRATE4")
                        {
                            this.weaponComboBox.IsEnabled = true;
                            this.amountUpDown.IsEnabled = true;
                            this.isPlayerCheckBox.IsEnabled = false;
                        }
                        else
                        {
                            this.weaponComboBox.IsEnabled = false;
                            this.amountUpDown.IsEnabled = true;
                            this.isPlayerCheckBox.IsEnabled = false;
                        }
                        break;

                    default:
                        this.weaponComboBox.IsEnabled = false;
                        this.amountUpDown.IsEnabled = false;
                        this.isPlayerCheckBox.IsEnabled = false;
                        break;

                }

            }

        }
    }
}
