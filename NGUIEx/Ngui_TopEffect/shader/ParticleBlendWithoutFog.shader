Shader "SoftStar/Particle/blend" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	[Toggle(_OpenFog)] _OpenFog("_OpenFog", Int) = 0
}

Category {
	    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	    Blend SrcAlpha OneMinusSrcAlpha 



	    Fog { Mode Off }
	    ColorMask RGBA
	    Cull Off Lighting Off ZWrite Off

	    SubShader {
		    Pass {
			    CGPROGRAM
			    #pragma vertex vert
			    #pragma fragment frag
			    #pragma multi_compile_particles
				#pragma shader_feature _OpenFog

			    #include "UnityCG.cginc"

			    sampler2D _MainTex;
			    fixed4 _TintColor;

			    struct appdata_t {
				    float4 vertex : POSITION;
				    fixed4 color : COLOR;
				    half2 texcoord : TEXCOORD0;
			    };

			    struct v2f {
				    float4 vertex : SV_POSITION;
				    fixed4 color : COLOR;
				    half3 texcoord : TEXCOORD0;
			    };
			
			    float4 _MainTex_ST;

			    v2f vert (appdata_t v)
			    {
				    v2f o;
				    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				    o.color = v.color;
				    o.texcoord.xy = TRANSFORM_TEX(v.texcoord,_MainTex);
					o.texcoord.z = 0;
					#if _OpenFog
					half z = unity_FogParams.x * o.vertex.z;
					half fogFactor = saturate( exp( -z * z));
					o.texcoord.z = fogFactor;
					#endif
				    return o;
			    }
			
			    fixed4 frag (v2f i) : SV_Target
			    {
				    fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
					#if _OpenFog
					col.rgb = lerp( unity_FogColor, col.rgb, i.texcoord.z);
					#endif
				    return col;
			    }
			    ENDCG 
		    }
	    }
    }
}
