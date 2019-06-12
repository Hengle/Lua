Shader "Custom/l_7" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5 //浮点值，可计算高光的光泽度
		_Metallic ("Metallic", Range(0,1)) = 0.0 //浮点值，材质的金属光泽
	}
	SubShader {
		Tags { "RenderType"="Opaque" } //渲染类型，不透明的物体
		LOD 200 //层级细节
		
		CGPROGRAM  //代码块，使用cg语法

		// Physically based Standard lighting model, and enable shadows on all light types
		
        #pragma surface surf Standard fullforwardshadows //#pragma编译指令 surface 函数名称 光照模型  其他选项[optional]

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0 //没有指定通常是默认2.0的硬件模型

		

		struct Input {
			float2 uv_MainTex;//纹理坐标必须以uv或uv2开头，此变量对应属性的_MainTex,自动对应sampler2D _MainTex;
		};

		//一 一 对应4个属性，cg类型和shader的类型是不一样，cg对应的参数可参考
		sampler2D _MainTex; //材质变量
		half _Glossiness;//浮点值
		half _Metallic;
		fixed4 _Color;//4维向量

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
