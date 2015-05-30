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
        public static void Test_5_29_2()
        {
            //var dir = @"C:\Users\Justin\Desktop\month_data";

            //foreach (var filePath in Directory.GetFiles(dir))
            //{
            //    var file = filePath.Split(new[] {dir}, StringSplitOptions.None)[1];
            //    var underSplit = file.Split('_');
            //    if (underSplit.Length < 2) { continue; }
            //    var leftUnder = underSplit[0];
            //    var rightUnder = underSplit[1];
            //    var dotSplit = rightUnder.Split('.');
            //    if (dotSplit.Length < 2) { continue; }
            //    var leftDot = dotSplit[0];
            //    var rightDot = dotSplit[1];
            //    if (leftDot == "keyword") { continue; }

            //    var date = leftUnder;
            //    var advertisers = string.Format(@"{0}\{1}_advertiser.dat", dir, date);
            //    var keywords = string.Format(@"{0}\{1}_keyword.dat", dir, date);

            //    var advFile = Stats.GetOneSectorAdvertisersFile(advertisers);

            //    var parser = new Parser();
            //    var outputFileByAdv = parser.ToFileList(advFile, keywords, 1, 5905, false);
            //    var outputFileByKey = parser.ToFileList(advFile, keywords, 1, 5905, true);

            //    var outByAdv = string.Format(@"{0}\{1}_5905_filtered_by_adv.dat", dir, date);
            //    var outByKey = string.Format(@"{0}\{1}_5905_filtered_by_key.dat", dir, date);

            //    var advGraph = parser.FromFile(outputFileByAdv, true, true, false);
            //    var keyGraph = parser.FromFile(outputFileByKey, true, true, true);

            //    outByAdv.DeleteWrite(advGraph.ToFile());
            //    outByKey.DeleteWrite(keyGraph.ToFile());
                
            //    IntensityByAdvertiserMaxIntensity intensityByAdv = new IntensityByAdvertiserMaxIntensity(advGraph, 1.0f, Decision.Weights);
            //    intensityByAdv.Init();
            //    intensityByAdv.Run();

            //    IntensityByAdvertiserMaxIntensity intensityByKey = new IntensityByAdvertiserMaxIntensity(keyGraph, 1.0f, Decision.Weights);
            //    intensityByKey.Init();
            //    intensityByKey.Run();

            //    string.Format(@"{0}\{1}_5905_by_adv_intensity.dat", dir, date).DeleteWrite(advGraph.ToCommunities());
            //    string.Format(@"{0}\{1}_5905_by_key_intensity.dat", dir, date).DeleteWrite(keyGraph.ToCommunities());

            //    advGraph = parser.FromFile(outputFileByAdv, true, true, false);
            //    keyGraph = parser.FromFile(outputFileByKey, true, true, true);

            //    IntensityByAdvertiserMaxIntensity intensityByAdv = new IntensityByAdvertiserMaxIntensity(advGraph, 1.0f, Decision.Weights);
            //    intensityByAdv.Init();
            //    intensityByAdv.Run();

            //    IntensityByAdvertiserMaxIntensity intensityByKey = new IntensityByAdvertiserMaxIntensity(keyGraph, 1.0f, Decision.Weights);
            //    intensityByKey.Init();
            //    intensityByKey.Run();

            //    string.Format(@"{0}\{1}_5905_by_adv_intensity.dat", dir, date).DeleteWrite(advGraph.ToCommunities());
            //    string.Format(@"{0}\{1}_5905_by_key_intensity.dat", dir, date).DeleteWrite(keyGraph.ToCommunities());
            //}

            //var advertisers = @"C:\Users\Justin\Desktop\20130109\20130109_advertisers.dat";
            //var adv = @"C:\Users\Justin\Desktop\20130109\output_one_sect_ari.dat";
            //var advFile = Stats.GetOneSectorAdvertisersFile(advertisers, adv);

            //var output = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph.dat";
            //var key = @"C:\Users\Justin\Desktop\20130109\20130109_keywords.dat";

            //var parser = new Parser();
            //var outputFileByKey = parser.ToFileList(advFile, key, output, 1, 5905, true);
            //var outputFileByAdv = parser.ToFileList(advFile, key, output, 1, 5905, false);

            //var outByKey = @"C:\Users\Justin\Desktop\20130109\20130109_5905_filtered_by_key.dat";
            //var outByAdv = @"C:\Users\Justin\Desktop\20130109\20130109_5905_filtered_by_adv.dat";

            //outByKey.DeleteWrite(parser.FromFile(outputFileByKey, true, true).ToFile());
            //outByAdv.DeleteWrite(parser.FromFile(outputFileByAdv, true, true, false).ToFile());
        }

        public static void Test_5_29_1()
        {
            var advertisers = @"C:\Users\Justin\Desktop\20130109\20130109_advertisers.dat";
            var advFile = Stats.GetOneSectorAdvertisersFile(advertisers);
        
            var key = @"C:\Users\Justin\Desktop\20130109\20130109_keywords.dat";

            var parser = new Parser();
            var outputFileByKey = parser.ToFileList(advFile, key, 1, 5905, true);
            var outputFileByAdv = parser.ToFileList(advFile, key, 1, 5905, false);

            var outByKey = @"C:\Users\Justin\Desktop\20130109\20130109_5905_filtered_by_key.dat";
            var outByAdv = @"C:\Users\Justin\Desktop\20130109\20130109_5905_filtered_by_adv.dat";

            var same2 = Same(outputFileByAdv, outputFileByKey);

            var p1 = parser.FromFile(outputFileByAdv, outputFileByKey, true, true, false);

            var same3 = Same(outputFileByAdv, outputFileByKey);
            
            var p2 = parser.FromFile(outputFileByKey, outputFileByAdv, true, true, false);

            var f1 = p1.Item1.ToFile();
            var f2 = p2.Item1.ToFile();

            var same1 = Same(f1, f2);

            outByAdv.DeleteWrite(f1);
            outByKey.DeleteWrite(f2);
        }

        public static bool Same(List<string> a, List<string> b)
        {
            Dictionary<string, bool> aDic = new Dictionary<string, bool>();
            foreach (var line in a)
            {
                var split = line.Split('\t');
                var adv = split[0];
                var key = split[1];
                aDic[adv + "-" + key] = true;
            }

            Dictionary<string, bool> bDic = new Dictionary<string, bool>();
            foreach (var line in b)
            {
                var split = line.Split('\t');
                var adv = split[0];
                var key = split[1];
                bDic[adv + "-" + key] = true;
            }

            var ret = true;

            foreach (var pair in aDic)
            {
                if (!(bDic.ContainsKey(pair.Key)))
                {
                    Debug.WriteLine("Mismatch Found in a not in b:");
                    Debug.WriteLine(pair.Key);
                    ret = false;
                }
            }

            foreach (var pair in bDic)
            {
                if (!(aDic.ContainsKey(pair.Key)))
                {
                    Debug.WriteLine("Mismatch Found in b not in a:");
                    Debug.WriteLine(pair.Key);
                    ret = false;
                }
            }

            if (aDic.Count != bDic.Count) { ret = false; }

            return ret;
        }

        public static void Test_5_25_3()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph.dat";
            var key_lambda = @"C:\Users\Justin\Desktop\20130109\key_lambda_1.0_2";
            var key_lambda_pdf = @"C:\Users\Justin\Desktop\20130109\key_lambda_1.0_2_pdf.dot";
            var key_lambda_dot = @"C:\Users\Justin\Desktop\20130109\key_lambda_1.0_2.dot";
            var key_max_intensity = @"C:\Users\Justin\Desktop\20130109\key_lambda_max_intensity_1.0_2";
            var key_max_intensity_dot = @"C:\Users\Justin\Desktop\20130109\key_lambda_max_intensity_1.0_2.dot";
            var graph_dat = @"C:\Users\Justin\Desktop\20130109\5_25_3_graph.dat";
            var parser = new Parser();
            var graph = parser.FromFile(input, true, true);
            graph_dat.DeleteWrite(graph.ToFile());
            var intensityByKeyword = new IntensityByKeyword(graph, 1.0f, Decision.Weights);
            intensityByKeyword.Init();
            intensityByKeyword.Message += (o, e) =>
            {
                Console.Clear();
                Console.WriteLine(((MessageEventArgs)e).Message);
            };
            intensityByKeyword.Run();
            key_lambda.DeleteWrite(graph.ToCommunities());
            key_lambda_dot.DeleteWrite(graph.ToDot(DotOptions.RedGray | DotOptions.Small));
            key_lambda_pdf.DeleteWrite(graph.ToDot(DotOptions.Pdf));

            var intensityByKeywordMaxIntensity = new IntensityByKeywordMaxIntensity(graph, 1.0f, Decision.Weights);
            intensityByKeywordMaxIntensity.Init();
            intensityByKeywordMaxIntensity.Message += (o, e) =>
            {
                Console.Clear();
                Console.WriteLine(((MessageEventArgs)e).Message);
            };
            intensityByKeywordMaxIntensity.Run();
            key_max_intensity.DeleteWrite(graph.ToCommunities());
            key_max_intensity_dot.DeleteWrite(graph.ToDot(DotOptions.RedGray | DotOptions.Small));

        }

        public static void Test_5_25_2()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph.dat";
            var adv_lambda = @"C:\Users\Justin\Desktop\20130109\adv_lambda_0.5_2";
            var adv_max_intensity = @"C:\Users\Justin\Desktop\20130109\adv_lambda_max_intensity_0.5_2";
            var adv_mod_perm = @"C:\Users\Justin\Desktop\20130109\adv_lambda_mod_perm_least_1.0_1";
            var parser = new Parser();
            var graph = parser.FromFile(input, true);
            var intensityByAdvertiser = new IntensityByAdvertiser(graph, 0.5f, Decision.Weights);
            intensityByAdvertiser.Init();
            intensityByAdvertiser.Message += (o, e) =>
            {
                Console.Clear();
                Console.WriteLine(((MessageEventArgs)e).Message);
            };
            intensityByAdvertiser.Run();
            adv_lambda.DeleteWrite(graph.ToCommunities());

            var intensityByAdvertiserMaxIntensity = new IntensityByAdvertiserMaxIntensity(graph, 0.5f, Decision.Weights);
            intensityByAdvertiserMaxIntensity.Init();
            intensityByAdvertiserMaxIntensity.Message += (o, e) =>
            {
                Console.Clear();
                Console.WriteLine(((MessageEventArgs)e).Message);
            };
            intensityByAdvertiserMaxIntensity.Run();
            adv_max_intensity.DeleteWrite(graph.ToCommunities());

            var modPerm = new ModPermLeast(graph, 1.0f, Decision.Weights);
            modPerm.Init();
            modPerm.Message += (o, e) =>
            {
                Console.Clear();
                Console.WriteLine(((MessageEventArgs)e).Message);
            };
            modPerm.Run();
            adv_mod_perm.DeleteWrite(graph.ToCommunities());
        }

        public static void Test_5_25_1()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph.dat";
            var adv_lambda = @"C:\Users\Justin\Desktop\20130109\adv_lambda";
            var adv_max_intensity = @"C:\Users\Justin\Desktop\20130109\adv_lambda_max_intensity";
            var parser = new Parser();
            var graph = parser.FromFile(input, true);
            var intensityByAdvertiser = new IntensityByAdvertiser(graph, 0.1f, Decision.Weights);
            intensityByAdvertiser.Init();
            intensityByAdvertiser.Message += (o, e) =>
            {
                Console.Clear();
                Console.WriteLine(((MessageEventArgs)e).Message);
            };
            intensityByAdvertiser.Run();
            adv_lambda.DeleteWrite(graph.ToCommunities());

            var intensityByAdvertiserMaxIntensity = new IntensityByAdvertiserMaxIntensity(graph, 0.1f, Decision.Weights);
            intensityByAdvertiserMaxIntensity.Init();
            intensityByAdvertiserMaxIntensity.Message += (o, e) =>
            {
                Console.Clear();
                Console.WriteLine(((MessageEventArgs)e).Message);
            };
            intensityByAdvertiserMaxIntensity.Run();
            adv_max_intensity.DeleteWrite(graph.ToCommunities());
        }

        public static void Test_4Ari()
        {
            var input = @"C:\Users\Justin\Desktop\20130109\output_ari_one_sect_graph.dat";
            var adv_norm = @"C:\Users\Justin\Desktop\20130109\adv_norm";
            var key_norm = @"C:\Users\Justin\Desktop\20130109\key_norm";
            var wfile = @"C:\Users\Justin\Desktop\20130109\wtfile";
            var perm_ky = @"C:\Users\Justin\Desktop\20130109\perm_ky";
            var perm_adv = @"C:\Users\Justin\Desktop\20130109\perm_adv";
            var parser = new Parser();
            var graph = parser.FromFile(input, true);
            wfile.DeleteWrite(graph.ToFile());
            var modPermCount = new ModPermCount(graph, 1.0f, Decision.Weights);
            modPermCount.Init();
            modPermCount.Message += (o, e) =>
            {
                Console.Clear();
                Console.WriteLine(((MessageEventArgs)e).Message);
            };
            modPermCount.Run();
            perm_ky.DeleteWrite(graph.ToCommunities());
            
            var modPermMaxPerm = new ModPermMaxPerm(graph, 1.0f, Decision.Weights);
            modPermMaxPerm.Init();
            modPermMaxPerm.Message += (o, e) =>
            {
                Console.Clear();
                Console.WriteLine(((MessageEventArgs)e).Message);
            };
            modPermMaxPerm.Run();
            perm_adv.DeleteWrite(graph.ToCommunities());

            var intensityByAdvertiser = new IntensityByAdvertiser(graph, 1.0f, Decision.Weights);
            intensityByAdvertiser.Init();
            intensityByAdvertiser.Message += (o, e) =>
            {
                Console.Clear();
                Console.WriteLine(((MessageEventArgs)e).Message);
            };
            intensityByAdvertiser.Run();
            adv_norm.DeleteWrite(graph.ToCommunities());

            var intensityByKeyword = new IntensityByKeyword(graph, 1.0f, Decision.Weights);
            intensityByKeyword.Init();
            intensityByKeyword.Message += (o, e) =>
            {
                Console.Clear();
                Console.WriteLine(((MessageEventArgs)e).Message);
            };
            intensityByKeyword.Run();
            key_norm.DeleteWrite(graph.ToCommunities());
        }

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
            var intensity = new ModPermMaxPerm(graph, 1.0f, Decision.Weights);
            intensity.Init();
            if (File.Exists(preDot)) { File.Delete(preDot); }
            File.WriteAllText(preDot, graph.ToDot(DotOptions.Paper));
            intensity.Message += (o, e) =>
            {
                Console.Clear();
                Console.WriteLine(((MessageEventArgs)e).Message);
            };
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
            var intensity = new ModPermMaxPerm(graph, 1.0f, Decision.Weights);
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
            var intensity = new ModPermMaxPerm(graph, 0.5f, Decision.Weights);
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
            var intensity = new ModPermMaxPerm(graph, 0.5f, Decision.Weights);
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
            var intensity = new ModPermMaxPerm(graph, 1.0f, Decision.Weights);
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
