using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lumione.Invokers
{
    public class IncludeInvoker : IInvoke
    {
        const string pattern = @"\s*_include_\s+(?<filePath>(?:\.{0,2}|\w+:)(?:\\?\w+)+\.\w+)\s*";
        public override string Invoke(string commentIn, string filePath)
        {
            commentIn = commentIn.Replace("/", @"\");
            if (Regex.IsMatch(commentIn, pattern))
            {
                var match = Regex.Match(commentIn, pattern);
                var includePath = match.Groups["filePath"].Value;

                if (!includePath.StartsWith("\\"))
                {
                    includePath = "\\" + includePath;
                }

                var combined = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath) + includePath);
                var fullPath = System.IO.Path.GetFullPath(combined);
                if (System.IO.File.Exists(fullPath))
                {
                    return System.IO.File.ReadAllText(fullPath);
                }
            }
            return "";
        }
    }
}
