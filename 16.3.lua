
-----   ���ؼ̳�




package.path = package.path .. ";E:/Book/lua/Study/Lua/?.lua"
require("__Util")

--ʵ�ֵĹؼ����ڣ�����������__index
--��ס: ��һ�����metatable����һ��__index����ʱ�����Lua����һ��ԭʼ���в����ڵĺ�����Lua���������__indexָ���ĺ���
--�ر���: һ���಻��ͬʱ����ʵ����metatable�����Լ���metatable���������ʵ���У����ǽ�һ������Ϊ����ʵ����metatable��������һ������Ϊ���metatable

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

-- ���������Ǳ�����ַ�������֧��search����,ֻ�б���ַ���֧��[]�������
function createClass(...) -- �ɱ�����������arg = {  ������ݣ�����һ��n��ʾ����}
      local c = {} -- new class
	  setmetatable(c,{__index = function(t,k) --ע���������»��� ������
	      return search(k,arg)
		  end})
		  
	  c.__index = c -- ������ΪԪ��
	  
	  function c:new(param)
	     param = param or {}
	     setmetatable(param,c) -- ��c��Ϊ�����Ԫ��
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

print(onechild:GetName())    -- 1����onechild�Լ������û���ҵ� 2 ����{}�������û���ҵ� -- 3��ȥBase������ҵ���--��ʵ���˼̳�

-- ����Ҫ��ʹ�� __index ����������Ҳ���ȥԪ���ڲ��ң�����Ԫ���__index,ע���������»���





























	 
	  