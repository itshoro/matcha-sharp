using System;
using System.Collections.Generic;

namespace Lumione
{
    public class Settings
    {
        public string ProjectPath { get; set; }
        public bool CreateSubdirectories { get; set; }
        public string JavascriptFolderName { get; set; } = "js";
        public string AssetsFolderName { get; set; } = "assets";
        public string CssFolderName { get; set; } = "css";
        public string DestinationFolderName { get; set; } = "build";
    }
}