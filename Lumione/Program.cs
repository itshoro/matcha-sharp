using System;
using System.IO;
using System.Threading;
using CommandLine;
using Lumione.Invokers;
using Newtonsoft.Json;

namespace Lumione
{
    public class Program
    {
        [Verb("init")]
        private class InitOptions
        {
            [Option('d', "destination")]
            public string DestinationPath { get; set; }
        }

        [Verb("build")]
        private class BuildOptions { }

        private static int Main(string[] args)
        {
            args = new[] { "init", "-d", @"F:\Development\HTML\Lumione\Lumione\bin\Debug\netcoreapp2.1\test" };
            return Parser.Default.ParseArguments<InitOptions, BuildOptions>(args)
                .MapResult(
                    (InitOptions opts) =>
                    {
                        Project project;
                        if (!string.IsNullOrEmpty(opts.DestinationPath))
                        {
                            project = new Project(Environment.CurrentDirectory, opts.DestinationPath);
                        }
                        else
                        {
                            project = new Project(Environment.CurrentDirectory);
                        }
                        project.Prepare();
                        return 0;
                    },
                    (BuildOptions opts) =>
                    {
                        var project = new Project(Environment.CurrentDirectory);
                        var builder = new ProjectBuilder();
                        var invokers = ReflectiveEnumerator.GetEnumerableOfType<InvokerBase>();
                        builder.Build(project, invokers);

                        return 0;
                    },
                    errors => -1
                );
        }
    }
}