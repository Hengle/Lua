using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sd_21
{
    public partial class Form1 : Form
    {
        int a;
        Triangle3D t;

        Matrix4x4 m_scale;//缩放
        Matrix4x4 m_rotation;//旋转
        Matrix4x4 m_view;//平移
        Matrix4x4 m_projection;//投影矩阵

        public Form1()
        {
            InitializeComponent();

            //初始化
            m_scale = new Matrix4x4();
            m_scale[1, 1] = 199;
            m_scale[2, 2] = 199;
            m_scale[3, 3] = 199;
            m_scale[4, 4] = 1;


            //平移
            m_view = new Matrix4x4();
            m_view[1, 1] = 1;
            m_view[2, 2] = 1;
            m_view[3, 3] = 1;
            m_view[4, 3] = 199; //z平移
            m_view[4, 4] = 1;
 

            //旋转
            m_rotation = new Matrix4x4();


            //投影
            m_projection = new Matrix4x4();
            m_projection[1, 1] = 1;
            m_projection[2, 2] = 1;
            m_projection[3, 3] = 1;
            m_projection[3, 4] = 1.0 / 199; //投影到小孔的距离
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //只有最后是1是点跟矩阵相乘才有意义
            Vector4 a = new Vector4(0, -0.5, 0, 1); 
            Vector4 b = new Vector4(0.5, 0.5, 0, 1);
            Vector4 c = new Vector4(-0.5, 0.5, 0, 1);
            t = new Triangle3D(a,b,c);

            //缩放
            //t.Transform(m_scale);

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            t.Draw(e.Graphics);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            a += 2;
            
            //弧度
            double angle = a / 360.0 * Math.PI; 
            m_rotation[1, 1] = Math.Cos(angle);
            m_rotation[1, 3] = Math.Sin(angle);
            m_rotation[2, 2] = 1;
            m_rotation[3, 1] = -Math.Sin(angle);
            m_rotation[3, 3] = Math.Cos(angle);
            m_rotation[4, 4] = 1;

            //联合矩阵
            Matrix4x4 m = m_scale.Mul(m_rotation);
            m = m.Mul(m_view);
            m = m.Mul(m_projection);


            //缩放
            //t.Transform(m_scale);
            t.Transform(m);
            this.Invalidate();
        }
    }
}
