using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Sd
{
    class Triangle
    {
        PointF A, B, C;

        public Triangle(PointF A, PointF B, PointF C)
        {
            this.A = A;
            this.B = B;
            this.C = C;
        }

        public void Draw(Graphics g)
        {
            Pen pen = new Pen(Color.Red,2);
            g.DrawLine(pen, A, B);
            g.DrawLine(pen, B, C);
            g.DrawLine(pen, C, A);
        }
    }
}
