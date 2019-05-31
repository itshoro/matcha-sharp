using System.Collections.Generic;

namespace Lumione
{
    public class Settings
    {
        public string BuildPath { get; private set; }
        public string IncludePath { get; private set; }
        public string BasePath { get; private set; }
        public List<string> IgnoredDirectories { get; private set; }

        public Settings(string path)
        {
            BasePath = path;
            BuildPath = path + @"\_build";
            IncludePath = path + @"\_includes";
            IgnoredDirectories = new List<string>();
            IgnoredDirectories.Add(IncludePath);
            IgnoredDirectories.Add(BuildPath);
        }
    }
}