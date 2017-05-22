using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BlokboekParkeergarageSimulator.Functions
{
    class CheckConnection
    {
        public static bool CheckInternet()
        {
            string InternetStatus = "false";
            try
            {
                //Met deze ping kijkt of de api server online is en of je internet hebt
                Ping myPing = new Ping();
                String host = "avondmtb.nl";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                InternetStatus = reply.ToString();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
