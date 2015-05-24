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

        public static void Test_Small_Graph_Dot()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\demo.dat";
            var parser = new Parser();
            var graph = parser.FromFile(input);
            Debug.WriteLine(graph.ToDot());
        }

        public static void Test_Filtering()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph.dat";
            var parser = new Parser();
            var graph = parser.FromFile(input, true);
            var filtered = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph_filtered.dat";
            if (File.Exists(filtered)) { File.Delete(filtered); }
            File.WriteAllText(filtered, graph.ToDot());
        }

        public static void Test_Cleaned()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph.dat";
            var cleaned = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph_cleaned.dat";
            if (File.Exists(cleaned)) { File.Delete(cleaned); }
            var parser = new Parser();
            parser.Clean(input, cleaned);
        }

        public static void Test_Wen()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\graph.txt";
            var preDotPaper = @"C:\Users\Justin\Desktop\20130109\graph_pre_paper.dot";
            var preDotPdf = @"C:\Users\Justin\Desktop\20130109\graph_pre_pdf.dot";
            var preDotCustomSmall = @"C:\Users\Justin\Desktop\20130109\graph_pre_custom_small.dot";
            var preDotCustomNormal = @"C:\Users\Justin\Desktop\20130109\graph_pre_custom_normal.dot";
            var parser = new Parser(ParserOptions.SpaceDelimited);
            var graph = parser.FromFile(input);
            if (File.Exists(preDotPaper)) { File.Delete(preDotPaper); }
            if (File.Exists(preDotPdf)) { File.Delete(preDotPdf); }
            if (File.Exists(preDotCustomSmall)) { File.Delete(preDotCustomSmall); }
            if (File.Exists(preDotCustomNormal)) { File.Delete(preDotCustomNormal); }
            File.WriteAllText(preDotPaper, graph.ToDot(DotOptions.Paper));
            File.WriteAllText(preDotPdf, graph.ToDot(DotOptions.Pdf));
            File.WriteAllText(preDotCustomSmall, graph.ToDot(DotOptions.SameColor | DotOptions.NodeLabels | DotOptions.NoCommunities | DotOptions.NoOveralp | DotOptions.Small | DotOptions.TransparentEdges));
            File.WriteAllText(preDotCustomNormal, graph.ToDot(DotOptions.SameColor | DotOptions.NodeLabels | DotOptions.NoCommunities | DotOptions.NoOveralp));
            //BoxAndCircles  | RedGray | Labels | NoOveralp
        }

        public static void Test_Filtered()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph.dat";
            var preDot = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph_pre.dot";
            var postDot = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph_post.dot";
            var communitiyFile = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph_com.dat";
            var parser = new Parser();
            var graph = parser.FromFile(input, true);
            var intensity = new Intensity(graph, 1.0f, Decision.Weights);
            intensity.Init();
            if (File.Exists(preDot)) { File.Delete(preDot); }
            File.WriteAllText(preDot, graph.ToDot(DotOptions.Paper));
            intensity.Run();
            if (File.Exists(postDot)) { File.Delete(postDot); }
            File.WriteAllText(postDot, graph.ToDot(DotOptions.RedGray | DotOptions.Small));
            if (File.Exists(communitiyFile)) { File.Delete(communitiyFile); }
            File.WriteAllText(communitiyFile, graph.ToCommunities());
        }

        public static void Test_Small_Graph_Dot_File()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph.dat";
            var parser = new Parser();
            var graph = parser.FromFile(input);
            var preDot = @"C:\Users\Justin\Desktop\20130109\demo_pre.dot";
            if (File.Exists(preDot)) { File.Delete(preDot); }
            File.WriteAllText(preDot, graph.ToDot());
        }

        public static void Test_W_2()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\input_w_2.dat";
            var parser = new Parser();
            var graph = parser.FromFile(input);
            var intensity = new Intensity(graph, 1.0f, Decision.Weights);
            intensity.Init();
            Debug.WriteLine(graph.ToDot());
            intensity.Run();
            Debug.WriteLine(graph.ToDot());
        }

        public static void Test_W_1()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\input_w_1.dat";
            var parser = new Parser();
            var graph = parser.FromFile(input);
            var intensity = new Intensity(graph, 0.5f, Decision.Weights);
            intensity.Init();
            Debug.WriteLine(graph.ToDot());
            intensity.Run();
            Debug.WriteLine(graph.ToDot());
        }

        public static void Test_Small_Graph()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\input.dat";
            var parser = new Parser();
            var graph = parser.FromFile(input);
            var intensity = new Intensity(graph, 0.5f, Decision.Weights);
            intensity.Init();
            intensity.Run();
            Debug.WriteLine(graph.ToString());
        }

        public static void Test_5905_Dot()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\output_5905_graph_filtered.dat";
            var preDot = @"C:\Users\Justin\Desktop\20130109\output_5905_graph_filtered_pre.dot";
            var postDot = @"C:\Users\Justin\Desktop\20130109\output_5905_graph_filtered_post.dot";
            var parser = new Parser();
            var graph = parser.FromFile(input);
            var intensity = new Intensity(graph, 1.0f, Decision.Weights);
            intensity.Init();
            if (File.Exists(preDot)) { File.Delete(preDot); }
            File.WriteAllText(preDot, graph.ToDot(DotOptions.Paper));
            intensity.Run();
            if (File.Exists(postDot)) { File.Delete(postDot); }
            File.WriteAllText(postDot, graph.ToDot());
        }

        public static void Test_5905_Graph()
        {
            var output = @"C:\Users\Justin\Desktop\20130109\output_5905_graph_filtered.dat";
            var keywords = @"C:\Users\Justin\Desktop\20130109\20130109_keywords.dat";
            var advertisers = @"C:\Users\Justin\Desktop\20130109\20130109_advertisers.dat";
            if (File.Exists(output)) { File.Delete(output); }
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
