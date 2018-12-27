

-- lua 用一个名为environment的表来保存所有的全局变量
-- lua 将环境本身存储在一个全局变量_G中，_G._G等于G

print("--14.0--")

function PG()
  for n in pairs(_G) do print(n) end
end

--PG()

--14.1
print("--14.1--")


-- 因为环境是一个普通的表，所以可以这样定义全局变量赋值
_G["p"] = 5 -- 等于  p = 5 
--PG() --会多一个p

--14.2
print("--14.2--")

--[[
 -- 在这种情况下需要使用rawset来声明新的变量，可以绕过metamethod
 -- declare一定要在setmetatable之前，因为它也是在访问_G
function declare (name,initval)      -- 参数就是名字和值
    rawset(_G,name,initval or false) -- 默认的参数为false，主要是防止为nil
end

-- lua所有的全局变量都保存在一个普通的表中，我们可以使用metatables来改变访问全局变量的行为
setmetatable(_G,{
    __newindex = function(_,n) --注意是两个下划线
	   error("attempt to write to undeclared variable" .. n , 2)
	end,
	
	__index = function(_,n) 
	   error("attempt to read undeclared variable" .. n,2)
	end,})


 -- aaa = 1    --会报错1，因为是新的全局变量
 -- print(aaa) --会报错2，因为也不能读取未定义的变量
 
declare ("a",1); -- 也可以不用()括号，declare "a"
print(a)

a= 2
print(a)

-- 但是这个时候测试一个变量是否存在，就不能简单的比较他是否为nil了。nil将抛出错误
-- 使用rawget

--print(b==nil) --出错

if rawget(_G,"b") == nil then
   print("__Yes")
else
   print("__NO")
end

--]]

-- 如果需要允许变量可以为nil,需要创建一个辅助表用来保存所有已经声明的变量的名字
-- 不管什么时候metamethod被调用的时候，他会检查这张辅助表看变量是否已经存在


package.path = package.path .. ";E:/__Lua/Study/?.lua"
require("Globaltable") -- 编译并执行

declare "b"
print(b)--nil


 
 
 
 