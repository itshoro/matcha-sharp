using Lumione.Invokers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Lumione
{
    public class ProjectBuilder
    {
        public async void BuildAsync(IProject project, IEnumerable<IInvoker> invokers, IFileAccess fileAccess)
        {
            project.PrepareBuild();

            var projectFiles = project.GetFiles();

            foreach (var file in projectFiles)
            {
                var contents = await fileAccess.ReadAsync(project.GetFilePath(file));
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
                            invokedContent.Add(await invoker.InvokeAsync(project, fileAccess, command.Groups["command"].Value));
                            wasInvoked = true;
                            break;
                        }
                        catch (ArgumentException)
                        {
                            // Stops the program from breaking if an invokation causes a failure. 
                            // I want to remove this, so that plugin devs have to be careful themselves.
                        }
                    }

                    if (!wasInvoked)
                    {
                        invokedContent.Add(null);
                    }
                }

                var destPath = project.GetDestinationPathOfFile(file);
                var newContents = ReplaceInvokedContent(contents, commands, invokedContent);

                await fileAccess.WriteAsync(destPath, newContents);
            }
        }

        public void Build(IProject project, IEnumerable<IInvoker> invokers, IFileAccess fileAccess)
        {
            project.PrepareBuild();

            var projectFiles = project.GetFiles();

            foreach (var file in projectFiles)
            {
                var contents = fileAccess.Read(project.GetFilePath(file));
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
                            // TODO: Let Invokers work asynchronous.
                            invokedContent.Add(invoker.Invoke(project, fileAccess, command.Groups["command"].Value));
                            wasInvoked = true;
                            break;
                        }
                        catch (ArgumentException)
                        {
                            // Stops the program from breaking if an invokation causes a failure. 
                            // I want to remove this, so that plugin devs have to be careful themselves.
                        }
                    }

                    if (!wasInvoked)
                    {
                        invokedContent.Add(null);
                    }
                }

                var newContents = ReplaceInvokedContent(contents, commands, invokedContent);
                var destPath = project.GetDestinationPathOfFile(file);

                fileAccess.Write(destPath, newContents);
            }
        }

        private string ReplaceInvokedContent(string contents, IEnumerable<Match> matches, IList<string> replacement)
        {
            var i = 0;
            var position = 0;
            var sb = new StringBuilder();
            foreach (var match in matches)
            {
                if (replacement[i] == null)
                    continue;
                var prepend = contents.Substring(position, match.Index);

                sb.Append(prepend);
                sb.Append(replacement[i]);

                position += match.Index + match.Length;
                i++;
            }

            if (position < contents.Length)
            {
                sb.Append(contents.Substring(position));
            }

            return sb.ToString();
        }

        private IList<Match> SearchForCommands(string fileContents)
        {
            return Regex.Matches(fileContents, @"\<\!\-\-\s*\{\%(?<command>.+)\%}\s*\-\-\>");
        }

        internal TextWriter StartFile(File file)
        {
            return new StreamWriter(new FileStream(file.Path, FileMode.Create));
        }
    }
}