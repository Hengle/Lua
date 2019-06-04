Shader "UI/Unlit/ParticleAdd" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
    _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
    xMin ("Min X", Float) = -2
    xMax ("Max X", Float) = 2
    yMin ("Min Y", Float) = -2
    yMax ("Max Y", Float) = 2
}

Category {
	    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	    Blend SrcAlpha One
	    Fog { Mode Off }
	    ColorMask RGB
	    Cull Off Lighting Off ZWrite Off

	    SubShader {
		    Pass {
			    CGPROGRAM
			    #pragma vertex vert
			    #pragma fragment frag
			    #pragma multi_compile_particles
			    #pragma multi_compile_fog

			    #include "UnityCG.cginc"

			    sampler2D _MainTex;
			    fixed4 _TintColor;
                float xMin;
                float xMax;
                float yMin;
                float yMax;

			    struct appdata_t {
				    float4 vertex : POSITION;
				    fixed4 color : COLOR;
				    float2 texcoord : TEXCOORD0;
			    };

			    struct v2f {
				    float4 vertex : SV_POSITION;
				    fixed4 color : COLOR;
				    float2 texcoord : TEXCOORD0;
                    float2 locPos : TEXCOORD1;
            	    #ifdef SOFTPARTICLES_ON
				    float4 projPos : TEXCOORD2;
				    #endif
			    };
			
			    float4 _MainTex_ST;

			    v2f vert (appdata_t v)
			    {
				    v2f o;
				    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				    o.color = v.color;
				    o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
            	    o.locPos = v.vertex.xy;
                    #ifdef SOFTPARTICLES_ON
				    o.projPos = ComputeScreenPos (o.vertex);
				    COMPUTE_EYEDEPTH(o.projPos.z);
				    #endif
				    return o;
			    }

			    sampler2D_float _CameraDepthTexture;
			    float _InvFade;
			
			    fixed4 frag (v2f i) : SV_Target
			    {
				    #ifdef SOFTPARTICLES_ON
				    float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				    float partZ = i.projPos.z;
				    float fade = saturate (_InvFade * (sceneZ-partZ));
				    i.color.a *= fade;
				    #endif
				    fixed4 col = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
                    col.a *= (i.locPos.x >= xMin);
                    col.a *= (i.locPos.x <= xMax);
                    col.a *= (i.locPos.y >= yMin);
                    col.a *= (i.locPos.y <= yMax);
				    return col;
			    }
			    ENDCG 
		    }
	    }
    }
}
