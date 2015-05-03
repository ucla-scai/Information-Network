using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Intensity
{
    class Program
    {
        static void Main(string[] args)
        {
            var graph = new Graph();
            var intensity = new Intensity(graph);
            intensity.Init();
            var score = intensity.Run();
            Console.Read();
        }
    }
}
