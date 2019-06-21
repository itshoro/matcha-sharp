using Lumione;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lumione.Invokers
{
    public interface IInvoker
    {
        string Invoke(IProject project, IFileAccess access, string command);
        Task<string> InvokeAsync(IProject project, IFileAccess access, string command);

        bool CanInvoke(FileType type);

        bool CanInvoke(string command);
    }
}