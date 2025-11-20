using System.Collections.Generic;
using System.Windows.Media;

namespace hogs_gameEditor_wpf.FileFormat
{
    internal class HOGSMAP
    {
        private string mapName { get; set; }
        private PMG pmg { get; set; }
        private PTG mapTiles { get; set; }
        private List<POG> mapEntities { get; set; }
        private List<MAD> entitiesAssets { get; set; }
        private string skybox { get; set; }
        private Weather weather { get; set; } //skycolor, fog, rain, clouds (created from openHOW .map data) 

        public HOGSMAP() { }


    }

    public class Weather
    {
        public string AmbientColour { get; set; }
        public Color SkyColourTop { get; set; }
        public Color SkyColourBottom { get; set; }
        public Color SunColour { get; set; }
        public float SunYaw { get; set; }
        public float SunPitch { get; set; }
        public string Temperature { get; set; }
        public string Type { get; set; }
        public string Time { get; set; }
        public Color FogColour { get; set; }
        public float FogIntensity { get; set; }
        public float FogDistance { get; set; }

        public Weather() { }
    }
}
