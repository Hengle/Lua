using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sd_21
{
    class Triangle3D
    {
        private Vector4 a, b, c; //变换后的值

        private float dot;  //法向量与光向量的点积

        private bool cullback;   //是否是背面剔除

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

            dot = 0.1f;
        }

        //三角形用矩阵的乘法进行变换
        public void Transform(Matrix4x4 m)
        {
            this.a = m.Mul(this.A);
            this.b = m.Mul(this.B);
            this.c = m.Mul(this.C);
        }

        //计算法向量
        public void CalculateLighting(Matrix4x4 _object2world,Vector4 L)
        {
            //从模型到世界
            this.Transform(_object2world);
            Vector4 U = this.b - this.a;
            Vector4 V = this.c - this.a;
            Vector4 normal = U.Cross(V);
            dot = normal.Normalized.Dot(L.Normalized);

            //限定到0-1的范围内
            //dot = Math.Max(0, dot);
            //dot = Math.Min(dot, 1);
            dot = dot < 0 ? 0.1f : dot;
            dot = dot > 1 ? 0.9f : dot;

            //背面剔除(视向量总是z的负方向)
            Vector4 e = new Vector4(0, 0, -1, 0);
            cullback = e.Normalized.Dot(e) < 0 ? true : false;
      

        }
        

        //绘制三角形到2d窗口上
        public void Draw(Graphics g)
        {
            //g.TranslateTransform(150, 150);

            //描边
            g.DrawLines(new Pen(Color.Red,2),this.Get2DPointFArr());

            //填充(只有不剔除才绘制)
            if (!cullback)
            {
                GraphicsPath path = new GraphicsPath();
                path.AddLines(this.Get2DPointFArr());

                //int r = (int)(255 * dot) + 55;
                //Color color = Color.FromArgb(r, r, r);
                //Brush br = new SolidBrush(color);
                //g.FillPath(br, path);

                g.FillPath(Brushes.Green, path);
                
            }

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
