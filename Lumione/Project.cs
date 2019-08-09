using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumione
{
    public class Project
    {
        public List<File> Files { get; private set; }
        public int FileCount { get { return (Files as ICollection<File>).Count; } }
        public Uri Uri { get; private set; }

        private Project()
        {
        }

        public Project(Uri uri, IFileAccess access, Settings settings)
        {
            Uri = uri;
            Files = new List<File>();

            var f = uri.IsAbsoluteUri ? access.GetFiles(Uri.AbsolutePath) : access.GetFiles(Uri.LocalPath);

            if (!settings.HasAbsoluteDestinationPath)
            {
                foreach (var file in f)
                {
                    File projectFile = new File(file.Remove(0, Uri.AbsolutePath.Length));
                    Files.Add(projectFile);
                }
            }
            else
            {
                foreach (var file in f)
                {
                    if (!file.StartsWith(settings.DestinationPath))
                    {
                        File projectFile = new File(file.Remove(0, Uri.AbsolutePath.Length));
                        Files.Add(projectFile);
                    }
                }
            }
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
            if (!settings.HasAbsoluteDestinationPath)
            {
                System.IO.Directory.CreateDirectory(Uri.AbsolutePath + @"\" + settings.DestinationFolderName);
            }
            System.IO.Directory.CreateDirectory(Uri.AbsolutePath + @"\" + settings.AssetsFolderName);
            System.IO.Directory.CreateDirectory(Uri.AbsolutePath + @"\" + settings.CssFolderName);
            System.IO.Directory.CreateDirectory(Uri.AbsolutePath + @"\" + settings.JavascriptFolderName);
            foreach (var reserved in reservedDirectories)
            {
                System.IO.Directory.CreateDirectory(Uri.AbsolutePath + @"\" + reserved);
            }
        }

        internal bool HasFile(string value)
        {
            return Files.Any(f => f.Path == value);
        }
    }
}