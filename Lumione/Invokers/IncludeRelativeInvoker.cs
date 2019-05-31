using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Lumione.Invokers
{
    /// <summary>
    /// The IncludeRelativeInvoker<see cref="IncludeRelativeInvoker"/> works similar to the IncludeInvoker <see cref="IncludeInvoker"/>,
    /// but instead of looking up the "_includes" directory, it looks up files relative to the current file path.
    /// Warning: Include paths containing "." or ".." are not processed.
    /// </summary>
    public class IncludeRelativeInvoker : IInvoke
    {
        /*
         * This pattern works on the following inputs:
         * {% include test.html %}
         * {% include dir\test.html %}
         * {% include some\dir\test.html %}
         * {% include \some\dir\test.html %}
         */
        private const string pattern = @"{%\s*include_relative\s+(?<filePath>(?:\\?(?:\w+\\?)+\.\w+))\s*%}";

        public IncludeRelativeInvoker(SettingsManager settingsManager) : base(settingsManager) { }
        public override string Invoke(string commentIn, string filePath)
        {
            commentIn = commentIn.Replace("/", @"\");
            if (Regex.IsMatch(commentIn, pattern))
            {
                var match = Regex.Match(commentIn, pattern);
                var includePath = match.Groups["filePath"].Value;

                return FindFileContents(filePath, includePath);
            }
            return string.Empty;
        }

        private string FindFileContents(string filePath, string includePath)
        {
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
            return string.Empty;
        }
    }
}
