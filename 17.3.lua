



-- 1种 使用weak table来将默认values和每一个table相联系
local a = {}
setmetatable(a,{__mode = "k"})

local mt = {__index = function(t)
     return a[t]
	 end}
	 
function set_a(t,d) -- d可以是表可以是函数或者其他
    a[t] = d
	setmetatable(t,mt)
end




--2种 使用不同的metatable来保存不同的默认值
local b = {}
setmetatable(b,{__mode = "v"})

function set_b(t,d)
   local mt = b[d] --
   if mt == nil then  
      mt = {__index = function()
	     return d
		 end}
	  b[d] = mt
	end
	setmetatable(t,mt)
end
   
   
-- 所以，如果你的程序有数千个tables，而这些表只有很少数带有不同默认值的，第二种方法显然更优秀。
--另一方面，如果只有很少的tabels可以共享相同的默认vaules，那么你还是用第一种方法吧。
