

local function printall(f)
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


------------------------------------------------------------------------------------------------------------------------
local tabl = {"a","a",1,2,"b"}

-- 当顺序删除时候，后面自动往前填补，所以会出现跳格
-- 做删除操作需要从后往前删除，这样前面没有删除的序号是不变动的

local function TableRemove(tb,conditionfunc)
    if tb ~= nil then
	   for i = #tb,1,-1 do
	      if conditionfunc(tb[i]) then  
		     table.remove(tb,i)
		   end
		end
	end
end


TableRemove(tabl,function(param) 
     return (param == "a")
	 end
    )

printall(tabl)






