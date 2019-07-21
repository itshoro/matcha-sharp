using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lumione
{
    public enum Scope
    {
        Root,
        Include
    }

    public interface IProject
    {
        void PrepareBuild();

        bool IsPrepared(IFileAccess access);

        IEnumerable<File> GetFiles();

        string GetDestinationPathOfFile(string relativePath, Scope scope = Scope.Root);

        string GetDestinationPathOfFile(File file);

        string GetFilePath(File file);

        string GetFilePath(string relativePath, Scope scope = Scope.Root);

        bool HasFile(string value, IFileAccess access, Scope scope = Scope.Root);

        Scope GetScope(string value);
    }
}