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
    }
}
