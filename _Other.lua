

local a  ={10,2,3,x= 3,4}
local b  ={10,2,3,4}

table.insert(a,7,343) --- 不带位置就是默认最后插入
table.insert(b,5,343)
--print(#a)
--print(a[6],a[7])

function sortfunc(m,p)
  return m > p
end  
 
--- table.sort是排序函数，它要求要排序的目标table的必须是从1到n连续的，即中间不能有nil。,且排序函数不能使用等号=
--- table排序必须是连续的，遇到nil后面的则不排序
table.sort(a,sortfunc)
table.sort(b,sortfunc)

function PriAll(tab)
print (tab)
for i,v in pairs(tab) do
 
 if type(v) == "table" then 
    for m,n in pairs(v) do
	   print("table--",m,n)
    end
 end	

 print(i,v)
end
end

---[[
PriAll(a)
PriAll(b)
--]]


m = "bbb"
m = string.format("${%s}", 22);
print(m)

---array--
print("-----Arry-----")

Ary = {3,4 }  -- 3被下面的重置了
Ary[1] = {"a","b"}
PriAll(Ary)


---test 200 local in one func ----
print("-----test 200 local in one func-----")
function TestLocalNum()
   local loctable = {}
   
   for i=0,200,1 do
      print("local " .. "a" .. i .. "=" .. i) -- 输出粘贴可测试会出错
   end 
return loctable
end

--TestLocalNum()  

   

















