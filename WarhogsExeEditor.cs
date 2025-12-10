using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hogs_gameEditor_wpf
{
    public class WarhogsExeEditor
    {
        public static byte[] exe =  File.ReadAllBytes(GlobalVars.gameFolder+"warhogs.exe");

        public Dictionary<short, string> mapOrder { get; set; }

        public WarhogsExeEditor() 
        {
            GetCurrentMapOrder();
        }

        private string GetNameFromAdress(long adress)
        {
            byte[] result = new byte[12];

            if( adress == 0x000D0AC0 )
            {
                Array.Copy(exe, adress, result, 0, 4);
            }
            else if( adress == 0x000D0AE4 ) 
            {
                Array.Copy(exe, adress, result, 0, 12);
            }
            else
            {
                Array.Copy(exe, adress, result, 0, 8);
            }

            return Encoding.ASCII.GetString(result).Trim('\0'); ;
        }

        private void SetNameIntoAdress(long adress,string mapname)
        {

            if (adress == 0x000D0AC0) //bay case 
            {
                for (int i = 0; i < 4; i++)
                {
                    exe[adress + i] = i < mapname.Length ? (byte)mapname[i] : (byte)0;
                }
            }
            else if (adress == 0x000D0AE4)
            {
                for (int i = 0; i < 12; i++)
                {
                    exe[adress + i] = i < mapname.Length ? (byte)mapname[i] : (byte)0;
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    exe[adress + i] = i < mapname.Length ? (byte)mapname[i] : (byte)0;
                }
            }


        }

        public Dictionary<short,string> GetCurrentMapOrder()
        {
            this.mapOrder = new();

            mapOrder.Add(1,  GetNameFromAdress(0x000D0A78)); //ESTU 
            mapOrder.Add(2,  GetNameFromAdress(0x000D0B40)); //ROAD 
            mapOrder.Add(3,  GetNameFromAdress(0x000D0B38)); //TRENCH
            mapOrder.Add(4,  GetNameFromAdress(0x000D0B28)); //DEVI 
            mapOrder.Add(5,  GetNameFromAdress(0x000D0B30)); //RUMBLE
            mapOrder.Add(6,  GetNameFromAdress(0x000D0B18)); //ZULUS
            mapOrder.Add(7,  GetNameFromAdress(0x000D0B20)); //TWIN
            mapOrder.Add(8,  GetNameFromAdress(0x000D0B10)); //SNIPER
            mapOrder.Add(9,  GetNameFromAdress(0x000D0AF8)); //MASHED
            mapOrder.Add(10, GetNameFromAdress(0x000D0B08)); //GUNS
            mapOrder.Add(11, GetNameFromAdress(0x000D0AE4)); //LIBERATE
            mapOrder.Add(12, GetNameFromAdress(0x000D0B00)); //OASIS
            mapOrder.Add(13, GetNameFromAdress(0x000D0AD4)); //FJORDS
            mapOrder.Add(14, GetNameFromAdress(0x000D0ACC)); //EYRIE
            mapOrder.Add(15, GetNameFromAdress(0x000D0AC0)); //BAY
            mapOrder.Add(16, GetNameFromAdress(0x000D0ADC)); //MEDIX
            mapOrder.Add(17, GetNameFromAdress(0x000D0AC4)); //BRIDGE
            mapOrder.Add(18, GetNameFromAdress(0x000D0AB8)); //DESVAL
            mapOrder.Add(19, GetNameFromAdress(0x000D0AB0)); //SNAKE
            mapOrder.Add(20, GetNameFromAdress(0x000D0AA8)); //EMPLACE
            mapOrder.Add(21, GetNameFromAdress(0x000D0A98)); //SUPLINE
            mapOrder.Add(22, GetNameFromAdress(0x000D0AA0)); //KEEP
            mapOrder.Add(23, GetNameFromAdress(0x000D0A90)); //TESTER
            mapOrder.Add(24, GetNameFromAdress(0x000D0A88)); //FOOT
            mapOrder.Add(25, GetNameFromAdress(0x000D0A80)); //FINAL

            return mapOrder;
        }


        public void SaveNewMapOrder( Dictionary<short, string> newMapOrder )
        {
            this.mapOrder = newMapOrder;

            SetNameIntoAdress(0x000D0A78, mapOrder[1]  ); //ESTU 
            SetNameIntoAdress(0x000D0B40, mapOrder[2]  ); //ROAD 
            SetNameIntoAdress(0x000D0B38, mapOrder[3]  ); //TRENCH
            SetNameIntoAdress(0x000D0B28, mapOrder[4]  ); //DEVI 
            SetNameIntoAdress(0x000D0B30, mapOrder[5]  ); //RUMBLE
            SetNameIntoAdress(0x000D0B18, mapOrder[6]  ); //ZULUS
            SetNameIntoAdress(0x000D0B20, mapOrder[7]  ); //TWIN
            SetNameIntoAdress(0x000D0B10, mapOrder[8]  ); //SNIPER
            SetNameIntoAdress(0x000D0AF8, mapOrder[9]  ); //MASHED
            SetNameIntoAdress(0x000D0B08, mapOrder[10] ); //GUNS
            SetNameIntoAdress(0x000D0AE4, mapOrder[11] ); //LIBERATE
            SetNameIntoAdress(0x000D0B00, mapOrder[12] ); //OASIS
            SetNameIntoAdress(0x000D0AD4, mapOrder[13] ); //FJORDS
            SetNameIntoAdress(0x000D0ACC, mapOrder[14] ); //EYRIE
            SetNameIntoAdress(0x000D0AC0, mapOrder[15] ); //BAY
            SetNameIntoAdress(0x000D0ADC, mapOrder[16] ); //MEDIX
            SetNameIntoAdress(0x000D0AC4, mapOrder[17] ); //BRIDGE
            SetNameIntoAdress(0x000D0AB8, mapOrder[18] ); //DESVAL
            SetNameIntoAdress(0x000D0AB0, mapOrder[19] ); //SNAKE
            SetNameIntoAdress(0x000D0AA8, mapOrder[20] ); //EMPLACE
            SetNameIntoAdress(0x000D0A98, mapOrder[21] ); //SUPLINE
            SetNameIntoAdress(0x000D0AA0, mapOrder[22] ); //KEEP
            SetNameIntoAdress(0x000D0A90, mapOrder[23] ); //TESTER
            SetNameIntoAdress(0x000D0A88, mapOrder[24] ); //FOOT
            SetNameIntoAdress(0x000D0A80, mapOrder[25] ); //FINAL

            WriteExe();

        }

        private void WriteExe()
        {
            File.WriteAllBytes(GlobalVars.gameFolder+"warhogs.exe", exe);
        }
    }
}
