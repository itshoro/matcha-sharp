using Lumione;
using Lumione.Invokers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace LumioneTests
{
    [TestClass]
    internal class InvokerTest
    {
        [TestMethod]
        public void TestIncludeInvokerCanInvokeOnRightSyntax()
        {
            var include = new IncludeInvoker();
            Assert.IsTrue(include.CanInvoke("{% include test.html %}"));
        }

        [TestMethod]
        public void TestIncludeInvokerCannnotInvokeOnWrongSyntax()
        {
            var include = new IncludeInvoker();
            Assert.IsFalse(include.CanInvoke("{% include nothing %}"));
        }

        [TestMethod]
        public void TestIncludeInvokerThrowsErrorWhenFileNotFound()
        {
            // TODO
        }

        [TestMethod]
        public void TestIncludeInvokerCanAddFromTopLevelIncludeDirectory()
        {
            var include = new IncludeInvoker();
            include.Invoke("{% include test.html %}", Settings.Default(Environment.CurrentDirectory));
        }
    }
}