



-- 1�� ʹ��weak table����Ĭ��values��ÿһ��table����ϵ
local a = {}
setmetatable(a,{__mode = "k"})

local mt = {__index = function(t)
     return a[t]
	 end}
	 
function set_a(t,d) -- d�����Ǳ�����Ǻ�����������
    a[t] = d
	setmetatable(t,mt)
end




--2�� ʹ�ò�ͬ��metatable�����治ͬ��Ĭ��ֵ
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
   
   
-- ���ԣ������ĳ�������ǧ��tables������Щ��ֻ�к��������в�ͬĬ��ֵ�ģ��ڶ��ַ�����Ȼ�����㡣
--��һ���棬���ֻ�к��ٵ�tabels���Թ�����ͬ��Ĭ��vaules����ô�㻹���õ�һ�ַ����ɡ�
