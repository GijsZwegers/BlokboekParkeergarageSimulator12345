using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlokboekParkeergarageSimulator.JsonClasses
{
    class newtonsoftJSON
    {
        //root class
        public class RootObject
        {
            public bool success { get; set; }
            public string message { get; set; }
            public string token { get; set; }
            public int rechten { get; set; }
            public User user { get; set; }
            public List<Log> log { get; set; }
            public string userinfo { get; set; }
        }
        //De User class 
        public class User
        {
            public string User_LastLogin { get; set; }
            public string Name_First { get; set; }
            public object Name_Middle { get; set; }
            public string Name_Last { get; set; }
            public string User_Mail { get; set; }
            public string User_Phone { get; set; }
        }
        public class Log
        {
            public string Timestamp_Log { get; set; }
            public string Log_Client { get; set; }
            public string Log_User { get; set; }
            public string Log_Description { get; set; }
        }

    }
}