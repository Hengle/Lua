using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sd_21
{
    class Vector4
    {
        public double x, y, z, w;

        public Vector4() { }

        public Vector4(double x,double y,double z,double w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Vector4(Vector4 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
            this.w = v.w;
        }

        public static Vector4 operator-(Vector4 a,Vector4 b)
        {
            return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z,a.w-b.w);
        }

        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public Vector4 Cross(Vector4 v)
        {
            //a×b =[a2b3 - a3b2，a3b1 - a1b3，a1b2 - a2b1]
            return new Vector4(this.y * v.z - this.z * v.y,
                this.z * v.x - this.x * v.z,
                this.x * v.y - this.y * v.x,
                0);
        }

        public float Dot(Vector4 v)
        {
            return (float)(this.x * v.x + this.y * v.y + this.z * v.z);
        }

        public Vector4 Normalized
        {
            get
            {
                double mod = Math.Sqrt(x*x + y*y + z*z + w*w);
                return new Vector4(x / mod, y / mod, z / mod, w / mod);
            }
        }
    }
}
