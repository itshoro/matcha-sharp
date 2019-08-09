using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lumione.Invokers
{
    internal class IncludeInvoker : InvokerBase
    {
        public IncludeInvoker() : base()
        {
            targets.Add(FileType.Document);
            reserved.Add(@"includes");
            pattern = @"include\s+(?<filePath>(?:\\?(?:\w+\\?)+\.\w+))";
        }

        public override string Invoke(Project project, Settings settings, IFileAccess access, string command)
        {
            var match = Regex.Match(command, pattern);

            if (match.Success && project.HasFile(match.Groups["filePath"].Value))
            {
                return access.ReadFromRoot(project, settings, match.Groups["filePath"].Value);
            }
            throw new ArgumentException("File not found.");
        }

        public override async Task<string> InvokeAsync(Project project, Settings settings, IFileAccess access, string command)
        {
            throw new NotImplementedException();
        }
    }
}