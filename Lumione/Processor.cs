using Lumione.Invokers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lumione
{
    public class Processor
    {
        private IEnumerable<string> FilePaths;
        private List<string> SupportedTypes;
        public int Count { get { return FilePaths.Count(); } }

        public Processor(SettingsManager settings)
        {
            SupportedTypes = new List<string>();
            SupportedTypes.Add(".html");
            SupportedTypes.Add(".htm");
            SupportedTypes.Add(".HTML");
            SupportedTypes.Add(".dHTM");

            FilePaths = AddFiles(settings.BasePath, settings.IgnoredDirectories);
            FilePaths = FilePaths.Where(s => settings.IgnoredDirectories.Any(dir => !s.StartsWith(dir)));

            if (!System.IO.Directory.Exists(settings.BuildPath))
            {
                System.IO.Directory.CreateDirectory(settings.BuildPath);
            }
        }

        private IEnumerable<string> AddFiles(string path, List<string> ignore)
        {
            var files = System.IO.Directory.EnumerateFiles(path, "*", System.IO.SearchOption.AllDirectories);
            return files.Where(s => SupportedTypes.Any(type => type == System.IO.Path.GetExtension(s)));
        }

        public void Invoke(IInvoke invoker, SettingsManager settings)
        {
            foreach (var filePath in FilePaths)
            {
                var newLines = new List<string>();
                var lines = System.IO.File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    if (ContainsComment(line))
                    {
                        newLines.Add(invoker.Invoke(line, filePath));
                    }
                    else
                    {
                        newLines.Add(line);
                    }
                }

                var relativePath = filePath.Remove(0, settings.BasePath.Length);
                var newPath = settings.BuildPath + relativePath;
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(newPath)))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(newPath));
                }
                System.IO.File.WriteAllLines(settings.BuildPath + relativePath, newLines.ToArray());
            }
        }

        private bool ContainsComment(string line)
        {
            return Regex.IsMatch(line, @"<!--(.*)-->");
        }
    }
}