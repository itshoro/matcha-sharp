using System.Text.RegularExpressions;

namespace Lumione.Invokers
{
    /// <summary>
    /// The IncludeRelativeInvoker<see cref="IncludeRelativeInvoker"/> works similar to the IncludeInvoker <see cref="IncludeInvoker"/>,
    /// but instead of looking up the "_includes" directory, it looks up files relative to the current file path.
    /// Warning: Include paths containing "." or ".." are not processed.
    /// </summary>
    public class IncludeRelativeInvoker : InvokerBase
    {
        public IncludeRelativeInvoker(Settings settings) : base(settings)
        {
            /*
             * This pattern works on the following inputs:
             * {% include_relative test.html %}
             * {% include_relative dir\test.html %}
             * {% include_relative some\dir\test.html %}
             * {% include_relative \some\dir\test.html %}
             */
            pattern = @"include_relative\s+(?<filePath>(?:\\?(?:\w+\\?)+\.\w+)";
        }

        public override string Invoke(string command)
        {
            command = command.Replace("/", @"\");
            if (Regex.IsMatch(command, pattern))
            {
                var match = Regex.Match(command, pattern);
                var includePath = match.Groups["filePath"].Value;

                return FindFileContents(Context.CurrentFilePath, includePath);
            }
            return string.Empty;
        }

        protected string FindFileContents(string filePath, string includePath)
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