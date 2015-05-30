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
            Tests.Test_5_29_1();
            return;

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
            else if (args.First() == "-i" || args.First() == "--input")
            {
                Intensity(args);
            }
            else
            {
                Help();
            }
            Console.WriteLine("\n<DONE>");
            Debug.WriteLine("\n<DONE>");
            Console.Read();
        }

        private static void Intensity(string[] args)
        {
            string input = null;
            string lambda = "1";
            string decision = "0";
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == "-i" || args[i] == "--input")
                {
                    input = args[i + 1];
                }
                if (args[i] == "-l" || args[i] == "--lambda")
                {
                    lambda = args[i + 1];
                }
                if (args[i] == "-d" || args[i] == "--decision")
                {
                    decision = args[i + 1];
                }
            }
            var graph = new Parser().FromFile(input);
            var decisionEnum = (Decision)int.Parse(decision);
            var intensity = new ModPermMaxPerm(graph, float.Parse(lambda), decisionEnum);
            intensity.Init();
            intensity.Message += new EventHandler(Intensity_Message);
            var score = intensity.Run();
            Debug.WriteLine(graph.ToDot());
            Console.WriteLine("score=" + score);
            Debug.WriteLine("score=" + score);
        }

        static void Intensity_Message(object sender, EventArgs e)
        {
            Console.Clear();
            Console.WriteLine(((MessageEventArgs)e).Message);
        }

        private static void Parse(string[] args)
        {
            if (args.Length < 7) { Help(); return; }
            string keyword = null;
            string advertiser = null;
            string output = null;
            string sectors = null;
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
                if (args[i] == "-s" || args[i] == "--sectors")
                {
                    sectors = args[i + 1];
                }
            }
            var parser = new Parser();
            parser.ToFile(advertiser, keyword, output, int.Parse(sectors));
        }

        private static void Help()
        {
            Console.WriteLine("");
            Console.WriteLine("Intensity: Information Network Analysis");
            Console.WriteLine("DPAS");
            Console.WriteLine("");
            Console.WriteLine("Intensity [-l] [--lambda] [-d] [--decision] [-s] [--sectors] [-i] [--input] [-p] [--parse] [-a] [--advertiser] [-k] [--keyword] [-o] [--output] [-h] [--help]");
            Console.WriteLine("");
            Console.WriteLine(Line("i", "input", "run intensity with given input file"));
            Console.WriteLine(Line("l", "lambda", "lambda coefficient used in intensity metric"));
            Console.WriteLine(Line("d", "decision", "which decision to take when placing keywords into a community, values are 0, 1, or 2 for weights, high intensity, and low intensity respective"));
            Console.WriteLine(Line("p", "parse", "build an output graph from raw data -a, -k, and -o flags must be set"));
            Console.WriteLine(Line("s", "sectors", "for building output file how many random sectors to include"));
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
