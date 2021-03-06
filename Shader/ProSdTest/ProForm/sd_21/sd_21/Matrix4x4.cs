﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sd_21
{
    class Matrix4x4
    {
        private double[,] pts;


        //4行4列
        public Matrix4x4()
        {
            pts = new double[4, 4];
        }

        //索引器
        public double this[int i,int j]
        {
            get
            {
                //为了和数组对应0开始
                return pts[i-1, j-1];
            }
            set
            {
                pts[i - 1, j - 1] = value;
            }
        }

        //乘法(右乘)
        public Matrix4x4 Mul(Matrix4x4 m)
        {
            Matrix4x4 newM = new Matrix4x4();

            for(int w= 1;w<=4;w++)
            {
                for(int h = 1; h <= 4; h++)
                {
                    for(int n = 1; n <= 4; n++)
                    {
                        newM[w, h] += this[w, n] * m[n, h];
                    }
                }
            }

            return newM;
        }

        //乘
        public Vector4 Mul(Vector4 v)
        {
            Vector4 newV = new Vector4();
            newV.x = v.x * this[1, 1] + v.y * this[2, 1] + v.z * this[3, 1] + v.w * this[4, 1];
            newV.y = v.x * this[1, 2] + v.y * this[2, 2] + v.z * this[3, 2] + v.w * this[4, 2];
            newV.z = v.x * this[1, 3] + v.y * this[2, 3] + v.z * this[3, 3] + v.w * this[4, 3];
            newV.w = v.x * this[1, 4] + v.y * this[2, 4] + v.z * this[3, 4] + v.w * this[4, 4];

            return newV;
        }

        //转置
        public Matrix4x4 Transpose()
        {
            Matrix4x4 t = new Matrix4x4();
            for(int i=1;i<=4;i++)
            {
                for(int j=1;j<=4;j++)
                {
                    t[i, j] = this[j, i];
                }
            }

            return t;
        }
    }
}
