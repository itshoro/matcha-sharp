using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lumione.Invokers
{
    public class LinkInvoker : InvokerBase
    {
        public LinkInvoker()
        {
            pattern = @"link\s+(<?filePath>.+)(?\s+(<?scope>.+))*\s*";
        }

        public override string Invoke(IProject project, IFileAccess access, string contents)
        {
            var matches = Regex.Match(contents, pattern);
            if (matches.Success)
            {
                return matches.Groups["scope"].Success ?
                    project.GetDestinationPathOfFile(matches.Groups["filePath"].Value) :
                    project.GetDestinationPathOfFile(matches.Groups["filePath"].Value, project.GetScope(matches.Groups["scope"].Value));
            }
            throw new ArgumentException();
        }

        public override Task<string> InvokeAsync(IProject project, IFileAccess access, string command)
        {
            throw new System.NotImplementedException();
        }
    }
}