using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Intensity
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var output = @"C:\Users\Justin\Desktop\20130109\output_test.dat";
            if (File.Exists(output)) { File.Delete(output); }

            var argsList = new List<string>();
            argsList.Add("-p");
            argsList.Add("-a");
            argsList.Add(@"C:\Users\Justin\Desktop\20130109\20130109_advertisers.dat");
            argsList.Add("-k");
            argsList.Add(@"C:\Users\Justin\Desktop\20130109\20130109_keywords.dat");
            argsList.Add("-o");
            argsList.Add(output);

            args = argsList.ToArray();

            if (args.Length == 0)
            {
                Help();
            }
            else if (args.First() == "-h" || args.First() == "--help")
            {
                Help();
            }
            else if (args.First() == "-p" || args.First() == "--parse")
            {
                Parse(args);
            }
            else
            {
                var graph = new Graph();
                var intensity = new Intensity(graph);
                intensity.Init();
                var score = intensity.Run();
            }
            Console.Read();
        }

        private static void Parse(string[] args)
        {
            if (args.Length != 7) { Help(); return; }
            string keyword = null;
            string advertiser = null;
            string output = null;
            for (var i = 1; i < args.Length; i++)
            {
                if (args[i] == "-k" || args[i] == "--keyword")
                {
                    keyword = args[i + 1];
                }
                if (args[i] == "-a" || args[i] == "--advertiser")
                {
                    advertiser = args[i + 1];
                }
                if (args[i] == "-o" || args[i] == "--output")
                {
                    output = args[i + 1];
                }
            }
            var parser = new Parser();
            parser.ToFile(keyword, advertiser, output);
        }

        private static void Help()
        {
            Console.WriteLine("");
            Console.WriteLine("Intensity: Information Network Analysis" );
            Console.WriteLine("DPAS");
            Console.WriteLine("");
            Console.WriteLine("Intensity [-p] [--parse] [-a] [--advertiser] [-k] [--keyword] [-o] [--output] [-h] [--help]");
            Console.WriteLine("");
            Console.WriteLine(Line("p", "parse", "build an output graph from raw data -a, -k, and -o flags must be set"));
            Console.WriteLine(Line("a", "advertiser", "raw advertiser data file"));
            Console.WriteLine(Line("k", "keyword", "raw keyword data file"));
            Console.WriteLine(Line("o", "output", "file to output a graph representation of raw data"));
            Console.WriteLine(Line("h", "help", "show this help menu and exit"));
        }

        private static string Line(string shortVersion, string longVersion, string description)
        {
            return string.Format("  -{0}, --{1}", shortVersion, longVersion).PadRight(25, '.') + string.Format("{0}", description);
        }
    }
}
