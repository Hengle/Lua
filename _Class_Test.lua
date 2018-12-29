
package.path = package.path .. ";E:/Book/lua/Study/Lua/?.lua"

PA = {}
local _PA = PA
function _PA:showall(f)
print("table--" , v)
for i,v in pairs(f) do
   if type(v) == "table" then
      print("table--" , v)
      for m,n in pairs(v) do
	     print(m,n)
	  end
   end
   print(i,v)
end
end


print(PA.showall)

function printall(f)
   PA:showall(f)
end
-- class
local p = {"p",2,3}
print("--class--")
require("_Class")


---new class---
print("--new class--")
newp = class("newp")
printall(newp)
-- clone -- 表包含: 包含名为基类的super表 + __index的元表 + New的function + 两个字段__ctype 和__cname 
print("--clone--")
local pp = clone(p)
printall(pp)

-- inherit
print("--class-inherit--")
local ppp = class("three_p",pp)
ppp[1] = "QQQ"
printall(ppp)

print("--index-super---")
printall(ppp.super) -- super是元表内作爲基類是不變的


print("---class--new--test----")
mbase = class("mbase")
printall(mbase)
mchid = class("mchid",mbase.New()) -- 实际就是调用闭包，来给这个类用元表的方式clone为基类



print("------------------new new----")
-- m = class.New()      -- error:attempt to index global 'class' (a function value)
-- m = class.New("m",n)

m = class("m")
n = class("n",m)
print("------------------m----")
printall(m)
print("------------------n----")
printall(n)












