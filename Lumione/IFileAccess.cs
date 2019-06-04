using System;
using System.Collections.Generic;
using System.Text;

namespace Lumione
{
    public interface IFileAccess
    {
        string Read(string path);

        void Write(string path, string contents);
    }
}