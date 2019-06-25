Shader "Sbin/l_36_niuqu"
{

	properties
	{
		_R("R",range(0,5)) = 1
		_OX("OX",range(-5,5)) = 0
	}

	SubShader
	{
	pass
	{
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#include "unitycg.cginc"

		struct v2f
		{
			float4 pos:POSITION;
			fixed4 color : COLOR;
		};

		v2f vert(appdata_base v)
		{
			float angle = length(v.vertex)*_SinTime.w;

			//绕y旋转矩阵
			//float4x4 m = {
			//  float4(cos(angle),0,sin(angle),0), //1行
			//  float4(0,1,0,0),//2行
			//  float4(-sin(angle),0,cos(angle),0),//3行
			//  float4(0,0,0,1)//4行
			//};
			//v.vertex = mul(m, v.vertex);

			float x = cos(angle)*v.vertex.x + sin(angle)*v.vertex.z;
			float z = cos(angle)*v.vertex.z - sin(angle)*v.vertex.x;
			v.vertex.x = x;
			v.vertex.z = z;

			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = fixed4(0,1,1,1);

			return o;			
		}

		fixed4 frag(v2f IN):COLOR 
		{
			return IN.color;
		}

		ENDCG
	}
	}
}
