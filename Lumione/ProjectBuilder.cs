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
        private StringBuilder PathBuilder;

        private Uri destinationBaseUri;
        private Uri destinationCssUri;
        private Uri destinationJsUri;
        private Uri destinationAssetUri;

        public ProjectBuilder()
        {
            PathBuilder = new StringBuilder();
        }

        public void Build(Project project, Settings settings, IEnumerable<string> reserved, IEnumerable<IInvokable> invokers, IFileAccess fileAccess)
        {
            var files = project.GetCompilableFiles(reserved);

            InitializeRelativeUris(project, settings);

            foreach (var file in files)
            {
                var contents = fileAccess.ReadFromRoot(project, settings, file.Path);
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
                            invokedContent.Add(invoker.Invoke(project, settings, fileAccess, command.Groups["command"].Value));
                            wasInvoked = true;
                            break;
                        }
                        catch (ArgumentException e)
                        {
                            // Stops the program from breaking if an invokation causes a failure.
                            // I want to remove this, so that plugin devs have to be careful themselves.
                            invokedContent.Add($"<!-- [{invoker.GetType().Name}] {e.Message} -->");
                        }
                    }

                    if (!wasInvoked)
                    {
                        invokedContent.Add(null);
                    }
                }

                var newContents = ReplaceInvokedContent(contents, commands, invokedContent);
                var destPath = GetDestinationPathOfFile(project, settings, file);

                fileAccess.Write(destPath, newContents);
            }
        }

        private void InitializeRelativeUris(Project project, Settings settings)
        {
            destinationBaseUri = settings.HasAbsoluteDestinationPath ?
                new Uri(settings.DestinationPath) :
                new Uri(project.Uri, new Uri(settings.DestinationFolderName, UriKind.Relative));

            destinationCssUri = new Uri(destinationBaseUri, new Uri(settings.CssFolderName, UriKind.Relative));
            destinationJsUri = new Uri(destinationBaseUri, new Uri(settings.JavascriptFolderName, UriKind.Relative));
            destinationAssetUri = new Uri(destinationBaseUri, new Uri(settings.AssetsFolderName, UriKind.Relative));
        }

        private string GetDestinationBaseDirectoryOfFile(Settings settings, File file)
        {
            switch (file.FileType)
            {
                case FileType.Stylesheet:
                    return settings.CssFolderName;

                case FileType.Script:
                    return settings.JavascriptFolderName;

                default:
                    return string.Empty;
            }
        }

        private string GetDestinationPathOfFile(Project project, Settings settings, File file)
        {
            Uri uri;
            switch (file.FileType)
            {
                case FileType.Asset:
                    uri = destinationAssetUri;
                    break;

                case FileType.Document:
                    uri = destinationBaseUri;
                    break;

                case FileType.Script:
                    uri = destinationJsUri;
                    break;

                case FileType.Stylesheet:
                    uri = destinationCssUri;
                    break;
            }

            PathBuilder.Clear();
            PathBuilder.Append(settings.DestinationFolderName);

            var baseDirectory = GetDestinationBaseDirectoryOfFile(settings, file);
            if (!string.IsNullOrEmpty(baseDirectory))
            {
                PathBuilder.Append(@"\");
                PathBuilder.Append(baseDirectory);
            }

            var relativePath = Path.GetDirectoryName(file.Path);
            PathBuilder.Append(relativePath);

            var fileName = Path.GetFileNameWithoutExtension(file.Path);
            if (settings.CreateSubdirectories && fileName != "index")
            {
                PathBuilder.Append(fileName);
                PathBuilder.Append(@"\index.html");
            }
            PathBuilder.Append(fileName);
            PathBuilder.Append(@".html");

            return PathBuilder.ToString();
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
    }
}