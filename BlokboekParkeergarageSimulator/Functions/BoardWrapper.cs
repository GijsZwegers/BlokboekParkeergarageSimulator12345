using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace BlokboekParkeergarageSimulator.Functions
{
    class BoardWrapper
    {
        [DllImport("Dll/K8055D.dll")]
        public static extern int OpenDevice(int iAdress);
        [DllImport("Dll/K8055D.dll")]
        public static extern void CloseDevice();
        [DllImport("Dll/K8055D.dll")]
        public static extern void SetDigitalChannel(int iOutputChannel);
        [DllImport("Dll/K8055D.dll")]
        public static extern void ClearAllDigital();
        [DllImport("Dll/K8055D.dll")]
        public static extern bool ReadDigitalChannel(int r);
        [DllImport("Dll/K8055D.dll")]
        public static extern bool ClearDigitalChannel(int t);

        private static object Locker = new object();
        //Slagboom open zetten TODO: Kijken of slagboom open int 1 is of int 2
        public static void ZetLampjeAan(int iLampnummer)
        {
            for (int i = 0; i < 4; i++)
            {
                if (OpenDevice(i) == i)
                {
                    ClearAllDigital();
                    SetDigitalChannel(iLampnummer);
                    Thread.Sleep(500);
                    CloseDevice();
                }
            }
        }
        public static bool[] magnet()
        {
            bool[] Return = new bool[5];
            lock (Locker)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (OpenDevice(i) == i)
                    {
                        Return[0] = ReadDigitalChannel(1);
                        Return[1] = ReadDigitalChannel(2);
                        Return[2] = ReadDigitalChannel(3);
                        Return[3] = ReadDigitalChannel(4);
                        Return[4] = ReadDigitalChannel(5);
                        CloseDevice();
                    }
                }
            }
            return Return;
        }
    }
}
