using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Json;

namespace BlokboekParkeergarageSimulator.ApiClass
{
    class ApiFuncionClass
    {
        //Random Quote voor niks, verwijder als je dit ziet #cuzyolo Urhg.. I Don't have enough power to destroy it....

            static string _sToken;
            static string _sNiveau;

            public string Token()
            {
                return _sToken;
            }
            public string Niveau()
            {
                return _sNiveau;
            }

            public string Login(string url, string json)
            {
                //token ophalen en returnen 
                string UserAccount = POSTapi(url, json);

                try
                {
                    _sToken = GetValueFromJson(UserAccount, "token");
                    _sNiveau = GetValueFromJson(UserAccount, "rechten");
                }
                catch (Exception) {}

                return UserAccount;
            }


        //Deze functie gebruiken wanneer je een POST hebt warbij je WEL een token nodig hebt
        public string POSTapiToken(string url, string json)
            {
                string s;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("auth", "Bearer " + _sToken);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string data = json;

                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    s = streamReader.ReadToEnd();
                }
                return s;
            }
            //Deze functie gebruiken wanneer je een post hebt warbij je GEEN token nodig hebt
            public string POSTapi(string url, string json)
            {
                string s;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string data = json;

                    streamWriter.Write(data);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    s = streamReader.ReadToEnd();
                }
                return s;
            }
            //Deze functie gebruiken wanneer je een GET hebt waarbij je WEL een token nodig hebt
            public string GETapiToken(string url)
            {
                string s;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.PreAuthenticate = true;
                httpWebRequest.Headers.Add("auth", "Bearer " + _sToken);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    s = streamReader.ReadToEnd();
                }
                return s;
            }
            public string GetValueFromJson(string json, string key)
            {

                IDictionary<string, object> dict = JsonParser.FromJson(json);
                return dict.Where(a => a.Key == key).Select(a => a.Value).FirstOrDefault().ToString();
            
        }
    }
}
