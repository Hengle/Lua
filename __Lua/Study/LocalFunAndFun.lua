

-- һ�㶼���local func�����ļ��Ŀ�ͷ��

-- Local function �� function ����
--1 ʹ��function�����ĺ���Ϊȫ�ֺ������ڱ�����ʱ���Բ�����Ϊ������˳����Ҳ��� 
--2 ʹ��local function�����ĺ���Ϊ�ֲ������������õ�ʱ�����Ҫ�������ĺ�������
--3 local functionʹ�ñ���Ҫ����������ʹ��

--[[
function Test()
   test1()
   test2()
end

local function test1()
   print("---test1--")
end

function test2()
   print("--test--")
end



Test() -- attemp to call global 'test1'
--]]


local function test1()
   print("---test1--")
end

function test2()
   print("---test2--")
end


function Test()
   test1()
   test2()
end

Test()