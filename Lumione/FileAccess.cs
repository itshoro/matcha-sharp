using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lumione
{
    internal class FileAccess : IFileAccess
    {
        public string ReadFromRoot(Project project, Settings settings, string path)
        {
            path = project.Uri.AbsolutePath + @"\" + path;

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

        public async Task<string> ReadAsync(string path)
        {
            return await Task.Run(async () =>
            {
                if (System.IO.File.Exists(path))
                    return await System.IO.File.ReadAllTextAsync(path);
                throw new ArgumentException($"File \"{path}\" was not found.");
            });
        }

        public async Task WriteAsync(string path, string contents)
        {
            await Task.Run(async () =>
           {
               if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                   System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
               await System.IO.File.WriteAllTextAsync(path, contents);
           });
        }

        public bool FileExists(params string[] files)
        {
            foreach (var path in files)
            {
                if (!System.IO.File.Exists(path))
                    return false;
            }
            return true;
        }

        public IEnumerable<string> GetFiles(string path)
        {
            return System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories);
        }

        public bool DirectoryExists(params string[] dirs)
        {
            foreach (var path in dirs)
            {
                if (!System.IO.File.Exists(path))
                    return false;
            }
            return true;
        }
    }
}