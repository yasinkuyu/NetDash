// Copyright (c) 2014, Insya Interaktif.
// Developer @yasinkuyu
// All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Insya.NetDash
{
    public static class Settings   
    {
        private static string _settingsPath = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/Setting.ini";
        private static string _path;
        private static string _exe = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string value, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string Default, StringBuilder retVal, int size, string filePath);

        static Settings()
        {
            _path = new FileInfo(_settingsPath).FullName;
        }

        public static string Get(string key, string section = null)
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section ?? _exe, key, "", retVal, 255, _path);
            return retVal.ToString();
        }

        public static List<String> GetList(string key, string section = null)
        {
            var retVal = new StringBuilder(255);
            GetPrivateProfileString(section ?? _exe, key, "", retVal, 255, _path);

            var listRead = new List<String>(retVal.ToString().Split(' '));

            return listRead;
        }

        public static void Set(string key, string value, string section = null)
        {
            WritePrivateProfileString(section ?? _exe, key, value, _path);
        }

        public static void Put(string key, string section = null)
        {
            Set(key, null, section ?? _exe);
        }

        public static void DeleteSection(string section = null)
        {
            Set(null, null, section ?? _exe);
        }

        public static bool KeyExists(string key, string section = null)
        {
            return Get(key, section).Length > 0;
        }
    }
}
