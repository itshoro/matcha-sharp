using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Lumione
{
    public interface IFileAccess
    {
        string ReadFromRoot(Project project, Settings settings, string path);

        void Write(string path, string contents);

        Task WriteAsync(string path, string contents);

        Task<string> ReadAsync(string path);

        IEnumerable<string> GetFiles(string path);

        bool FileExists(params string[] path);

        bool DirectoryExists(params string[] dirs);
    }
}