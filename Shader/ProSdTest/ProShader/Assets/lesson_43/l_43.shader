Shader "Sbin/l_43_diffuse"
{
	SubShader
	{
	pass
	{
		//支持顶点光照
		tags{"LightMode" = "ForwardBase"}

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#include "unitycg.cginc"
		#include "lighting.cginc"

		struct v2f
		{
			float4 pos:POSITION;
			fixed4 color : COLOR;
		};

		v2f vert(appdata_base v)
		{
	
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			//o.color = fixed4(0, 0, 1, 1);
			

			//光向量需要旋转到模型空间再来计算
			float3 N = normalize(v.normal);
			float3 L = normalize(_WorldSpaceLightPos0);

			//1 如果非等比缩放则方向量的方向是不正确的
			//L = mul(_World2Object, float4(L, 0)).xyz;

			//2 非等比缩放，模型到世界矩阵的转置
			N = mul(float4(N, 0), _World2Object).xyz;
			N = normalize(N);



			float ndotl = saturate(dot(N, L));
			o.color = _LightColor0 * ndotl;
			return o;
		}

		fixed4 frag(v2f IN):COLOR 
		{
			return IN.color + UNITY_LIGHTMODEL_AMBIENT;
		}

		ENDCG
	}
	}
}
