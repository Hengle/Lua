


-----   ˽��
print("--------private-----")



package.path = package.path .. ";E:/Book/lua/Study/Lua/?.lua"
require("__Util")


Bank = {}
local _Bank = Bank

function _Bank:newAccount(init)
   local self = {balance = init}  -- �κδ��ڱ��ڵĶ�ʵ����˽�У�local��ʹ���ܴﵽ������Ŀ��
   
   --- �հ����� withdraw��ȡ
   local withdraw = function(num)
      self.balance = self.balance - num
   end
   
   --�հ�����deposit����
   local deposit = function(num)
      self.balance = self.balance + num
   end
   
   --�հ�����balance����Ŀ
   local getbalance = function()
      return self.balance 
   end
   
   return {putin = deposit,putout = withdraw,get = getbalance}
end


----test --- private-----
one = Bank:newAccount(100)

print(one.get)  -- �ǵ�û�����ž��Ƿ��غ����ĵ�ַ
print(one.get())
one.putin(50)
print(one.get())
one.putout(55)
print(one.get())


--print all ----------
print(" -- table all --")

PALL(one)






















