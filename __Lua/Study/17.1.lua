package.path = package.path .. ";E:/Book/lua/Study/Lua/?.lua"
require("__Util")

--使用记忆技术，我们可以将同样的颜色结果存储在同一个table中。为了建立每一种颜色唯一的key

local results = {}

--设置weak为value
setmetatable(results,{__mode = "v"})   


--使用记忆技术，我们可以将同样的颜色结果存储在同一个table中。为了建立每一种颜色唯一的key，我们简单的使用一个分隔符连接颜色索引下标：

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

collectgarbage()-- 强制进行一次垃圾收集
print("---after add---")
PALL(results)
