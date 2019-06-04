using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lumione;
using Lumione.Service;
using System;
using System.IO;
using System.Linq;

namespace LumioneTests
{
    [TestClass]
    public class BuildTest
    {
        private Settings testSettings;
        private BuildService buildService;
        private CleanUpService cleanUpService;

        private string testPath;

        [TestInitialize]
        public void Init()
        {
            testPath = Environment.CurrentDirectory + @"\test";
            testSettings = Settings.Default(testPath);
            cleanUpService = new CleanUpService(testSettings);
            buildService = new BuildService(testSettings);
        }

        [TestCleanup]
        public void CleanUp()
        {
            cleanUpService.Add(testPath);
            cleanUpService.Execute();
        }

        [TestMethod]
        public void TestIncludeInvoker()
        {
            var inclusionContent = "World";
            var mainContent = "Hello {% include inclusion.html %}!";

            File.WriteAllText(testPath + @"\index.html", mainContent);
            File.WriteAllText(testSettings.IncludePath + @"\inclusion.html", inclusionContent);

            buildService.Execute();

            var amountOfBuiltDocuments = Directory.EnumerateFiles(testSettings.DestinationPath, "*", SearchOption.AllDirectories).ToList().Count;
            var content = File.ReadAllText(testSettings.DestinationPath + @"\index.html");

            Assert.IsTrue(amountOfBuiltDocuments == 1 && content == "Hello World!");
        }

        [TestMethod]
        public void TestIncludeRelative()
        {
            Assert.Fail();
        }

        [TestMethod]
        public void TestFilesInFoldersAreBeingBuiltAsWell()
        {
            Assert.Fail();
        }
    }
}