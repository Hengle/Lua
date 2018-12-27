

-- 在一个函数内部使用全局变量名Account是一个不好的习惯，如果全局变量改变就会有问题了
-- 改进：定义方法的时候带上一个参数，这个参数经常是self或者this

Account = {balance = 0}

--[[
function Account.withdraw (v)
    Account.balance = Account.balance - v
end

a = Account
Account = nil
a.withdraw(100.00)   -- ERROR!

--]]

-- 改进：定义方法的时候带上一个参数，这个参数经常是self或者this

function Account.withdraw (self, v)
    self.balance = self.balance - v
end

-- self参数是很多面向对象的要点，只是隐藏了起来
-- lua则使用冒号来隐藏这个参数的声明

-- 重写上面代码

function Account:withdraw(v)
    self.balance = self.balance - v
end

a = Account
Account = nil

a:withdraw(100)
print(a.balance)

a.withdraw(a,100)
print(a.balance)

function printall(f)
for i,v in pairs(f) do
   print(i,v)
end
end

--16.1
print("--16.1--")

package.path = package.path .. ";E:/Book/lua/Study/?.lua"
require("Inherit") -- 编译并执行

SpecialAccount = BankAccount:new()
s = SpecialAccount:new{limit = 1000}
s:deposit(100)












