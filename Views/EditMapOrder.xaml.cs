using hogs_gameManager_wpf;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace hogs_gameEditor_wpf.Views
{
    public partial class EditMapOrder : Window
    {
        private Dictionary<string, string> mapList;

        public EditMapOrder(double posX, double posY, Dictionary<string, string> mapList)
        {
            InitializeComponent();

            this.mapList = new();
            for (int i = 1; i <= 25; i++)
            {
                var a = mapList.ElementAt(i);
                this.mapList.Add(a.Key, a.Value);
            }

            // Positionner la fenêtre ici
            this.Left = posX + 50;
            this.Top = posY + 50;

            foreach (var item in this.mapList)
            {
                mapOrderListView.Items.Add( newItem: new MapItem{ Name = item.Key, Map = item.Value } );
            }

            this.mapNameReplacerComboBox.ItemsSource = new List<string>()
            {
                " 00: Boot Camp (CAMP)",
                " 01: The War Foundation (ESTU)",
                " 02: Routine Patrol (ROAD)",
                " 03: Trench Warfare (TRENCH)",
                " 04: Morning Glory (DEVI)",
                " 05: Island Invasion (RUMBLE)",
                " 06: Under Siege (ZULUS)",
                " 07: Communication Breakdown (TWIN)",
                " 08: The Spying Game (SNIPER)",
                " 09: The Village People (MASHED)",
                " 10: Bangers 'N' Mash (GUNS) ",
                " 12: Just Deserts (OASIS) ",
                " 13: Glacier Guns (FJORDS) ",
                " 14: Battle Stations (EYRIE) ",
                " 15 Beta (EASY) ",
                " 15: Fortified Swine (BAY) ",
                " 16: Over The Top (MEDIX) ",
                " 17: Geneva Convention (BRIDGE) ",
                " 18: I Spy (DESVAL) ",
                " 19: Chemical Compound (SNAKE) ",
                " 20: Achilles Heal (EMPLACE) ",
                " 21: High And Dry (SUPLINE) ",
                " 22: Assassination (KEEP) ",
                " 23: Hero Warship (TESTER) ",
                " 24: Hamburger Hill (FOOT) ",
                " 25: Well, Well, Well! (FINAL) ",
                " Bridge The Gap (LECPROD) ",
                " Chill Hill (ICE) ",
                " Cratermass (CMASS) ",
                " Dam Busters (BHILL) ",
                " Death Bowl (DBOWL) ",
                " Death Valley (DVAL2) ",
                " Death Valley Beta (DVAL) ",
                " Doomed (ARTGUN) ",
                " Duvet Fun (PLAY2) ",
                " Friendly Fire (BOOM) ",
                " Frost Fight (MLAKE) ",
                " Graveyard Shift (CREEPY2) ",
                " Hedge Maze (MAZE) ",
                " Ice Flow (ICEFLOW) ",
                " Ice Flow Beta (DEMO2) ",
                " Island Hopper (ISLAND) ",
                " Moon Buttes (BUTE) ",
                " One Way System (ONEWAY) ",
                " Pigin' Hell (HELL2) ",
                " Pigs in Space (LUNAR1) ",
                " Play Pen (PLAY1) ",
                " Ridge Back (RIDGE) ",
                " Skulduggery (HELL3) ",
                " Square Off (SEPIA1) ",
                " The Lake (LAKE) ",
                " You Hillock (ARCHI) "
            };

            this.mapNameReplacerComboBox.IsEnabled = false;
            this.validateSwapMapButton.IsEnabled = false;

        }

        private void mapOrderListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (mapOrderListView.SelectedIndex == 15) 
            {
                // Déselectionne cet élément
                mapOrderListView.SelectedIndex = -1;

                // Bloque les contrôles associés
                mapNameReplacerComboBox.IsEnabled = false;
                validateSwapMapButton.IsEnabled = false;

                return;
            }

            this.mapNameReplacerComboBox.IsEnabled = true;
            this.mapNameReplacerComboBox.SelectedIndex = -1;
            this.validateSwapMapButton.IsEnabled = false;
        }

        private void mapNameReplacerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            this.validateSwapMapButton.IsEnabled = true;
        }

        private void validateSwapMapButton_Click(object sender, RoutedEventArgs e)
        {

            if (mapOrderListView.SelectedItem is MapItem selected)
            {
                string new_mapname = (string)this.mapNameReplacerComboBox.SelectedItem ;

                selected.Map = new_mapname.Split('(', ')')[1].Trim();
                this.mapList[selected.Name] = selected.Map;
            }

            this.mapNameReplacerComboBox.SelectedIndex = -1;
            this.mapNameReplacerComboBox.IsEnabled = false;
            this.validateSwapMapButton.IsEnabled = false;
        }





        private class MapItem : INotifyPropertyChanged
        {
            private string _map;

            public string Name { get; set; }

            public string Map
            {
                get => _map;
                set
                {
                    if (_map != value)
                    {
                        _map = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Map)));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        private void validateAllButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = Application.Current.MainWindow as MainWindow;

            if (main != null)
            {
                var converted = new Dictionary<short, string>();

                for(short i = 1; i<26;i++  )
                {
                    converted.Add(i, this.mapList.ElementAt(i-1).Value );
                }

                main.wee.SaveNewMapOrder( converted );
                main.InitMapList();

                this.Close();
            }
        }
    }
}
