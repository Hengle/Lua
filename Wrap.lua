

-- --wrap������ʹ��------
print("----------------wrap------------------")

--wrap��Create������
--1 ��ͬ�Ķ��Ǵ���һ��Coroutine
--2 ����1��wrap����ͨ��resumeȥ�õ���һ������ֵ(������Ϣ)
--3 ����2��wrap���������ֱ�ӵ��ú���ת��coroutine����create���ȹ�����Ҫresume����׬��coroutine
--4 ����3��wrap����ʹ��Coroutine.State(Э��)���鿴״̬

print("----------------1------------------")
co = coroutine.wrap (function ()
    print("co")
end)

print(co) -- thread:xx
print(co())-- co

print("----------------2------------------")
co2 = coroutine.wrap(function()
      print("--before yield---")
      print("co",coroutine.yield())
	  print("--after yield---")
	  end)
	  
print(co2)	  --thread:xx
print(co2())  --before yield-- ������print����
print(co2(1,2,3))--co 1,2,3  + --after yield --  ���ú������´�print������ʼִ��


-- �������Ҫ���coroutine��״̬��wrap�ȽϷ���,����Ҳ����


print("---Wrap---06---")

a ={}


function CreateWrap()
  a.func = coroutine.wrap(CallNumber)
end


function CallNumber()
   print("-----start wrap-----")
   local i = 0
   for i=0,10,1 do
      print(ShowNumber(i))
	  coroutine.yield()
   end
   print("----end wrap -----")
end


function ShowNumber(num)
   local a,b = 0,1
   while num>0 do
       a,b = b,a+b
	   num = num -1
   end
   return a,b
end

print("-Print--Wrap---06---")
CreateWrap()
for j=0,10,1 do
  a.func()
end


















