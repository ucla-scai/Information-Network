using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Intensity
{
    public class Tests
    {
        public static void Test_Parse()
        {
            var output = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph.dat";
            if (File.Exists(output)) { File.Delete(output); }

            var args = new List<string>();
            args.Add("-p");
            args.Add("-a");
            args.Add(@"C:\Users\Justin\Desktop\20130109\output_one_sect_ari.dat");
            args.Add("-k");
            args.Add(@"C:\Users\Justin\Desktop\20130109\20130109_keywords.dat");
            args.Add("-s");
            args.Add("1");
            args.Add("-o");
            args.Add(output);
            Program.Main(args.ToArray());
        }

        public static void Test_Small_Graph()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\demo.dat";
            var parser = new Parser();
            var graph = parser.FromFile(input);
            //var intensity = new Intensity(graph, float.Parse("1"));
            //intensity.Init();
            //var score = intensity.Run();
            Debug.WriteLine(graph.ToDot());
            //Console.WriteLine("score=" + score);
            //Debug.WriteLine("score=" + score);
        }

        public static void Test_5905_Graph()
        {
            var output = @"C:\Users\Justin\Desktop\20130109\output_5905_graph.dat";
            var keywords = @"C:\Users\Justin\Desktop\20130109\20130109_keywords.dat";
            var advertisers = @"C:\Users\Justin\Desktop\20130109\20130109_advertisers.dat";
            var parser = new Parser();
            parser.ToFile(advertisers, keywords, output, 1, 5905);
        }

        public static void Test_Advertiser_Keywords()
        {
            var output = @"C:\Users\Justin\Desktop\20130109\output_keywords_5905.dat";
            var keywords = @"C:\Users\Justin\Desktop\20130109\20130109_keywords.dat";
            var advertisers = @"C:\Users\Justin\Desktop\20130109\20130109_advertisers.dat";
            Stats.GetKeywordFile(advertisers, keywords, output);
        }

        public static void Test_Intensity()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\output_test_wenchao.dat";
            var args = new List<string>();
            args.Add("-i");
            args.Add(input);
            args.Add("-l");
            args.Add("1");
            Program.Main(args.ToArray());
        }

        public static void Test_Get_Sector()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\output_test_wenchao.dat";
            var advertisers = @"C:\Users\Justin\Desktop\20130109\20130109_advertisers.dat";
            Stats.GetSector(input, advertisers);
        }

        public static void Test_Get_Cpc()
        {
            var advertisers = @"C:\Users\Justin\Desktop\20130109\20130109_advertisers.dat";
            var output = @"C:\Users\Justin\Desktop\20130109\output_test_ariaym.dat";
            Stats.GetCpC("6414", advertisers, output);
        }

        public static void Test_Get_Multi_Sector_Adv()
        {
            var advertisers = @"C:\Users\Justin\Desktop\20130109\20130109_advertisers.dat";
            var output = @"C:\Users\Justin\Desktop\20130109\output_multi_sect_ari.dat";
            Stats.GetMultiSectorAdvertisers(advertisers, output);
        }

        public static void Test_Get_One_Sector_Adv()
        {
            var advertisers = @"C:\Users\Justin\Desktop\20130109\20130109_advertisers.dat";
            var output = @"C:\Users\Justin\Desktop\20130109\output_one_sect_ari.dat";
            Stats.GetOneSectorAdvertisers(advertisers, output);
        }
        public static void Test_Get_Sector_Adv()
        {
            var advertisers = @"C:\Users\Justin\Desktop\20130109\20130109_advertisers.dat";
            var output = @"C:\Users\Justin\Desktop\20130109\advertisers_5905.dat";
            Stats.GetSectorAdvertisers(advertisers, output);
        }

        public static void Test_Get_Max_Sector_Adv()
        {
            var advertisers = @"C:\Users\Justin\Desktop\20130109\output_one_sect_ari.dat";
            var output = @"C:\Users\Justin\Desktop\20130109\output_one_sect_ari.dat";
            Stats.GetMaxSectorAdvertiser(advertisers, output);
        }
    }
}
