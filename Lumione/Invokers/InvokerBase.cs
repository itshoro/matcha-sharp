using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lumione.Invokers
{
    public abstract class InvokerBase : IInvokable
    {
        protected ICollection<FileType> targets;
        protected ICollection<string> reserved;
        protected string pattern;

        protected InvokerBase()
        {
            targets = new List<FileType>();
            reserved = new List<string>();
        }

        public virtual bool CanInvoke(FileType type)
        {
            return targets.Contains(type);
        }

        protected void AddFileTypeTarget(FileType target)
        {
            targets.Add(target);
        }

        public abstract string Invoke(Project project, Settings settings, IFileAccess access, string contents);

        public abstract Task<string> InvokeAsync(Project project, Settings settings, IFileAccess access, string command);

        public bool CanInvoke(string command)
        {
            return Regex.IsMatch(command, pattern);
        }

        public IEnumerable<string> ReservedDirectories()
        {
            return (reserved as IEnumerable<string>);
        }
    }
}