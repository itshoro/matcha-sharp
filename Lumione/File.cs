using System;
using System.Collections.Generic;
using System.Text;

namespace Lumione
{
    internal enum FileType
    {
        Document,
        Stylesheet,
        Script,
        Asset
    }

    internal class File
    {
        public FileType FileType { get; private set; }
        public string Path { get; set; }

        public File(string path, FileType type)
        {
            Path = path;
            FileType = type;
        }
    }
}