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
        [DllImport("K8055D.dll")]
        public static extern int OpenDevice(int iAdress);
        [DllImport("K8055D.dll")]
        public static extern void CloseDevice();
        [DllImport("K8055D.dll")]
        public static extern void SetDigitalChannel(int iOutputChannel);
        [DllImport("K8055D.dll")]
        public static extern void ClearAllDigital();


        //Slagboom open zetten TODO: Kijken of slagboom open int 1 is of int 2
        public static void SlagboomOpen()
        {
            {
                if (OpenDevice(1) == 1)
                {
                    ClearAllDigital();
                    SetDigitalChannel(1);
                    Thread.Sleep(500);
                    CloseDevice();
                }
            }
        }

        //Slagboom dicht zetten TODO: Kijken of slagboom dicht int 2 is of 1 
        public static void SlagboomDicht()
        {
            {
                if (OpenDevice(2) == 2)
                {
                    ClearAllDigital();
                    SetDigitalChannel(2);
                    Thread.Sleep(500);
                    CloseDevice();
                }
            }
        }
    }
}
