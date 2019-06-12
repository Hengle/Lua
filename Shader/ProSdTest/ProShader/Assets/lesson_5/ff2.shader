Shader "Custom/ff2" {
	properties
	{
		_Color("Main Color",color) = (1,1,1,1)
		_Ambient("Ambient",color) = (0.3,0.3,0.3,0.3)
		_Specular("Specular",color) = (1,1,1,1)
		_Shininess("Shininess",range(0,8)) = 4
		_Emission("Emission",color) = (1,1,1,1)
		_MainTex("MainTex",2d)=""{}
		_SecondTex("SecondTex",2d) = ""{}
		_Constant("Constant",color) = (1,1,1,0.3)
	}

   SubShader{
		//Tags { "Queue" = "Transparent" }

		pass {
		Blend SrcAlpha OneMinusSrcAlpha

		//color(1,1,1,1)
		//小括号是固定值，中括号代表是一个参数
		//color[_Color]
		material
		{
			diffuse[_Color]       //漫反射
			ambient[_Ambient]     //环境光
			specular[_Specular]   //高光(还必须指定separatespecular镜面高光)
			shininess[_Shininess] //指定高光的区域
			emission[_Emission]   //自发光
		}
		lighting on //光照打开
		separatespecular on//使用高光需要开启

		settexture[_MainTex] //一个texture只能带一个参数，显卡有最大的混合个数，基本两张贴图混合是所有显卡支持的
		{
			//combine是当前纹理乘以顶点光照的结果
			combine texture * primary double// primary是前面计算了材质和光照的结果,即是提高两倍的亮度 quad是四倍
        }

		settexture[_SecondTex] //一个texture只能带一个参数
		{
			constantColor[_Constant]
			combine texture * previous double,texture*constant// previous是前面所有计算和采样的结果
		}
	 }
	}
}
