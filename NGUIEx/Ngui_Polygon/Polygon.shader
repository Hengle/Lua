Shader "Unlit/Polygon"
{
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
        _SmoothDelta ("SmoothDelta", Float) = 0.01
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
            ZWrite Off
            Fog { Mode Off }
            Offset -1, -1
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _SmoothDelta;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };
    
            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                half2 uv : TEXCOORD0;
            };
    
            v2f o;

            v2f vert (appdata_t v)
            {
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.color = v.color;
                o.uv = v.texcoord;
                return o;
            }
                
            fixed4 frag (v2f IN) : COLOR
            {
                float3 c = IN.color.rgb;

                float a = IN.color.a;

                float smoothValue = 0;
                float delta = 1.3 * fwidth(IN.uv.x);
                float v = IN.uv.x + delta;

                //if(v >= 1.0f){
                //    smoothValue = (v -1) / delta;
                //}
                smoothValue = step(1.0, v) * (v -1) / delta;
                a = smoothstep(1.0, 0.0f, smoothValue) * a;

                return float4(c, a);
            }
            ENDCG
        }
    }

    //备胎设为Unity自带的普通漫反射  
    Fallback" Diffuse "  
}