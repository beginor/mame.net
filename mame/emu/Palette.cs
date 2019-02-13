using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace mame
{
    public class Palette
    {
        public static uint[] entry_color;
        private static uint trans_uint;
        private static int numcolors;
        public static Color trans_color;
        public static void palette_init()
        {
            int index;
            trans_color = Color.Magenta;
            trans_uint = (uint)trans_color.ToArgb();
            switch (Machine.sBoard)
            {
                case "CPS-1":
                case "CPS-1(QSound)":
                case "CPS2":
                    numcolors = 0xc00;
                    break;
                case "Namco System 1":
                    numcolors = 0x2000;
                    break;
                case "IGS011":
                    numcolors = 0x800;
                    break;
                case "PGM":
                    numcolors = 0x901;
                    break;
            }
            entry_color = new uint[numcolors];
            for (index = 0; index < numcolors; index++)
            {
                entry_color[index] = trans_uint;
            }
        }
        public static void palette_entry_set_color(int index, uint rgb)
        {
            if (index >= numcolors || entry_color[index] == rgb)
                return;
            if (index % 0x10 == 0x0f && rgb == 0)
            {
                entry_color[index] = trans_uint;
            }
            else
            {
                entry_color[index] = 0xff000000 | rgb;
            }
        }
        public static uint make_rgb(int r, int g, int b)
        {
            return ((((uint)(r) & 0xff) << 16) | (((uint)(g) & 0xff) << 8) | ((uint)(b) & 0xff));
        }
        public static uint make_argb(int a, int r, int g, int b)
        {
            return ((((uint)(a) & 0xff) << 24) | (((uint)(r) & 0xff) << 16) | (((uint)(g) & 0xff) << 8) | ((uint)(b) & 0xff));
        }
        public static byte pal5bit(byte bits)
        {
            bits &= 0x1f;
            return (byte)((bits << 3) | (bits >> 2));
        }
    }
}