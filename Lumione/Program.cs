using System;

namespace Lumione
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var settings = new Settings(Environment.CurrentDirectory);
            var processor = new Processor(settings);

            Console.WriteLine("========");
            Console.WriteLine("Settings");
            Console.WriteLine("========");
            Console.WriteLine($"BasePath {settings.BasePath}");
            Console.WriteLine($"BuildPath {settings.BuildPath}");
            Console.WriteLine($"IncludePath {settings.IncludePath}");
            Console.WriteLine();
            Console.WriteLine($"Tracking {processor.Count} files..");

            processor.ExecuteCommandsIfPossible();
        }
    }
}