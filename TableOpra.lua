

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

-- ��˳��ɾ��ʱ�򣬺����Զ���ǰ������Ի��������
-- ��ɾ��������Ҫ�Ӻ���ǰɾ��������ǰ��û��ɾ��������ǲ��䶯��

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






