using System.Text.RegularExpressions;

namespace Lumione.Invokers
{
    /// <summary>
    /// The IncludeInvoker<see cref="IncludeInvoker"/> replaces include statements by inlining the coresponding content from the "_includes" directory.
    /// This is done by looking up the file path from the given include statement within the directory.
    /// Warning: Include paths containing "." or ".." are not processed.
    /// </summary>
    public class IncludeInvoker : InvokerBase
    {
        public IncludeInvoker(Settings settings) : base(settings)
        {
            /*
             * This pattern works on the following inputs:
             * {% include test.html %}
             * {% include dir\test.html %}
             * {% include some\dir\test.html %}
             * {% include \some\dir\test.html %}
             */
            pattern = @"include\s+(?<filePath>(?:\\?(?:\w+\\?)+\.\w+))";
        }

        public override string Invoke(string commentIn)
        {
            commentIn = commentIn.Replace("/", @"\");
            if (Regex.IsMatch(commentIn, pattern))
            {
                var match = Regex.Match(commentIn, pattern);
                var includePath = match.Groups["filePath"].Value;

                return FindFileContents(includePath);
            }
            return string.Empty;
        }

        private string FindFileContents(string includePath)
        {
            if (!includePath.StartsWith("\\"))
            {
                includePath = "\\" + includePath;
            }

            var combined = System.IO.Path.Combine(settings.IncludePath + includePath);
            var fullPath = System.IO.Path.GetFullPath(combined);
            return System.IO.File.Exists(fullPath) ? System.IO.File.ReadAllText(fullPath) : string.Empty;
        }
    }
}