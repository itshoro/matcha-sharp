using Lumione.Invokers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lumione
{
    public class Processor
    {
        private const string commandPattern = @"\<\!\-\-\s*\{\%\s*(?<command>.+)\s*\%\}\s*\-\-\>";

        private Settings settings;
        private IEnumerable<string> FilePaths;
        private List<string> SupportedTypes;
        public int Count { get { return FilePaths.Count(); } }

        public Processor(Settings settings)
        {
            this.settings = settings;

            SupportedTypes = new List<string>();
            SupportedTypes.Add("html");
            SupportedTypes.Add("htm");
            SupportedTypes.Add("css");

            FilePaths = AddFiles(settings.SourcePath);

            if (!Directory.Exists(settings.DestinationPath))
            {
                Directory.CreateDirectory(settings.DestinationPath);
            }
        }

        internal void ExecuteCommandsIfPossible()
        {
            foreach (var filePath in FilePaths)
            {
                var fileContents = File.ReadAllText(filePath);
                var matches = Regex.Matches(fileContents, commandPattern);

                var processedContent = string.Empty;

                if (matches.Count > 0)
                {
                    processedContent = InvokeCommandIfSupported(filePath, fileContents, matches, processedContent);
                }

                var relativePath = filePath.Remove(0, settings.SourcePath.Length);
                var newPath = settings.DestinationPath + relativePath;
                if (!Directory.Exists(Path.GetDirectoryName(newPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                }
                File.WriteAllText(newPath, processedContent == string.Empty ? fileContents : processedContent);
            }
        }

        private string InvokeCommandIfSupported(string filePath, string fileContents, MatchCollection matches, string processedContent)
        {
            var positionInFile = 0;
            foreach (Match match in matches)
            {
                foreach (var invoker in ReflectiveEnumerator.GetEnumerableOfType<InvokerBase>(settings))
                {
                    invoker.Context.CurrentFilePath = filePath;
                    try
                    {
                        if (invoker.CanInvoke(match.Groups["command"].Value))
                        {
                            var prepend = fileContents.Substring(positionInFile, match.Index);
                            var invokedContent = invoker.Invoke(match.Groups["command"].Value);
                            positionInFile += match.Index + match.Length;

                            processedContent += prepend + invokedContent;
                            break;
                        }
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine($"Command \"{match.Groups["command"]}\" in file \"{filePath}\" is not supported.");
                    }
                }
            }
            if (positionInFile < fileContents.Length)
            {
                processedContent += fileContents.Substring(positionInFile);
            }

            return processedContent;
        }

        private IEnumerable<string> AddFiles(string path)
        {
            var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
            files = files.Where(s => SupportedTypes.Any(type => type == Path.GetExtension(s).Substring(1).ToLower()));
            return files.Where(file => !settings.IgnoredDirectories.Any(directory => file.StartsWith(directory)));
        }
    }
}