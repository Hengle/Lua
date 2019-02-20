Shader "Unlit/Transparent Color Masked"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_MainTex_Alpha ("Alpha", 2D) = "white" {}
		_Mask ("Alpha (A)", 2D) = "white" {}
	}
	
	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite On
			Fog { Mode Off }
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _Mask;
			float4 _MainTex_ST;

                        sampler2D _MainTex_Alpha;
			fixed4 tex2D_ETC1(sampler2D sa, sampler2D sb,fixed2 v)
			{
				fixed4 col = tex2D(sa,v);
				fixed alp = tex2D(sb,v).r;
				col.a = min(col.a,alp);
				return col;
			}
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				fixed4 color : COLOR;
			};
	
			v2f o;

			v2f vert (appdata_t v)
			{
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = v.texcoord;
				o.texcoord1 = v.texcoord1;
				o.color = v.color;
				return o;
			}
				
			fixed4 frag (v2f IN) : COLOR
			{
				//half4 col = tex2D(_MainTex, IN.texcoord);
				half4 col = tex2D_ETC1(_MainTex,_MainTex_Alpha,IN.texcoord);
				half flag = step(0.02, dot(IN.color, fixed4(1,1,1,0)));
				col.rgb = col.rgb * IN.color.rgb * flag + dot(col.rgb, fixed3(.222,.707, 0.071)) * (1-flag);
				col.a *= tex2D(_Mask, IN.texcoord1).a;
				return col;
			}
			ENDCG
		}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			ColorMask RGB
			AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
}
