using System;
using System.Collections.Generic;
using System.Text;

namespace Lumione
{
    public class SettingsManager
    {
        public string BuildPath { get; private set; }
        public string BasePath { get; private set; }
        public List<string> IgnoredDirectories { get; private set; }
        public SettingsManager(string path)
        {
            BasePath = path;
            BuildPath = path + @"\build";
            IgnoredDirectories = new List<string>();
            IgnoredDirectories.Add(BuildPath);
        }
    }
}
