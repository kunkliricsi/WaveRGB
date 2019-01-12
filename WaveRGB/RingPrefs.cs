﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WaveRGB
{
    static class RingPrefs
    {
        public const string appVersionString = "v1.1";

        public const int maxRings = 5;

        //changing these Strings will localize the interface shown by LGS.

        public static string ringName = "Ring";

        public static double renderFPS = 30;
        public static string renderName = "Frame Rate";
                                                                 // these default values will be returned / retained by LGS
        public static bool[] active         = new bool[maxRings]    { true, true, false, false, true };
        public static double[] life         = new double[maxRings]  { 15, 15, 15, 30, 30 }; //in frames
        public static double[] delay        = new double[maxRings]  { 0, 0, 0, 0, 0 };      //in frames
        public static double[] sizeStart    = new double[maxRings]  { 10, 10, 20, 10, 5 }; //in render pixels
        public static double[] sizeEnd      = new double[maxRings]  { 100, 100, 200, 100, 5 };
        public static Color[] color         = new Color[maxRings]   { Colors.Yellow, Colors.Red, Colors.LimeGreen, Colors.Blue, Colors.White };
        public static double[] thickness    = new double[maxRings]  { 20, 4, 10, 4, 5 };      //in render pixels
        public static double[] opacityStart = new double[maxRings]  { 100, 100, 100, 100, 100 };    //in %
        public static double[] opacityEnd   = new double[maxRings]  { 0, 0, 50, 0, 0 };             //in %

        public static string activeName = "Active";
        public static string lifeName = "Lifetime";
        public static string delayName = "Render Delay";
        public static string sizeStartName = "Starting Radius";
        public static string sizeEndName = "Ending Radius";
        public static string colorName = "Color";
        public static string thicknessName = "Thickness";
        public static string opacityStartName = "Starting Visibility %";
        public static string opacityEndName = "Ending Visibility %";
    }
}
