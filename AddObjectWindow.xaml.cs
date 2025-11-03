using hogs_gameEditor_wpf.FileFormat;
using hogs_gameManager_wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hogs_gameEditor_wpf
{
    /// <summary>
    /// Interaction logic for AddObjectWindow.xaml
    /// </summary>
    public partial class AddObjectWindow : Window
    {
        string mapName;
        int  newId;
        List<TypesValues> entityTypesList;
        Dictionary<string, byte> weaponList;

        internal class TypesValues
        {
            public string type;
            public string entity;
            public short number;

            public TypesValues(string type, string entity, short number)
            {
                this.type = type;
                this.entity = entity;
                this.number = number;
            }
        }

        public AddObjectWindow(string mapName)
        {
            MainWindow main = (MainWindow)Application.Current.MainWindow;
            this.mapName = mapName;
            this.newId = main.CurrentMap[main.CurrentMap.Count -1 ].index +1 ;
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            entityTypesList = new List<TypesValues>();

            weaponList = new Dictionary<string, byte>();

            entityTypesList.Add(new TypesValues("Grunt", "GR_ME", 0));
            entityTypesList.Add(new TypesValues("Gunner", "HV_ME", 1));
            entityTypesList.Add(new TypesValues("Bombardier", "HV_ME", 2));
            entityTypesList.Add(new TypesValues("Pyrotechnic", "HV_ME", 3));
            entityTypesList.Add(new TypesValues("Sapper", "SA_ME", 5));
            entityTypesList.Add(new TypesValues("Engineer", "SA_ME", 6));
            entityTypesList.Add(new TypesValues("Saboteur", "SA_ME", 7));
            entityTypesList.Add(new TypesValues("Scout", "SN_ME", 8));
            entityTypesList.Add(new TypesValues("Sniper", "SP_ME", 9));
            entityTypesList.Add(new TypesValues("Spy", "SB_ME", 10));
            entityTypesList.Add(new TypesValues("Orderly", "ME_ME", 11));
            entityTypesList.Add(new TypesValues("Medic", "ME_ME", 12));
            entityTypesList.Add(new TypesValues("Surgeon", "ME_ME", 13));
            entityTypesList.Add(new TypesValues("Commando", "CO_ME", 4));
            entityTypesList.Add(new TypesValues("Hero", "LE_ME", 14));
            entityTypesList.Add(new TypesValues("Ace", "AC_ME", 15));
            entityTypesList.Add(new TypesValues("Legend", "AC_ME", 16));

            entityTypesList.Add(new TypesValues("Explosive Drum", "DRUM", 393));
            entityTypesList.Add(new TypesValues("Gas Drum", "DRUM2", 435));
            entityTypesList.Add(new TypesValues("Weapon Crate", "CRATE1", 67));
            entityTypesList.Add(new TypesValues("Health Crate", "CRATE2", 68));
            entityTypesList.Add(new TypesValues("Promotion Point", "PROPOINT", 395));
            entityTypesList.Add(new TypesValues("Tank", "TANK", 148));
            entityTypesList.Add(new TypesValues("Aqua Tank", "AM_TANK", 64));
            entityTypesList.Add(new TypesValues("Carry", "CARRY", 155));
            entityTypesList.Add(new TypesValues("Water Carry", "AMLAUNCH", 136));
            entityTypesList.Add(new TypesValues("Artillery", "BIG_GUN", 93));
            entityTypesList.Add(new TypesValues("PillBox", "PILLBOX", 96));
            entityTypesList.Add(new TypesValues("Small Tent", "TENT_S", 74));
            entityTypesList.Add(new TypesValues("Medical Tent (green)", "M_TENT1", 70));
            entityTypesList.Add(new TypesValues("Medical Tent (tan)", "M_TENT2", 89));
            entityTypesList.Add(new TypesValues("Shelter", "SHELTER", 73));

            this.objectTypeToAddComboBox.ItemsSource = entityTypesList.Select(x => x.type ).ToList();

            weaponList.Add("Cattle Prod", 5);
            weaponList.Add("Pistol", 6);
            weaponList.Add("Rifle Burst", 8);
            weaponList.Add("MG", 9);
            weaponList.Add("Heavy MG", 10);
            weaponList.Add("Sniper Rifle", 11);
            weaponList.Add("Shotgun", 12);
            weaponList.Add("Flamethrower", 13);
            weaponList.Add("Rocket Launcher", 14);
            weaponList.Add("Guided Missile", 15);
            weaponList.Add("Medicine Dart", 16);
            weaponList.Add("Tranquiliser", 17);
            weaponList.Add("Grenade", 18);
            weaponList.Add("Clustergrenade", 19);
            weaponList.Add("HX-Grenade", 20);
            weaponList.Add("Roller Grenade", 21);
            weaponList.Add("Confusion Gas", 22);
            weaponList.Add("Freeze Gas", 23);
            weaponList.Add("Madness Gas", 24);
            weaponList.Add("Poison Gas", 25);
            weaponList.Add("Mortar", 26);
            weaponList.Add("Bazooka", 27);
            weaponList.Add("Airburst", 28);
            weaponList.Add("Super Airburst", 29);
            weaponList.Add("Medicine Ball", 30);
            weaponList.Add("Homing Missile", 31);
            weaponList.Add("Mine", 32);
            weaponList.Add("Anti-P Mine", 33);
            weaponList.Add("TNT", 34);
            weaponList.Add("Jetpack", 51);
            weaponList.Add("Suicide", 52);
            weaponList.Add("Healing Hands", 53);
            weaponList.Add("Self Heal", 54);
            weaponList.Add("Pick Pocket", 55);
            weaponList.Add("Shockwave", 56);
            weaponList.Add("Spec-Ops", 57);
            weaponList.Add("Airstrike", 58);
            weaponList.Add("Fire Rain Airstrike", 59);
            weaponList.Add("HX-TNT", 67);
            weaponList.Add("Hide", 68);
            weaponList.Add("Super Shotgun", 69);
            weaponList.Add("Shrapnel Grenade", 70);
            weaponList.Add("Grenade Launcher", 71);

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
            switch (objectTypeToAddComboBox.SelectedItem) 
            {
                case "Grunt":
                case "Gunner":
                case "Bombardier":
                case "Pyrotechnic":
                case "Sapper":
                case "Engineer":
                case "Saboteur":
                case "Scout":
                case "Sniper":
                case "Spy":
                case "Orderly":
                case "Medic":
                case "Surgeon":
                case "Commando":
                case "Hero":
                case "Ace":
                case "Legend":
                    this.weaponComboBox.IsEnabled = false;
                    this.amountUpDown.IsEnabled = false;
                    this.isPlayerCheckBox.IsEnabled = true;
                    break;

                case "Explosive Drum" :
                case "Gas Drum" :
                case "Promotion Point" :
                case "Tank":
                case "Aqua Tank" :
                case "Carry":
                case "Water Carry" :
                case "Artillery":
                case "PillBox":
                case "Medical Tent (green)":
                case "Medical Tent (tan)":
                case "Shelter":
                    this.weaponComboBox.IsEnabled = false;
                    this.amountUpDown.IsEnabled = false;
                    this.isPlayerCheckBox.IsEnabled = false;
                    break;


                case "Weapon Crate":
                    this.weaponComboBox.IsEnabled = true;
                    this.amountUpDown.IsEnabled = true;
                    this.isPlayerCheckBox.IsEnabled = false;
                    break;

                case "Health Crate":
                    this.weaponComboBox.IsEnabled = false;
                    this.amountUpDown.IsEnabled = true;
                    this.isPlayerCheckBox.IsEnabled = false;
                    break;

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

                POG mo = new POG
                {
                    name = POG.NameToCharArray(this.entityTypesList[this.objectTypeToAddComboBox.SelectedIndex].entity),
                    unused0 = POG.NameToCharArray("NULL"),
                    position = new short[] { top1, 128, left1 },
                    index = (short)this.newId,
                    angles = new short[] { 0, angle, 0 },
                    type = this.entityTypesList[this.objectTypeToAddComboBox.SelectedIndex].number,
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

    }
}
