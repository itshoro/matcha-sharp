using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lumione.Invokers
{
    /// <summary>
    /// The IncludeInvoker<see cref="IncludeInvoker"/> replaces include statements by inlining the coresponding content from the "_includes" directory.
    /// This is done by looking up the file path from the given include statement within the directory.
    /// Warning: Include paths containing "." or ".." are not processed.
    /// </summary>
    public class IncludeInvoker : IInvoke
    {
        /*
         * This pattern works on the following inputs:
         * {% include test.html %}
         * {% include dir\test.html %}
         * {% include some\dir\test.html %}
         * {% include \some\dir\test.html %}
         */
        private const string pattern = @"{%\s*include\s+(?<filePath>(?:\\?(?:\w+\\?)+\.\w+))\s*%}";
        
        private readonly string includeDirectory;

        public IncludeInvoker(SettingsManager settingsManager) : base(settingsManager)
        {
            includeDirectory = System.IO.Path.Combine(settingsManager.BasePath, "\\_includes");
        }
        public override string Invoke(string commentIn, string filePath)
        {
            commentIn = commentIn.Replace("/", @"\");
            if (Regex.IsMatch(commentIn, pattern))
            {
                var match = Regex.Match(commentIn, pattern);
                var includePath = match.Groups["filePath"].Value;

                return FindFileContents(includePath);
            }
            return String.Empty;
        }

        private string FindFileContents(string includePath)
        {
            if (!includePath.StartsWith("\\"))
            {
                includePath = "\\" + includePath;
            }

            var combined = System.IO.Path.Combine(includeDirectory + includePath);
            var fullPath = System.IO.Path.GetFullPath(combined);
            return System.IO.File.Exists(fullPath) ? System.IO.File.ReadAllText(fullPath) : String.Empty;
        }
    }
}
