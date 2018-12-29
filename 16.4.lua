


-----   私有
print("--------private-----")



package.path = package.path .. ";E:/Book/lua/Study/Lua/?.lua"
require("__Util")


Bank = {}
local _Bank = Bank

function _Bank:newAccount(init)
   local self = {balance = init}  -- 任何存在表内的都实现了私有，local的使用能达到这样的目的
   
   --- 闭包操作 withdraw，取
   local withdraw = function(num)
      self.balance = self.balance - num
   end
   
   --闭包操作deposit，存
   local deposit = function(num)
      self.balance = self.balance + num
   end
   
   --闭包操作balance，数目
   local getbalance = function()
      return self.balance 
   end
   
   return {putin = deposit,putout = withdraw,get = getbalance}
end


----test --- private-----
one = Bank:newAccount(100)

print(one.get)  -- 记得没有括号就是返回函数的地址
print(one.get())
one.putin(50)
print(one.get())
one.putout(55)
print(one.get())


--print all ----------
print(" -- table all --")

PALL(one)






















