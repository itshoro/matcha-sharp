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
            pattern = @"include\s+(?<filePath>(?:\\?(?:\w+\\?)+\.\w+))";
        }

        public override string Invoke(IProject project, IFileAccess access, string command)
        {
            var match = Regex.Match(command, pattern);

            if (match.Success && project.HasFile(match.Groups["filePath"].Value, Scope.Include))
            {
                return access.Read(project.GetFilePath(match.Groups["filePath"].Value, Scope.Include));
            }
            throw new ArgumentException("File not found.");
        }

        public override async Task<string> InvokeAsync(IProject project, IFileAccess access, string command)
        {
            var match = Regex.Match(command, pattern);
            if (match.Success && project.HasFile(match.Groups["filePath"].Value, Scope.Include))
            {
                return await access.ReadAsync(project.GetFilePath(match.Groups["filePath"].Value, Scope.Include));
            }
            throw new ArgumentException("File not found.");
        }
    }
    }