
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


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

            //Generate_thumbnails();
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

            if (this.exportmaps_checkbox.IsChecked == true)      { tasks.Add(Task.Run(GlobalVars.ExportMapsAndModels)); }
            if (this.exportchars_checkbox.IsChecked == true)     { tasks.Add(Task.Run(GlobalVars.ExportCharsFolder)); }
            if (this.exportskybox_checkbox.IsChecked == true)    { tasks.Add(Task.Run(GlobalVars.ExportSkyboxes)); }
            if (this.exportlanguages_checkbox.IsChecked == true) { tasks.Add(Task.Run(GlobalVars.ExportLanguages)); }
            if (this.exportmoddedmad_checkbox.IsChecked == true) { tasks.Add(Task.Run(() => { GlobalVars.MadMtdModdingTool(); GlobalVars.MadMtdModdingTool(true); } )); }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks); //need to wait everything is done before 
            }

            //heavy stuff there, need your PC resources  !
            if (this.exportmoddedmad_checkbox.IsEnabled) { GlobalVars.ExportAudioAndSounds(); }


            //todo : messagebox "do you want to zip" -> yes = new thread( zip ) + reset progress bar / process mode | no = close

            /*
            if (File.Exists(GlobalVars.gameFolder + "/devtools/Export.zip") == true)
            {
                File.Delete(GlobalVars.gameFolder + "/devtools/Export.zip");
            }

            ZipFile.CreateFromDirectory(GlobalVars.exportFolder, GlobalVars.gameFolder + "/devtools/Export.zip", CompressionLevel.SmallestSize, false);
            */

            this.status_progress_bar.Value = 100;
            await Task.Delay(1000);
            this.Close();

        }


        private void Generate_thumbnails()
        {
            string output = GlobalVars.exportFolder + "thumbnails/";
            Directory.CreateDirectory(output);

            foreach (string file in Directory.GetFiles(GlobalVars.exportFolder + "models/") )
            {
                string arguments = file + " --output=" + output + Path.GetFileNameWithoutExtension(file) + ".png --no-background --grid=false --filename=false --metadata=false --resolution=256,256 --axis=false --camera-zoom-factor=1.1 --light-intensity=7";
                
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "C:/Program Files/F3D/bin/f3d.exe",
                    Arguments = arguments,
                    UseShellExecute = false,     // pas de nouvelle console Windows
                    CreateNoWindow = true,       // pas de fenêtre visible
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                // Démarre le processus
                using (Process process = new Process())
                {
                    process.StartInfo = psi;
                    process.Start();

                    process.WaitForExit();
                }

            }

        }

        // Méthode pour mettre à jour la ProgressBar en toute sécurité
        public void IncrementProgress()
        {
            // Dispatcher.Invoke s'assure qu'on est sur le thread UI
            status_progress_bar.Dispatcher.Invoke(() =>
            {
                status_progress_bar.Value += 0.0365; 
            });
        }


    }
}
