using Lumione;
using System.Collections.Generic;

namespace Lumione.Invokers
{
    internal interface IInvoker
    {
        string Invoke(IProject project, string command);

        bool CanInvoke(FileType type);

        bool CanInvoke(string command);
    }
}