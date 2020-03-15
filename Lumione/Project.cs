using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Lumione
{
    public class Project
    {
        public IEnumerable<File> Files { get; private set; }
        public int FileCount { get { return (Files as ICollection<File>).Count; } }
        public string Directory { get; private set; }

        private Project()
        {
        }

        public Project(string path, IFileAccess access, Settings settings)
        {
            IEnumerable<File> generateProjectFiles(IEnumerable<string> applicableFilePaths)
            {
                foreach (var filePath in applicableFilePaths)
                    yield return new File(filePath);
            };

            Directory = path;
            Files = new List<File>();

            Files = generateProjectFiles(access.GetFiles(Directory, settings));
        }

        public IEnumerable<File> GetCompilableFiles(IEnumerable<string> reservedDirectories)
        {
            foreach (var file in Files)
            {
                if (reservedDirectories.Any(dir => !file.Path.StartsWith(dir)))
                    yield return file;
            }
        }

        public void Prepare(Settings settings, IEnumerable<string> reservedDirectories)
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.Join(Directory, settings.AssetsFolderName));
            System.IO.Directory.CreateDirectory(System.IO.Path.Join(Directory, settings.CssFolderName));
            System.IO.Directory.CreateDirectory(System.IO.Path.Join(Directory, settings.JavascriptFolderName));
            foreach (var reserved in reservedDirectories)
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Join(Directory, reserved));
            }
        }

        internal bool HasFile(string value)
        {
            return Files.Any(f => f.Path == value);
        }
    }
}