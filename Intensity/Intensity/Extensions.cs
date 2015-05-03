using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intensity
{
    public static class Extensions
    {
        public static bool Is(this float f, int i)
        {
            return Math.Floor(f) == Math.Ceiling(f) && Convert.ToInt32(Math.Ceiling(f)) == i;
        }

        public static int ToInt(this float f)
        {
            return Convert.ToInt32(f);
        }
    }
}
