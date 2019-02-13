using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using mame;

namespace ui
{
    public partial class namcos1Form : Form
    {
        private mainForm _myParentForm;
        public namcos1Form(mainForm form)
        {
            this._myParentForm = form;
            InitializeComponent();
        }
        private void namcos1Form_Load(object sender, EventArgs e)
        {
            tbLayer.Text = "1";
        }
        private void namcos1Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void btnDraw_Click(object sender, EventArgs e)
        {
            int i, j, i1, n;
            uint u1;
            Color c1;
            Bitmap bm1 = new Bitmap(512, 512);
            n = int.Parse(tbLayer.Text);
            for (i = 0; i < 0x200; i++)
            {
                for (j = 0; j < 0x200; j++)
                {
                    i1 = Tmap.ttmap[n].pixmap[i + j * 0x200] + Tmap.ttmap[n].palette_offset;
                    u1 = Palette.entry_color[i1];
                    c1 = Color.FromArgb((int)Palette.entry_color[Tmap.ttmap[n].pixmap[i + j * 0x200] + Tmap.ttmap[n].palette_offset]);
                    bm1.SetPixel(i, j, c1);
                }
            }
            pictureBox1.Image = bm1;
        }
    }
}
