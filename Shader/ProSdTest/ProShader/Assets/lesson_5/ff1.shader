Shader "Sbin/ff1" {

	properties
	{
		_Color("Main Color",color) = (1,1,1,1)
		_Ambient("Ambient",color) = (0.3,0.3,0.3,0.3)
		_Specular("Specular",color) = (1,1,1,1)
		_Shininess("Shininess",range(0,8)) = 4
		_Emission("Emission",color) = (1,1,1,1)
	}

	SubShader {

		pass{
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
	   }
	}

}
