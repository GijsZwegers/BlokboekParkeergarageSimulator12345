using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BlokboekParkeergarageSimulator.Functions
{
    class CheckConnection
    {
        public static bool CheckInternet()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create("https://www.avondmtb.nl");
            request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
            request.Method = "HEAD";
            request.Timeout = 3000;
            try
            {
                
                // do something with response.Headers to find out information about the request
                return true;
            }
            catch (WebException)
            {
                //set flag if there was a timeout or some other issues
                return false;
            }
        }
    }
}
