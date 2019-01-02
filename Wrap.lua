

-- --wrap函数的使用------
print("----------------wrap------------------")

--wrap和Create的区别
--1 相同的都是创建一个Coroutine
--2 区别1：wrap不会通过resume去得到第一个返回值(错误信息)
--3 区别2：wrap创建完后能直接调用函数转到coroutine，而create是先挂起需要resume才能赚到coroutine
--4 区别3：wrap不能使用Coroutine.State(协程)来查看状态

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
print(co2())  --before yield-- 挂起在print函数
print(co2(1,2,3))--co 1,2,3  + --after yield --  调用函数重新从print函数开始执行


-- 如果不需要检测coroutine的状态，wrap比较方便,调用也方便


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


















