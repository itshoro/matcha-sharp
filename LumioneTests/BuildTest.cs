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

        public bool DirectoryExists(params string[] dirs)
        {
            return true;
        }

        public bool FileExists(params string[] paths)
        {
            foreach (var path in paths)
            {
                if (!fileSystem.Keys.Select(o => o.Path).Contains(path))
                    return false;
            }
            return true;
        }

        public IEnumerable<string> GetFiles(string path, Settings settings)
        {
            return fileSystem.Keys.Select(o => o.Path);
        }

        public IEnumerable<string> GetFiles(params string[] path)
        {
            throw new NotImplementedException();
        }

        public string ReadFromRoot(Project project, Settings settings, string path)
        {
            return fileSystem[fileSystem.Keys.First(o => o.Path.Remove(0, project.Directory.Length) == path)];
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

    [TestClass]
    public class BuildTest
    {
        private List<string> files;
        private Dictionary<TestFile, string> fileSystem;

        private IEnumerable<InvokerBase> invokers;
        private ProjectBuilder builder;
        private List<string> reservedDirectories;
        private Settings settings;

        [TestInitialize]
        public void Prepare()
        {
            files = new List<string>();
            fileSystem = new Dictionary<TestFile, string>();

            invokers = ReflectiveEnumerator.GetEnumerableOfType<InvokerBase>();
            builder = new ProjectBuilder();
            reservedDirectories = new List<string>();
            foreach (var invoker in invokers)
            {
                reservedDirectories.AddRange(invoker.ReservedDirectories());
            }

            settings = new Settings()
            {
                CreateSubdirectories = true
            };
        }

        [TestMethod]
        public void TestBuildDocumentIsBeingBuiltInRootDirectory()
        {
            files.Add(System.IO.Path.Combine(Environment.CurrentDirectory, "index.html"));
            fileSystem.Add(new TestFile { Path = System.IO.Path.Combine(Environment.CurrentDirectory, "index.html"), IsInDestination = false }, "Hello World!");

            var fileAccess = new DictionaryFileAccess(fileSystem);
            var project = new Project(Environment.CurrentDirectory, fileAccess, settings);

            builder.Build(project, settings, reservedDirectories, invokers, fileAccess);

            fileAccess.fileSystem.Keys.Where(f => f.IsInDestination).Count().Should().Be(1);
            fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(f => f.IsInDestination)].Should().Be("Hello World!");
            fileAccess.fileSystem.Keys.First(f => f.IsInDestination).Path.Should().Be(System.IO.Path.Combine(Environment.CurrentDirectory, "build", "index.html"));
        }

        //[TestMethod]
        //public void TestBuildDocumentIsBeingBuiltInSubDirectory()
        //{
        //    files.Add(@"root\test\index.html");
        //    fileSystem.Add(new TestFile { Path = @"root\test\index.html", IsInDestination = false }, "Hello World!");

        //    var fileAccess = new DictionaryFileAccess(fileSystem);
        //    var project = new Project(string.Empty, fileAccess, settings);

        //    builder.Build(project, settings, reservedDirectories, invokers, fileAccess);

        //    fileAccess.fileSystem.Keys.Where(f => f.IsInDestination).Count().Should().Be(1);
        //    fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(f => f.IsInDestination)].Should().Be("Hello World!");
        //    fileAccess.fileSystem.Keys.First(f => f.IsInDestination).Path.Should().Be(@"root\deploy\test\index.html");
        //}

        //[TestMethod]
        //public void TestBuildStylesheetIsBeingBuiltInSubDirectory()
        //{
        //    files.Add(@"root\style.css");
        //    fileSystem.Add(new TestFile { Path = @"root\style.css", IsInDestination = false }, "Hello World!");

        //    var fileAccess = new DictionaryFileAccess(fileSystem);
        //    var project = new Project(string.Empty, fileAccess, settings);

        //    builder.Build(project, settings, reservedDirectories, invokers, fileAccess);

        //    fileAccess.fileSystem.Keys.Where(f => f.IsInDestination).Count().Should().Be(1);
        //    fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(f => f.IsInDestination)].Should().Be("Hello World!");
        //    fileAccess.fileSystem.Keys.First(f => f.IsInDestination).Path.Should().Be(@"root\deploy\css\style.css");
        //}

        //[TestMethod]
        //public void TestBuildStylesheetIsBeingBuiltInRootDirectory()
        //{
        //    files.Add(@"root\test\index.html");
        //    fileSystem.Add(new TestFile { Path = @"root\test\index.html", IsInDestination = false }, "Hello World!");

        //    var fileAccess = new DictionaryFileAccess(fileSystem);
        //    var project = new Project(string.Empty, fileAccess, settings);

        //    builder.Build(project, settings, reservedDirectories, invokers, fileAccess);

        //    fileAccess.fileSystem.Keys.Where(f => f.IsInDestination).Count().Should().Be(1);
        //    fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(f => f.IsInDestination)].Should().Be("Hello World!");
        //    fileAccess.fileSystem.Keys.First(f => f.IsInDestination).Path.Should().Be(@"root\deploy\test\index.html");
        //}

        //[TestMethod]
        //public void TestBuildSingleFileInRootDirectory()
        //{
        //    files.Add(@"root\index.html");
        //    fileSystem.Add(new TestFile { Path = @"root\index.html", IsInDestination = false }, "Hello World!");

        //    var fileAccess = new DictionaryFileAccess(fileSystem);
        //    var project = new Project(string.Empty, fileAccess, settings);

        //    builder.Build(project, settings, reservedDirectories, invokers, fileAccess);

        //    fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).Count().Should().Be(1);
        //    fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(o => o.IsInDestination)].Should().Be("Hello World!");
        //    fileAccess.fileSystem.Keys.First(o => o.IsInDestination).Path.Should().Be(@"\" + settings.DestinationFolderName + @"\index.html");
        //}

        //[TestMethod]
        //public void TestBuildSingleFileInSubDirectory()
        //{
        //    files.Add(@"test\index.html");
        //    fileSystem.Add(new TestFile { Path = @"root\test\index.html", IsInDestination = false }, "Hello World!");
        //    var fileAccess = new DictionaryFileAccess(fileSystem);

        //    var project = new Project(string.Empty, fileAccess, settings);

        //    builder.Build(project, settings, reservedDirectories, invokers, fileAccess);

        //    fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).Count().Should().Be(1);
        //    fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(o => o.IsInDestination)].Should().Be("Hello World!");
        //    fileAccess.fileSystem.Keys.First(o => o.IsInDestination).Path.Should().Be(@"\" + settings.DestinationFolderName + @"\test\index.html");
        //}

        //[TestMethod]
        //public void TestBuildFileCanBePutInSubDirectories()
        //{
        //    var fileAccess = new DictionaryFileAccess(fileSystem);
        //    files.Add(@"root\test.html");
        //    fileSystem.Add(new TestFile { Path = @"root\test.html", IsInDestination = false }, "Hello World!");
        //    var project = new Project(string.Empty, fileAccess, settings);

        //    builder.Build(project, settings, reservedDirectories, invokers, fileAccess);

        //    Uri uri = new Uri(string.Empty, UriKind.Relative);

        //    fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).Count().Should().Be(1);
        //    fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(o => o.IsInDestination)].Should().Be("Hello World!");
        //    fileAccess.fileSystem.Keys.First(o => o.IsInDestination).Path.Should().Be(@"\" + settings.DestinationFolderName + @"\test\index.html");
        //}

        //[TestMethod]
        //public void TestBuildMultipleFilesInRootDirectory()
        //{
        //    var fileAccess = new DictionaryFileAccess(fileSystem);
        //    files.Add(@"root\index.html");
        //    files.Add(@"root\index2.html");
        //    files.Add(@"root\index3.html");
        //    fileSystem.Add(new TestFile { Path = @"root\index.html", IsInDestination = false }, "This is a file.");
        //    fileSystem.Add(new TestFile { Path = @"root\index2.html", IsInDestination = false }, "This is another file.");
        //    fileSystem.Add(new TestFile { Path = @"root\index3.html", IsInDestination = false }, "This is the last file.");
        //    var project = new Project(string.Empty, fileAccess, settings);

        //    builder.Build(project, settings, reservedDirectories, invokers, fileAccess);

        //    fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).Count().Should().Be(3);
        //    fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(o => o.IsInDestination)].Should().Be("This is a file.");
        //    fileAccess.fileSystem[fileAccess.fileSystem.Keys.Last(o => o.IsInDestination)].Should().Be("This is the last file.");

        //    var relevantFilesFromFilesystem = fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).ToList().Select(o => o.Path);
        //    files = files.Select(o => @"\" + settings.DestinationFolderName + @"\" + Path.GetFileNameWithoutExtension(o) + @"\index.html").ToList();
        //    files[0] = @"root\deploy\index.html";
        //    relevantFilesFromFilesystem.Should().BeEquivalentTo(files);
        //}

        //[TestMethod]
        //public void TestFilesInAReservedFolderAreNotBeingMovedToTheDeployFolder()
        //{
        //    var fileAccess = new DictionaryFileAccess(fileSystem);
        //    files.Add(@"root\includes\test.html");

        //    fileSystem.Add(new TestFile { Path = @"root\includes\test.html", IsInDestination = false }, "This is the index file.");
        //    var project = new Project(string.Empty, fileAccess, settings);

        //    builder.Build(project, settings, reservedDirectories, invokers, fileAccess);

        //    fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).Count().Should().Be(0);
        //}
    }
}