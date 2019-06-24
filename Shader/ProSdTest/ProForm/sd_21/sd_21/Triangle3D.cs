using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sd_21
{
    class Triangle3D
    {
        private Vector4 a, b, c; //变换后的值
        public Vector4 A, B, C;

        public Triangle3D() { }
        public Triangle3D(Vector4 a, Vector4 b, Vector4 c)
        {
            //引用的赋值，所以改变了则传参的数据也会被改变
            /*
            this.A = a;
            this.B = b;
            this.C = c;
            */

            //不用引用赋值，直接new赋值
            this.A = new Vector4(a);
            this.B = new Vector4(b);
            this.C = new Vector4(c);

            this.a = this.A;
            this.b = this.B;
            this.c = this.C;
        }

        //三角形用矩阵的乘法进行变换
        public void Transform(Matrix4x4 m)
        {
            this.a = m.Mul(this.A);
            this.b = m.Mul(this.B);
            this.c = m.Mul(this.C);
        }
        

        //绘制三角形到2d窗口上
        public void Draw(Graphics g)
        {
            g.TranslateTransform(150, 150);
            g.DrawLines(new Pen(Color.Blue,2),this.Get2DPointFArr());
        }

        private PointF[] Get2DPointFArr()
        {
            PointF[] arr = new PointF[4];
            arr[0] = Get2DPointF(this.a);
            arr[1] = Get2DPointF(this.b);
            arr[2] = Get2DPointF(this.c);
            arr[3] = arr[0];
            return arr;
        }

        private PointF Get2DPointF(Vector4 v)
        {
            PointF p = new PointF();
            p.X = (float)(v.x / v.w);
            p.Y = -(float)(v.y / v.w);

            return p;
        }
    }
}
