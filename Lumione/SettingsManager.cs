using System;
using System.Collections.Generic;
using System.Text;

namespace Lumione
{
    [Serializable]
    public class SettingsManager
    {
        public string BuildPath { get; private set; }
        public string BasePath { get; private set; }
        public List<string> IgnoredDirectories { get; private set; }
        public SettingsManager(string path)
        {
            BasePath = path;
            BuildPath = path + @"\_build";
            BuildPath = path + @"\_includes";
            IgnoredDirectories = new List<string>();
            IgnoredDirectories.Add(BuildPath);
        }

        private void Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
