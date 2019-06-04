using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lumione.Invokers
{
    internal class IncludeInvoker : InvokerBase
    {
        public IncludeInvoker() : base()
        {
            targets.Add(FileType.Document);
            pattern = @"include\s+(?<filePath>(?:\\?(?:\w+\\?)+\.\w+))";
        }

        public override string Invoke(IProject project, string command)
        {
            var match = Regex.Match(command, pattern);

            if (match.Success && project.HasFile(match.Groups["filePath"].Value, Scope.Include))
            {
                return project.GetFileContents(match.Groups["filePath"].Value, Scope.Include);
            }
            throw new ArgumentException("File not found.");
        }
    }
}