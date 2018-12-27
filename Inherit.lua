

--

BankAccount = {balance = 0}

function BankAccount:new(o)
   o = o or {}
   setmetatable(o,self) -- 更改self就是多重继承，在一个表内去搜索，16.3
   self.__index = self --注意是两个下划线
   return o
end

function BankAccount:deposit(v)
   self.balance = self.balance + v
   print(self.balance)
end

function BankAccount:withdraw(v)
   if v>self.balance then
      error " > funds"
   end
   self.balance = self.balance - v
end


