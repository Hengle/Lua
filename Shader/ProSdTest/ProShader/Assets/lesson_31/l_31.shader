Shader "Sbin/l_31"
{

	SubShader
	{
	pass
	{
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#include "unitycg.cginc"

		float4x4 mvp;
		float4x4 rm;
		float4x4 sm;

		struct v2f
		{
			float4 pos:POSITION;
		};

		v2f vert(appdata_base v)
		{
			v2f o;

			//float4x4 m = mul(UNITY_MATRIX_MVP, rm);
			float4x4 m = mul(UNITY_MATRIX_MVP, sm);
			o.pos = mul(m, v.vertex);

			//o.pos = mul(mvp,v.vertex);
			return o;			
		}

		fixed4 frag(v2f IN):COLOR 
		{
			return fixed4(1,1,1,1);
		}

		ENDCG
	}
	}
}
