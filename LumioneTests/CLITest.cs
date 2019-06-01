using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lumione;
using Lumione.Actions;
using System;

namespace LumioneTests
{
    [TestClass]
    public class CLITest
    {
        [TestMethod]
        public void TestInitCommand()
        {
            var settings = new Settings(Environment.CurrentDirectory);
        }

        [TestMethod]
        public void TestBuildCommand()
        {
            var settings = new Settings(Environment.CurrentDirectory);
            var builder = new SiteBuilder(null, settings);
        }
    }
}