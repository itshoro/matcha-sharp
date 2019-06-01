using System;
using System.IO;
using CommandLine;
using Newtonsoft.Json;

namespace Lumione
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Lumione v0.1");
            args = new string[] { "build" };
            if (args.Length == 0)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("\tdotnet lumione.dll <command>");
                return;
            }

            var settingsExist = false;
            var settings = Settings.Default(Environment.CurrentDirectory);
            var settingsFile = settings.SourcePath + @"\settings.json";
            if (File.Exists(settingsFile))
            {
                settingsExist = true;
                var szSettings = File.ReadAllText(settingsFile);
                settings = JsonConvert.DeserializeObject<Settings>(szSettings);
            }

            if (args.Length == 1)
            {
                if (args[0] == "build")
                {
                    var processor = new Processor(settings);
                    Console.WriteLine($"Source: {settings.SourcePath}");
                    Console.WriteLine($"Destination: {settings.DestinationPath}");
                    Console.Write($"Tracking...");
                    Console.WriteLine($"{processor.Count} files.");

                    Console.WriteLine($"Building site...");
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    processor.ExecuteCommandsIfPossible();
                    watch.Stop();
                    Console.WriteLine($"\ttook {watch.Elapsed}");
                }
                else if (args[0] == "init")
                {
                    Console.Write("Setting up environment...");
                    Directory.CreateDirectory(settings.IncludePath);
                    Directory.CreateDirectory(settings.AssetsPath);
                    Console.WriteLine("\tfinished!");

                    if (!settingsExist)
                    {
                        Console.Write("Serializing settings...");
                        var szSettings = JsonConvert.SerializeObject(settings, Formatting.Indented);
                        File.WriteAllText(settingsFile, szSettings);
                        Console.WriteLine("\tfinished!");
                    }
                    else
                    {
                        Console.WriteLine("Settings already exist.");
                    }
                }
                else
                {
                    Console.WriteLine($"Fatal: Not supported");
                }
            }
        }
    }
}