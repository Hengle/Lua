Shader "SoftStar/Particle/UIParticleClip" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	[Toggle(_OpenFog)] _OpenFog("_OpenFog", Int) = 0
	_ClipRange0 ("_ClipRange0",Vector) = (0.0, 0.0, 0.0, 0.0)
	_ClipArgs0 ("_ClipArgs0",Vector) = (1000.0, 1000.0, 0.0, 0.0)
}

Category {
	    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	    Blend SrcAlpha One
	    Fog { Mode off }

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
				float4 _ClipRange0;
				float4 _ClipArgs0;

			    struct appdata_t {
				    half4 vertex : POSITION;
				    fixed4 color : COLOR;
				    half2 texcoord : TEXCOORD0;
			    };

			    struct v2f {
				    float4 vertex : SV_POSITION;
				    fixed4 color : COLOR;
				    half3 texcoord : TEXCOORD0;
					float2 worldPos : TEXCOORD1;
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
					o.worldPos = v.vertex.xy * _ClipRange0.zw + _ClipRange0.xy;
					
				    return o;
			    }
			
			    fixed4 frag (v2f i) : SV_Target
			    {
					// Softness factor
					float2 factor = (float2(1.0, 1.0) - abs(i.worldPos)) * _ClipArgs0;
				
				    fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord.xy);
					#if _OpenFog
					col.rgb = lerp( unity_FogColor, col.rgb, i.texcoord.z);
					#endif
					
					col.a *= clamp( min(factor.x, factor.y), 0.0, 1.0);
				    return col;
			    }
			    ENDCG 
		    }
	    }
    }
}
