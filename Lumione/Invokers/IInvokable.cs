using Lumione;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lumione.Invokers
{
    public interface IInvokable
    {
        string Invoke(Project project, Settings settings, IFileAccess access, string command);

        Task<string> InvokeAsync(Project project, Settings settings, IFileAccess access, string command);

        bool CanInvoke(FileType type);

        bool CanInvoke(string command);
    }
}