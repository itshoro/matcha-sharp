using System;
using System.Collections.Generic;
using System.Text;

namespace Lumione
{
    internal enum Scope
    {
        Root,
        Include
    }

    internal interface IProject
    {
        void PrepareBuild();

        IEnumerable<File> GetFiles();

        void AddToDestination(File file, string contents);

        string GetFileContents(File file);

        string GetFileContents(string relativePath, Scope scope = Scope.Root);

        bool HasFile(string value, Scope scope = Scope.Root);
    }
}