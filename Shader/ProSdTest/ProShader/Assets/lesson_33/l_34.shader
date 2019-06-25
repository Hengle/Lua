Shader "Sbin/l_34"
{

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
			v2f o;

			//投影的奇次坐标
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

			//屏幕
			float x = o.pos.x / o.pos.w;
			if (x <= -1)
			{
				o.color = fixed4(1, 0, 0, 1);
			}
			else if(x>=1)
			{
				o.color = fixed4(0, 0, 1, 1);
			}
			else
			{
				o.color = fixed4(x/2+0.5,x/2+0.5, x / 2 + 0.5,1);
			}

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
