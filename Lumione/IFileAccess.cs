using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lumione
{
    public interface IFileAccess
    {
        string Read(string path);

        void Write(string path, string contents);
        Task WriteAsync(string path, string contents);
        Task<string> ReadAsync(string path);
        
    }
}