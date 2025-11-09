using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hogs_gameEditor_wpf
{
    /// <summary>
    /// Interaction logic for Exporter.xaml
    /// </summary>
    public partial class ExporterWindow : Window
    {
        public ExporterWindow()
        {
            InitializeComponent();
            status_progress_bar.Value = 0;
            this.status_progress_bar.Visibility = Visibility.Hidden;
        }


        private void letsgo_button_Click(object sender, RoutedEventArgs e)
        {
            this.letsgo_button.Visibility = Visibility.Hidden;
            this.status_progress_bar.Visibility = Visibility.Visible;
            this.exportmaps_checkbox.IsEnabled = false;
            this.exportchars_checkbox.IsEnabled = false;
            this.exportskybox_checkbox.IsEnabled = false;
            this.exportlanguages_checkbox.IsEnabled = false;
            this.exportmoddedmad_checkbox.IsEnabled = false;
            this.exportaudio_checkbox.IsEnabled = false;
            export();
        }

        public async void export()
        {
            //GlobalVars.Export_FEBmps();
            var tasks = new List<Task>();


            if (this.exportmaps_checkbox.IsEnabled)      { tasks.Add(Task.Run(GlobalVars.ExportMapsAndModels)); }
            if (this.exportchars_checkbox.IsEnabled)     { tasks.Add(Task.Run(GlobalVars.ExportCharsFolder)); }
            if (this.exportskybox_checkbox.IsEnabled)    { tasks.Add(Task.Run(GlobalVars.ExportSkyboxes)); }
            if (this.exportlanguages_checkbox.IsEnabled) { tasks.Add(Task.Run(GlobalVars.ExportLanguages)); }
            if (this.exportmoddedmad_checkbox.IsEnabled) { tasks.Add(Task.Run(GlobalVars.MadMtdModdingTool)); }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks);
            }

            //heavy stuff there, need your PC resources  !
            if (this.exportmoddedmad_checkbox.IsEnabled) { GlobalVars.ExportAudioAndSounds(); }


            /*
            if (File.Exists(GlobalVars.gameFolder + "/devtools/Export.zip") == true)
            {
                File.Delete(GlobalVars.gameFolder + "/devtools/Export.zip");
            }

            ZipFile.CreateFromDirectory(GlobalVars.exportFolder, GlobalVars.gameFolder + "/devtools/Export.zip", CompressionLevel.SmallestSize, false);
            */

            this.status_progress_bar.Value = 100;
            



        }


        // Méthode pour mettre à jour la ProgressBar en toute sécurité
        public void IncrementProgress()
        {
            // Dispatcher.Invoke s'assure qu'on est sur le thread UI
            status_progress_bar.Dispatcher.Invoke(() =>
            {
                status_progress_bar.Value += 0.039;
                //status_progress_bar.Value += 0.05;
            });
        }


    }
}
