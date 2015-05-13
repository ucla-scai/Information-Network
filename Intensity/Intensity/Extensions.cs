﻿using System;
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

        public static bool Is(this float f, float s)
        {
            return Math.Round(f, 4) == Math.Round(s, 4);
        }

        public static int ToInt(this float f)
        {
            return Convert.ToInt32(f);
        }

        public static decimal ToDecimal(this int i)
        {
            return Convert.ToDecimal(i);
        }

        public static float ToFloat(this int i)
        {
            return Convert.ToSingle(i);
        }

        public static float ToFloat(this decimal d)
        {
            return Convert.ToSingle(d);
        }
    }
}
