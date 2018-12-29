
-----   多重继承




package.path = package.path .. ";E:/Book/lua/Study/Lua/?.lua"
require("__Util")

--实现的关键在于：将函数用作__index
--记住: 当一个表的metatable存在一个__index函数时，如果Lua调用一个原始表中不存在的函数，Lua将调用这个__index指定的函数
--特别是: 一个类不能同时是其实例的metatable又是自己的metatable。在下面的实现中，我们将一个类作为他的实例的metatable，创建另一个表作为类的metatable

--16.3--
print("--16.3--")

-----------
print("--------more inherit-----")

local function search(k,plist)
     for i=1,table.getn(plist) do 
	    local v = plist[i][k]
		if v then  
		   return v
		end
	  end
end

-- 参数必须是表或者字符串，才支持search查找,只有表和字符串支持[]这个操作
function createClass(...) -- 可变参数，存放在arg = {  存放数据，还有一个n表示数量}
      local c = {} -- new class
	  setmetatable(c,{__index = function(t,k) --注意是两根下划线 ！！！
	      return search(k,arg)
		  end})
		  
	  c.__index = c -- 自身作为元表
	  
	  function c:new(param)
	     param = param or {}
	     setmetatable(param,c) -- 把c作为新类的元表
	     return param
	  end
	  
	  return c
end

print("--------test------")
Base = {}
local _B = Base 

function _B:GetName()
   return self.name
end

function _B:SetName(name)
   self.name = name
end

ChildBase = createClass({},Base)
print("--ChildBase--")
PALL(ChildBase)

onechild = ChildBase:new{name = "bobos"}

print("--onechild--")
PALL(onechild)

print(onechild:GetName())    -- 1先在onechild自己里查找没有找到 2 先在{}里面查找没有找到 -- 3再去Base里查找找到了--就实现了继承

-- 最主要是使用 __index 这个是自身找不到去元表内查找，调用元表的__index,注意是两根下划线





























	 
	  