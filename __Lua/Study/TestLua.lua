



function CreateWrap()
  a = coroutine.wrap(ShowNumber)
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