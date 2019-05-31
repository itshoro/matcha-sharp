using Lumione.Invokers;
using System;
using System.IO;

namespace Lumione
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new SettingsManager(Environment.CurrentDirectory);
            var processor = new Processor(settings);

            if (processor.Count == 0)
            {
                Console.WriteLine("Couldn't act on this path (Project is empty)");
                return;
            }

            Console.WriteLine($"BasePath {settings.BasePath}");
            Console.WriteLine($"BuildPath {settings.BuildPath}");

            var invokers = ReflectiveEnumerator.GetEnumerableOfType<IInvoke>();
            foreach (var invoker in invokers)
            {
                processor.Invoke(invoker, settings);
            }
        }
    }
}
