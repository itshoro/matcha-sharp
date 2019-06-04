using System;
using System.Collections.Generic;
using System.Text;

namespace Lumione
{
    internal class FileAccess : IFileAccess
    {
        public string Read(string path)
        {
            if (System.IO.File.Exists(path))
                return System.IO.File.ReadAllText(path);
            throw new ArgumentException($"File \"{path}\" was not found.");
        }

        public void Write(string path, string content)
        {
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            System.IO.File.WriteAllText(path, content);
        }
    }
}