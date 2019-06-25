Shader "Sbin/l_35"
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

		float _R;
		float _OX;

		struct v2f
		{
			float4 pos:POSITION;
			fixed4 color : COLOR;
		};

		v2f vert(appdata_base v)
		{
			//转换到世界空间
			float4 wpos = mul(_Object2World, v.vertex);

			float2 xy = v.vertex.xz;
			float2 _x = wpos.x - float2(_OX, 0);
			float2 _y = wpos.z;
			float d = _R - sqrt((_x*_x + _y*_y));
			//float d = _R - length(xy-float2(_OX,0));
			
			d = d < 0 ? 0 : d;
			float height = 1;
			float4 uppos = float4(v.vertex.x, height*d, v.vertex.z, v.vertex.w);

			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, uppos);
			o.color = fixed4(uppos.y, uppos.y, uppos.y,1);

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
