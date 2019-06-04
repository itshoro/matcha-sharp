using Lumione.Invokers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Lumione
{
    internal class ProjectBuilder
    {
        public void Build(IProject project, IEnumerable<IInvoker> invokers)
        {
            project.PrepareBuild();

            var projectFiles = new List<File>(project.GetFiles()); // Remove unecessary instantiation

            foreach (var file in projectFiles)
            {
                var contents = project.GetFileContents(file);
                var commands = SearchForCommands(contents);

                var invokedContent = new List<string>();

                foreach (var command in commands)
                {
                    var wasInvoked = false;
                    foreach (var invoker in invokers)
                    {
                        if (!invoker.CanInvoke(file.FileType) || !invoker.CanInvoke(command.Groups["command"].Value))
                            continue;

                        try
                        {
                            invokedContent.Add(invoker.Invoke(project, command.Groups["command"].Value));
                            wasInvoked = true;
                            break;
                        }
                        catch (ArgumentException)
                        {
                        }
                    }

                    if (!wasInvoked)
                    {
                        invokedContent.Add(null);
                    }
                }

                FinalizeFile(project, file, contents, commands, invokedContent);
            }
        }

        private void FinalizeFile(IProject project, File file, string contents, IEnumerable<Match> commands, List<string> invokedContent)
        {
            var i = 0;
            var start = 0;

            var fileContents = string.Empty;
            foreach (var match in commands)
            {
                if (invokedContent[i] == null)
                    continue;
                var prepend = contents.Substring(start, match.Index);
                fileContents += prepend;
                fileContents += invokedContent[i];
                start += match.Index + match.Length;
                i++;
            }

            if (start < contents.Length)
            {
                fileContents += contents.Substring(start);
            }

            project.AddToDestination(file, fileContents);
        }

        private IEnumerable<Match> SearchForCommands(string fileContents)
        {
            return Regex.Matches(fileContents, @"\<\!\-\-\s*\{\%(?<command>.+)\%}\s*\-\-\>");
        }

        internal TextWriter StartFile(File file)
        {
            return new StreamWriter(new FileStream(file.Path, FileMode.Create));
        }
    }
}