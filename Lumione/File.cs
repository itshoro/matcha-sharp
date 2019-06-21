using System;
using System.Collections.Generic;
using System.Text;

namespace Lumione
{
    public enum FileType
    {
        Document,
        Stylesheet,
        Script,
        Asset
    }

    public class File
    {
        public FileType FileType { get; }
        public string Path { get; }

        private File() {}
        public File(string path) 
        {
            Path = path;
            FileType = GetFileType(System.IO.Path.GetExtension(path));
        }
        public File(string path, FileType type)
        {
            Path = path;
            FileType = type;
        }
        private FileType GetFileType(string fileExtension) {
            switch (fileExtension) {
                case ".html":
                    return FileType.Document;
                case ".htm":
                    return FileType.Document;
                case ".scss":
                    return FileType.Stylesheet;
                case ".css":
                    return FileType.Stylesheet;
                case ".js":
                    return FileType.Script;
                default:
                    return FileType.Asset;
            }
        }
    }
}