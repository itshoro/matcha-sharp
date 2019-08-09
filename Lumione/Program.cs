using System;
using System.IO;
using System.Threading;
using CommandLine;
using Lumione.Invokers;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Lumione
{
    public class Program
    {
        [Verb("init")]
        private class InitOptions
        {
            // [Option('d', "destination")]
            // public string DestinationPath { get; set; }
        }

        [Verb("build")]
        private class BuildOptions
        {
            // [Option('m', "minify")]
            // public bool Minify { get; set; }
        }

        private static int Main(string[] args)
        {
            var invokers = ReflectiveEnumerator.GetEnumerableOfType<InvokerBase>();
            var reserved = new List<string>();
            foreach (var invoker in invokers)
            {
                reserved.AddRange(invoker.ReservedDirectories());
            }

            Settings settings = new Settings()
            {
                CreateSubdirectories = true
            };

            var project = new Project(new Uri(Environment.CurrentDirectory), new FileAccess(), settings);

            return Parser.Default.ParseArguments<InitOptions, BuildOptions>(args)
                .MapResult(
                    (InitOptions opts) =>
                    {
                        project.Prepare(settings, reserved);
                        return 0;
                    },
                    (BuildOptions opts) =>
                    {
                        var builder = new ProjectBuilder();
                        builder.Build(project, settings, reserved, invokers, new FileAccess());
                        return 0;
                    },
                    errors => -1
                );
        }
    }
}