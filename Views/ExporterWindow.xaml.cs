using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            status_progress_bar.Visibility = Visibility.Hidden;

            //Generate_thumbnails();
        }


        private void letsgo_button_Click(object sender, RoutedEventArgs e)
        {
            letsgo_button.Visibility = Visibility.Hidden;
            status_progress_bar.Visibility = Visibility.Visible;
            exportmaps_checkbox.IsEnabled = false;
            exportchars_checkbox.IsEnabled = false;
            exportskybox_checkbox.IsEnabled = false;
            exportlanguages_checkbox.IsEnabled = false;
            exportmoddedmad_checkbox.IsEnabled = false;
            exportaudio_checkbox.IsEnabled = false;
            export();
        }

        public async void export()
        {
            //GlobalVars.Export_FEBmps();
            List<Task> tasks = [];

            if (exportmaps_checkbox.IsChecked == true) { tasks.Add(Task.Run(GlobalVars.ExportMapsAndModels)); }
            if (exportchars_checkbox.IsChecked == true) { tasks.Add(Task.Run(GlobalVars.ExportCharsFolder)); }
            if (exportskybox_checkbox.IsChecked == true) { tasks.Add(Task.Run(GlobalVars.ExportSkyboxes)); }
            if (exportlanguages_checkbox.IsChecked == true) { tasks.Add(Task.Run(GlobalVars.ExportLanguages)); }
            if (exportmoddedmad_checkbox.IsChecked == true) { tasks.Add(Task.Run(GlobalVars.MadMtdModdingTool)); }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks); //need to wait everything is done before 
            }

            //heavy stuff there, need your PC resources  !
            if (exportaudio_checkbox.IsEnabled) { GlobalVars.ExportAudioAndSounds(); }


            //todo : messagebox "do you want to zip" -> yes = new thread( zip ) + reset progress bar / process mode | no = close

            /*
            if (File.Exists(GlobalVars.gameFolder + "/devtools/Export.zip") == true)
            {
                File.Delete(GlobalVars.gameFolder + "/devtools/Export.zip");
            }
            status_progress_bar.IsIndeterminate = true;
            ZipFile.CreateFromDirectory(GlobalVars.exportFolder, GlobalVars.gameFolder + "/devtools/Export.zip", CompressionLevel.SmallestSize, false);
            */
            status_progress_bar.IsIndeterminate = false;
            status_progress_bar.Value = 100;
            await Task.Delay(1000);
            Close();

        }


        private void Generate_thumbnails()
        {
            string output = GlobalVars.exportFolder + "thumbnails/";
            Directory.CreateDirectory(output);

            foreach (string file in Directory.GetFiles(GlobalVars.exportFolder + "models/"))
            {
                string arguments = file + " --output=" + output + Path.GetFileNameWithoutExtension(file) + ".png --no-background --grid=false --filename=false --metadata=false --resolution=256,256 --axis=false --camera-zoom-factor=1.1 --light-intensity=7";

                ProcessStartInfo psi = new()
                {
                    FileName = "C:/Program Files/F3D/bin/f3d.exe",
                    Arguments = arguments,
                    UseShellExecute = false,     // pas de nouvelle console Windows
                    CreateNoWindow = true,       // pas de fenêtre visible
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                // Démarre le processus
                using Process process = new();
                process.StartInfo = psi;
                process.Start();

                process.WaitForExit();

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
