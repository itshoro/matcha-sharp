using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lumione.Invokers;

namespace Lumione
{
    internal class Project : IProject
    {
        public string BasePath { get; private set; }
        public string StylesPath { get; private set; }
        public string IncludesPath { get; private set; }
        public string ScriptsPath { get; private set; }
        public string AssetsPath { get; private set; }
        public string DestinationPath { get; private set; }

        private readonly bool destinationIsRelative;

        private ICollection<File> files;
        private readonly bool putInSubdirectories;

        private Project()
        {
        }

        public Project(string basePath)
        {
            BasePath = basePath;
            DestinationPath = @"\_build";
            StylesPath = @"\_stylesheet";
            IncludesPath = @"\_includes";
            ScriptsPath = @"\_scripts";
            AssetsPath = @"\_assets";

            destinationIsRelative = true;
            putInSubdirectories = false;

            files = new List<File>();
            var f = System.IO.Directory.GetFiles(basePath, "*", System.IO.SearchOption.AllDirectories);
            foreach (var file in f)
            {
                File projectFile = new File(file.Remove(0, BasePath.Length), GetType(System.IO.Path.GetExtension(file)));
                files.Add(projectFile);
            }
        }

        internal void Prepare()
        {
            System.IO.Directory.CreateDirectory(BasePath + StylesPath);
            System.IO.Directory.CreateDirectory(BasePath + ScriptsPath);
            System.IO.Directory.CreateDirectory(BasePath + AssetsPath);
            System.IO.Directory.CreateDirectory(BasePath + IncludesPath);
            if (destinationIsRelative)
            {
                System.IO.Directory.CreateDirectory(BasePath + DestinationPath);
            }
            else
            {
                System.IO.Directory.CreateDirectory(DestinationPath);
            }
        }

        public Project(string basePath, string destinationPath)
        {
            BasePath = basePath;
            DestinationPath = destinationPath;
            StylesPath = @"\_stylesheet";
            IncludesPath = @"\_includes";
            ScriptsPath = @"\_scripts";
            AssetsPath = @"\_assets";

            destinationIsRelative = false;
            putInSubdirectories = false;

            files = new List<File>();
            var f = System.IO.Directory.GetFiles(basePath, "*", System.IO.SearchOption.AllDirectories);
            foreach (var file in f)
            {
                File projectFile = new File(file.Remove(0, BasePath.Length), GetType(System.IO.Path.GetExtension(file)));
                files.Add(projectFile);
            }
        }

        private FileType GetType(string extension)
        {
            switch (extension)
            {
                case ".html":
                    return FileType.Document;

                case ".htm":
                    return FileType.Document;

                case ".sass":
                    return FileType.Stylesheet;

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

        public IEnumerable<File> GetFiles()
        {
            return files.Where(o => !o.Path.StartsWith(IncludesPath) && !o.Path.StartsWith(DestinationPath));
        }

        public void PrepareBuild()
        {
            var destinationBase = destinationIsRelative ? BasePath + DestinationPath : DestinationPath;

            System.IO.Directory.CreateDirectory(destinationBase);
            System.IO.Directory.CreateDirectory(destinationBase + StylesPath);
            System.IO.Directory.CreateDirectory(destinationBase + ScriptsPath);
            System.IO.Directory.CreateDirectory(destinationBase + AssetsPath);
        }

        public void AddToDestination(File file, string contents)
        {
            new FileAccess().Write(BasePath + DestinationPath + file.Path, contents);
        }

        public string GetFileContents(File file)
        {
            return GetFileContents(file.Path);
        }

        public string GetFileContents(string relativePath, Scope scope = Scope.Root)
        {
            if (!relativePath.StartsWith(@"\"))
                relativePath = @"\" + relativePath;
            return new FileAccess().Read(BasePath + GetDirectoryFromScope(scope) + relativePath);
        }

        private string GetDirectoryFromScope(Scope scope)
        {
            switch (scope)
            {
                case Scope.Include:
                    return IncludesPath;

                default:
                    return string.Empty;
            }
        }

        public bool HasFile(string relativePath, Scope scope = Scope.Root)
        {
            if (!relativePath.StartsWith(@"\"))
                relativePath = @"\" + relativePath;
            return System.IO.File.Exists(BasePath + GetDirectoryFromScope(scope) + relativePath);
        }
    }
}