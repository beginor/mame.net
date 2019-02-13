using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace mame
{
    public partial class PGM
    {
        public static Color m_ColorG;
        public static bool bRender0G, bRender1G, bRender2G, bRender3G;

        public static void GDIInit()
        {
            m_ColorG = Color.Magenta;
        }
        public static void GetData()
        {

        }
        public static Bitmap GetTx()
        {
            Bitmap bm1;
            bm1 = new Bitmap(512, 256);
            return bm1;
        }
        public static Bitmap GetBg()
        {
            Bitmap bm1;
            bm1 = new Bitmap(0x200, 0x100);
            int i1, i2, i3, n1, n2, n3;
            int i, j;
            byte[] bb1, bb2, bb3;
            ushort[] uu1;
            int[] ii2;
            bb1 = GetBB("txpix1.dat");
            bb2 = GetBB("pal1.dat");
            bb3 = GetBB("txflag2.dat");
            n1 = bb1.Length / 2;
            uu1 = new ushort[n1];
            for (i1 = 0; i1 < n1; i1++)
            {
                uu1[i1] = (ushort)(bb1[i1 * 2] + bb1[i1 * 2 + 1] * 0x100);
            }
            n2 = bb2.Length / 4;
            ii2 = new int[n2];
            for (i2 = 0; i2 < n2; i2++)
            {
                ii2[i2] = bb2[i2 * 4] + bb2[i2 * 4 + 1] * 0x100 + bb2[i2 * 4 + 2] * 0x10000 + unchecked((int)0xff000000);
            }
            n3 = bb3.Length;
            for (i = 0; i < 0x200; i++)
            {
                for (j = 0; j < 0x100; j++)
                {
                    if (bb3[j * 0x200 + i] == 0x10)
                    {
                        //bm1.SetPixel(i, j, Color.FromArgb(ii2[uu1[j * 0x800 + i]]));
                        bm1.SetPixel(i, j, Color.Black);
                    }
                }
            }
            return bm1;
        }
        public static byte[] GetBB(string sFile)
        {
            int n1;
            byte[] bb1;
            FileStream fs1 = new FileStream(sFile, FileMode.Open);
            BinaryReader br1 = new BinaryReader(fs1);
            n1 = (int)fs1.Length;
            bb1 = new byte[n1];
            br1.Read(bb1, 0, n1);
            br1.Close();
            fs1.Close();
            return bb1;
        }
    }
}
