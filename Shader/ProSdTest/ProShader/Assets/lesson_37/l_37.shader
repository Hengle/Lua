Shader "Sbin/l_37_bowen"
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
	
			//横向波
			//v.vertex.y += 0.2*sin(v.vertex.x + _Time.y);

			//圆形波
			//v.vertex.y += 0.2*sin(-length(v.vertex.xz) + _Time.y);

			//两个方向叠加后的正弦波
			v.vertex.y += 0.2*sin(v.vertex.x + v.vertex.z + _Time.y);
			v.vertex.y += 0.3*sin(v.vertex.x - v.vertex.z + _Time.w);

			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = fixed4(v.vertex.y, v.vertex.y, v.vertex.y,1);

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
