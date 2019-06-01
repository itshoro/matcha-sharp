using System;
using System.Collections.Generic;

namespace Lumione
{
    public class Settings
    {
        public string DestinationPath { get; set; }
        public string IncludePath { get; set; }
        public string SourcePath { get; set; }
        public string AssetsPath { get; set; }
        public List<string> IgnoredDirectories { get; set; }

        public Settings()
        {
        }

        private Settings(string path)
        {
            SourcePath = path;
            DestinationPath = path + @"\_build";
            IncludePath = path + @"\_includes";
            AssetsPath = path + @"\assets";
            IgnoredDirectories = new List<string>();
            IgnoredDirectories.Add(IncludePath);
            IgnoredDirectories.Add(DestinationPath);
        }

        public static Settings Default(string path)
        {
            return new Settings(path);
        }
    }
}