using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lumione
{
    public class LocalProject : IProject
    {
        public string BasePath { get; private set; }
        public string StylesPath { get; private set; }
        public string IncludesPath { get; private set; }
        public string ScriptsPath { get; private set; }
        public string AssetsPath { get; private set; }
        public string DestinationPath { get; private set; }
        private ICollection<File> files;
        public bool PutInSubdirectories { get; set; }
        public bool DestinationIsRelative { get; private set; }

        private LocalProject()
        {
        }

        public LocalProject(string basePath, IFileAccess access)
        {
            BasePath = basePath;
            DestinationPath = @"\_build";
            StylesPath = @"\_stylesheet";
            IncludesPath = @"\_includes";
            ScriptsPath = @"\_scripts";
            AssetsPath = @"\_assets";

            DestinationIsRelative = true;
            PutInSubdirectories = false;

            files = new List<File>();
            var f = access.GetFiles(basePath);
            foreach (var file in f)
            {
                File projectFile = new File(file.Remove(0, BasePath.Length));
                files.Add(projectFile);
            }
        }

        internal void Prepare()
        {
            System.IO.Directory.CreateDirectory(BasePath + StylesPath);
            System.IO.Directory.CreateDirectory(BasePath + ScriptsPath);
            System.IO.Directory.CreateDirectory(BasePath + AssetsPath);
            System.IO.Directory.CreateDirectory(BasePath + IncludesPath);
            if (DestinationIsRelative)
            {
                System.IO.Directory.CreateDirectory(BasePath + DestinationPath);
            }
            else
            {
                System.IO.Directory.CreateDirectory(DestinationPath);
            }
        }

        public LocalProject(string basePath, string destinationPath, IFileAccess access)
        {
            BasePath = basePath;
            DestinationPath = destinationPath;
            StylesPath = @"\_stylesheet";
            IncludesPath = @"\_includes";
            ScriptsPath = @"\_scripts";
            AssetsPath = @"\_assets";

            DestinationIsRelative = false;
            PutInSubdirectories = false;

            files = new List<File>();
            var f = access.GetFiles(basePath);
            foreach (var file in f)
            {
                File projectFile = new File(file.Remove(0, BasePath.Length));
                files.Add(projectFile);
            }
        }

        public IEnumerable<File> GetFiles()
        {
            return files.Where(o => !o.Path.StartsWith(IncludesPath) && !o.Path.StartsWith(DestinationPath));
        }

        public void PrepareBuild()
        {
            var destinationBase = DestinationIsRelative ? BasePath + DestinationPath : DestinationPath;

            System.IO.Directory.CreateDirectory(destinationBase);
            System.IO.Directory.CreateDirectory(destinationBase + StylesPath);
            System.IO.Directory.CreateDirectory(destinationBase + ScriptsPath);
            System.IO.Directory.CreateDirectory(destinationBase + AssetsPath);
        }

        // Todo: This probably belongs into ProjectBuilder.cs
        public string GetDestinationPathOfFile(File file)
        {
            var sb = new StringBuilder();
            sb.Append(DestinationIsRelative ? BasePath + DestinationPath : DestinationPath);
            if (file.FileType != FileType.Document)
            {
                sb.Append(GetBaseDirectoryFromFileType(file.FileType));
            }

            var dirName = System.IO.Path.GetDirectoryName(file.Path);
            if (!string.IsNullOrEmpty(dirName))
            {
                sb.Append(@"\");
                sb.Append(System.IO.Path.GetDirectoryName(file.Path));
            }

            if (PutInSubdirectories && file.FileType == FileType.Document)
            {
                sb.Append(@"\");
                sb.Append(System.IO.Path.GetFileNameWithoutExtension(file.Path));
                sb.Append(@"\");
                sb.Append("index.html");
            }
            else
            {
                sb.Append(@"\");
                sb.Append(System.IO.Path.GetFileName(file.Path));
            }
            return sb.ToString();
        }

        private string GetBaseDirectoryFromFileType(FileType type)
        {
            switch (type)
            {
                case FileType.Asset:
                    return AssetsPath;

                case FileType.Script:
                    return ScriptsPath;

                case FileType.Stylesheet:
                    return StylesPath;

                default:
                    return string.Empty; // TODO: DO I want to throw here?
            }
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

        public bool HasFile(string relativePath, IFileAccess access, Scope scope = Scope.Root)
        {
            if (!relativePath.StartsWith(@"\"))
                relativePath = @"\" + relativePath;
            return access.FileExists(BasePath + GetDirectoryFromScope(scope) + relativePath);
        }

        public string GetFilePath(File file)
        {
            return BasePath + file.Path;
        }

        public string GetFilePath(string relativePath, Scope scope = Scope.Root)
        {
            if (!relativePath.StartsWith(@"\"))
                relativePath = @"\" + relativePath;
            return BasePath + GetDirectoryFromScope(scope) + relativePath;
        }

        public string GetDestinationPathOfFile(string relativePath, Scope scope = Scope.Root)
        {
            // TODO:
            throw new NotImplementedException();
        }

        public Scope GetScope(string value)
        {
            if (Enum.TryParse(value, true, out Scope enumValue))
            {
                return enumValue;
            }
            else
            {
                return Scope.Root;
            }
        }

        public bool IsPrepared(IFileAccess access)
        {
            var destinationBase = DestinationIsRelative ? BasePath + DestinationPath : DestinationPath;
            return access.DirectoryExists(
                destinationBase,
                destinationBase + StylesPath,
                destinationBase + ScriptsPath,
                destinationBase + AssetsPath);
        }
    }
}