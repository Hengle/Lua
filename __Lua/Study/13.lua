
-- metatable.lua里有ipairs和pairs的不同,ipair遍历到第一个nil则退出

--a = {2,3,4,x=5,y =6}
  a ={[2]=2,[3]="b0",x=0,z=190}

print("-- pairs --")
for i,v in pairs(a) do
   print(i,v)
end

print("-- ipairs --")
for i,v in ipairs(a) do
   print(i,v)
end

print("--one param for--")
for k in pairs(a) do
   print(k)
end



-- 13 metatables and metamethod
print("--13.1--")

-- lua中每一个表都有其metatable

t= {}
print(getmetatable(t)) -- nil

-- setmetatable改变一个表的metatable
t1 = {}
setmetatable(t,t1)
assert(getmetatable(t) == t1)

-- 任何一个表都可以是其他一个表的metatable，一组相关的表可以共享一个metatable（描述他们共同的行为）。
-- 一个表也可以是自身的metatable（描述其私有行为）。




--13.1
print("-- 13.1--")
package.path = package.path .. ";E:/__Lua/Study/?.lua"
require("Metatable") -- 编译并执行




















