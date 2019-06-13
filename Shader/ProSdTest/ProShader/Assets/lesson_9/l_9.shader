Shader "Sbin/l_9" {
	
	SubShader
	{
	pass
	{
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
			
	    #define MACROFL FL4(fl4.xy,fl3.zy)
		typedef float4 FL4; 

		void vert(in float2 objPos:POSITION,out float4 pos : POSITION,out float4 col : COLOR)
		{
			//如果顶点需要输出position语义必须是float4
			pos = float4(objPos,0,1); //4阶向量,函数是需要；结尾的
			//col = float4(0, 0, 1, 1);
			col = pos;
		}

		void frag(in float4 pos:POSITION,inout float4 col : COLOR) //inout顶点程序的输入也同时输出
		{
			//赋值
			fixed2 fl2 = fixed2(1, 1);
			fixed3 fl3 = fixed3(0.5, 1, 0.5);
			fixed4 fl4 = fixed4(1, 0.5, 0, 1);
			//float4 ft = float4(fl4.xy,fl3.zy);
			//FL4 ft = MACROFL;
			//col = ft;

			//矩阵
			//float2x4 M2x4 = { fl4,{0,1,0,1} };
			//col = float4(M2x4[1]);
			//col = fixed4(M2x4[0]);

			//数组没有swizzle，数组只能使用角标，
			float arr[4]={1,0.5,0.5,1};
			col = float4(arr[0], arr[1], arr[2], arr[3]);
		}

		ENDCG
	}
	}
}
