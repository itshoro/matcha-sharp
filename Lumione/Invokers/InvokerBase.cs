using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lumione.Invokers
{
    public abstract class InvokerBase : IInvoker
    {
        protected ICollection<FileType> targets;
        protected string pattern;

        protected InvokerBase()
        {
            targets = new List<FileType>();
        }

        public virtual bool CanInvoke(FileType type)
        {
            return targets.Contains(type);
        }

        protected void AddFileTypeTarget(FileType target)
        {
            targets.Add(target);
        }

        public abstract string Invoke(IProject project, IFileAccess access, string contents);
        public abstract Task<string> InvokeAsync(IProject project, IFileAccess access, string command);

        public bool CanInvoke(string command)
        {
            return Regex.IsMatch(command, pattern);
        }
    }
}