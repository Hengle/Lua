package.path = package.path .. ";E:/Book/lua/Study/Lua/?.lua"
require("__Util")

--ʹ�ü��似�������ǿ��Խ�ͬ������ɫ����洢��ͬһ��table�С�Ϊ�˽���ÿһ����ɫΨһ��key

local results = {}

--����weakΪvalue
setmetatable(results,{__mode = "v"})   


--ʹ�ü��似�������ǿ��Խ�ͬ������ɫ����洢��ͬһ��table�С�Ϊ�˽���ÿһ����ɫΨһ��key�����Ǽ򵥵�ʹ��һ���ָ���������ɫ�����±꣺

function createRGB(r,g,b)
   local key = r .. "-" .. g .. "-" .. b
   if results[key] then
      return results[key]
   else
      local newcolor = {red = r,green = g,blue = b}
	  results[key] = newcolor
	  return newcolor
	end
end

print("---before add---")
createRGB(123,212,111)
PALL(results)

collectgarbage()-- ǿ�ƽ���һ�������ռ�
print("---after add---")
PALL(results)
