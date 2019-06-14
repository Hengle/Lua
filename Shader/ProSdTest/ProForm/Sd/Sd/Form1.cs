using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sd
{
    public partial class Form1 : Form
    {
        Triangle t;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PointF A = new PointF(0,-100);
            PointF B = new PointF(100,100);
            PointF C = new PointF(-100,100);
            t = new Triangle(A,B,C);
        }

        private void Form1_Paint_1(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(150,110);
            t.Draw(e.Graphics);
        }
    }
}
