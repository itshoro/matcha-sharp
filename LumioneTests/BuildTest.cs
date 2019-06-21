using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lumione;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lumione.Invokers;
using FluentAssertions;
using System.Text;

namespace LumioneTests
{
    public struct TestFile
    {
        public string Path { get; set; }
        public bool IsInDestination { get; set; }
    };
    internal class DictionaryFileAccess : IFileAccess
    {
        public IDictionary<TestFile, string> fileSystem;

        public DictionaryFileAccess(IDictionary<TestFile, string> fileSystem)
        {
            this.fileSystem = fileSystem;
        }
        public string Read(string path)
        {
            return fileSystem[fileSystem.Keys.First(o => o.Path == path)];
        }

        public Task<string> ReadAsync(string path)
        {
            throw new NotImplementedException();
        }

        public void Write(string path, string contents)
        {
            fileSystem.Add(new TestFile { Path = path, IsInDestination = true }, contents);
        }

        public Task WriteAsync(string path, string contents)
        {
            throw new NotImplementedException();
        }
    }
    internal class DictionaryProject : IProject
    {
        public string BasePath { get; set; }
        public string StylesPath { get; private set; }
        public string IncludesPath { get; private set; }
        public string ScriptsPath { get; private set; }
        public string AssetsPath { get; private set; }
        public string DestinationPath { get; set; }
        public bool destinationIsRelative;
        public bool putInSubdirectories;
        private ICollection<Lumione.File> files;

        public DictionaryProject()
        {
            files = new List<Lumione.File>();
            BasePath = string.Empty;
            DestinationPath = @"\_build";
            StylesPath = @"\_stylesheet";
            IncludesPath = @"\_includes";
            ScriptsPath = @"\_scripts";
            AssetsPath = @"\_assets";
        }

        public DictionaryProject(ICollection<Lumione.File> files)
        {
            this.files = files;
            BasePath = string.Empty;
            DestinationPath = @"\_build";
            StylesPath = @"\_stylesheet";
            IncludesPath = @"\_includes";
            ScriptsPath = @"\_scripts";
            AssetsPath = @"\_assets";
        }

        private string GetBaseDirectoryFromFileType (FileType type) 
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

        public string GetDestinationPathOfFile(Lumione.File file)
        {
            var sb = new StringBuilder();
            sb.Append(destinationIsRelative ? BasePath + DestinationPath : DestinationPath);
            if (file.FileType != FileType.Document)
            {
                sb.Append(GetBaseDirectoryFromFileType(file.FileType));
            }

            var dirName = System.IO.Path.GetDirectoryName(file.Path);
            if (!string.IsNullOrEmpty(dirName)) {
                sb.Append(@"\");
                sb.Append(System.IO.Path.GetDirectoryName(file.Path));

            }

            if (putInSubdirectories && file.FileType == FileType.Document)
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

        public string GetFilePath(Lumione.File file)
        {
            return file.Path;
        }

        public string GetFilePath(string relativePath, Scope scope = Scope.Root)
        {
            return relativePath;
        }

        public IEnumerable<Lumione.File> GetFiles()
        {
            return files;
        }

        public bool HasFile(string path, Scope scope = Scope.Root)
        {
            return files.Select(o => o.Path).Contains(path);
        }

        public void PrepareBuild()
        {
            // Do Nothing
        }
    }
    [TestClass]
    public class BuildTest
    {
        private ProjectBuilder builder;
        private IEnumerable<IInvoker> invokers;

        [TestMethod]
        public void TestBuildSingleFileInRootDirectory()
        {
            builder = new ProjectBuilder();
            var files = new List<Lumione.File>();
            var filesystem = new Dictionary<TestFile, string>();

            files.Add(new Lumione.File("index.html"));
            filesystem.Add(new TestFile{ Path = "index.html", IsInDestination = false }, "Hello World!");

            var project = new DictionaryProject(files);
            var fileAccess = new DictionaryFileAccess(filesystem);
            invokers = ReflectiveEnumerator.GetEnumerableOfType<InvokerBase>();

            builder.Build(project, invokers, fileAccess);

            fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).Count().Should().Be(1);
            fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(o => o.IsInDestination)].Should().Be("Hello World!");
            fileAccess.fileSystem.Keys.First(o => o.IsInDestination).Path.Should().Be(project.DestinationPath + @"\index.html");

        }

        [TestMethod]
        public void TestBuildSingleFileInSubDirectory()
        {
            builder = new ProjectBuilder();
            var files = new List<Lumione.File>();
            var filesystem = new Dictionary<TestFile, string>();

            files.Add(new Lumione.File(@"test\index.html"));
            filesystem.Add(new TestFile{ Path = @"test\index.html", IsInDestination = false }, "Hello World!");

            var project = new DictionaryProject(files);
            var fileAccess = new DictionaryFileAccess(filesystem);
            invokers = ReflectiveEnumerator.GetEnumerableOfType<InvokerBase>();

            builder.Build(project, invokers, fileAccess);

            fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).Count().Should().Be(1);
            fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(o => o.IsInDestination)].Should().Be("Hello World!");
            fileAccess.fileSystem.Keys.First(o => o.IsInDestination).Path.Should().Be(project.DestinationPath + @"\test\index.html");
        }

        [TestMethod]
        public void TestBuildFileCanBePutInSubDirectories()
        {
            builder = new ProjectBuilder();
            var files = new List<Lumione.File>();
            var filesystem = new Dictionary<TestFile, string>();

            files.Add(new Lumione.File(@"test.html"));
            filesystem.Add(new TestFile{ Path = @"test.html", IsInDestination = false }, "Hello World!");

            var project = new DictionaryProject(files);
            var fileAccess = new DictionaryFileAccess(filesystem);
            invokers = ReflectiveEnumerator.GetEnumerableOfType<InvokerBase>();

            project.putInSubdirectories = true;

            builder.Build(project, invokers, fileAccess);

            fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).Count().Should().Be(1);
            fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(o => o.IsInDestination)].Should().Be("Hello World!");
            fileAccess.fileSystem.Keys.First(o => o.IsInDestination).Path.Should().Be(project.DestinationPath + @"\test\index.html");
        }

        [TestMethod]
        public void TestBuildMultipleFilesInRootDirectory()
        {
            builder = new ProjectBuilder();
            var files = new List<Lumione.File>();
            var filesystem = new Dictionary<TestFile, string>();

            files.Add(new Lumione.File(@"index.html"));
            files.Add(new Lumione.File(@"index2.html"));
            files.Add(new Lumione.File(@"index3.html"));
            filesystem.Add(new TestFile{ Path = @"index.html", IsInDestination = false }, "This is a file.");
            filesystem.Add(new TestFile{ Path = @"index2.html", IsInDestination = false }, "This is another file.");
            filesystem.Add(new TestFile{ Path = @"index3.html", IsInDestination = false }, "This is the last file.");

            var project = new DictionaryProject(files);
            var fileAccess = new DictionaryFileAccess(filesystem);
            invokers = ReflectiveEnumerator.GetEnumerableOfType<InvokerBase>();

            builder.Build(project, invokers, fileAccess);

            fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).Count().Should().Be(3);
            fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(o => o.IsInDestination)].Should().Be("This is a file.");
            fileAccess.fileSystem[fileAccess.fileSystem.Keys.Last(o => o.IsInDestination)].Should().Be("This is the last file.");
            
            // var expected = files.ForEach(o => (project.DestinationPath + @"\" + o));

            var relevantFilesFromFilesystem = fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).ToList();
            relevantFilesFromFilesystem.Should().BeEquivalentTo(files);
     
        }
    }
}