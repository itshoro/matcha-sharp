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

        public IEnumerable<string> GetFiles(string path)
        {
            return fileSystem.Keys.Select(o => o.Path);
        }

        public IEnumerable<string> GetFiles(params string[] path)
        {
            throw new NotImplementedException();
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

    [TestClass]
    public class BuildTest
    {
        private ProjectBuilder builder;
        private IEnumerable<IInvoker> invokers;

        [TestMethod]
        public void TestBuildSingleFileInRootDirectory()
        {
            builder = new ProjectBuilder();
            var files = new List<string>();
            var filesystem = new Dictionary<TestFile, string>();

            files.Add("index.html");
            filesystem.Add(new TestFile { Path = "index.html", IsInDestination = false }, "Hello World!");

            var fileAccess = new DictionaryFileAccess(filesystem);
            var project = new LocalProject(string.Empty, fileAccess);
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
            var files = new List<string>();
            var filesystem = new Dictionary<TestFile, string>();

            files.Add(@"test\index.html");
            filesystem.Add(new TestFile { Path = @"test\index.html", IsInDestination = false }, "Hello World!");

            var fileAccess = new DictionaryFileAccess(filesystem);
            var project = new LocalProject(string.Empty, fileAccess);
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
            var files = new List<string>();
            var filesystem = new Dictionary<TestFile, string>();

            files.Add(@"test.html");
            filesystem.Add(new TestFile { Path = @"test.html", IsInDestination = false }, "Hello World!");

            var fileAccess = new DictionaryFileAccess(filesystem);
            var project = new LocalProject(string.Empty, fileAccess);
            invokers = ReflectiveEnumerator.GetEnumerableOfType<InvokerBase>();

            project.PutInSubdirectories = true;

            builder.Build(project, invokers, fileAccess);

            fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).Count().Should().Be(1);
            fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(o => o.IsInDestination)].Should().Be("Hello World!");
            fileAccess.fileSystem.Keys.First(o => o.IsInDestination).Path.Should().Be(project.DestinationPath + @"\test\index.html");
        }

        [TestMethod]
        public void TestBuildMultipleFilesInRootDirectory()
        {
            builder = new ProjectBuilder();
            var files = new List<string>();
            var filesystem = new Dictionary<TestFile, string>();

            files.Add(@"index.html");
            files.Add(@"index2.html");
            files.Add(@"index3.html");
            filesystem.Add(new TestFile { Path = @"index.html", IsInDestination = false }, "This is a file.");
            filesystem.Add(new TestFile { Path = @"index2.html", IsInDestination = false }, "This is another file.");
            filesystem.Add(new TestFile { Path = @"index3.html", IsInDestination = false }, "This is the last file.");

            var fileAccess = new DictionaryFileAccess(filesystem);
            var project = new LocalProject(string.Empty, fileAccess);
            invokers = ReflectiveEnumerator.GetEnumerableOfType<InvokerBase>();

            builder.Build(project, invokers, fileAccess);

            fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).Count().Should().Be(3);
            fileAccess.fileSystem[fileAccess.fileSystem.Keys.First(o => o.IsInDestination)].Should().Be("This is a file.");
            fileAccess.fileSystem[fileAccess.fileSystem.Keys.Last(o => o.IsInDestination)].Should().Be("This is the last file.");

            var relevantFilesFromFilesystem = fileAccess.fileSystem.Keys.Where(o => o.IsInDestination).ToList().Select(o => o.Path);
            files = files.Select(o => @"\_build\" + o).ToList();
            relevantFilesFromFilesystem.Should().BeEquivalentTo(files);
        }
    }
}