

local a  ={10,2,3,x= 3,4}
local b  ={10,2,3,4}

table.insert(a,7,343) --- ����λ�þ���Ĭ��������
table.insert(b,5,343)
--print(#a)
--print(a[6],a[7])

function sortfunc(m,p)
  return m > p
end  
 
--- table.sort������������Ҫ��Ҫ�����Ŀ��table�ı����Ǵ�1��n�����ģ����м䲻����nil��,������������ʹ�õȺ�=
--- table��������������ģ�����nil�����������
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

Ary = {3,4 }  -- 3�������������
Ary[1] = {"a","b"}
PriAll(Ary)


---test 200 local in one func ----
print("-----test 200 local in one func-----")
function TestLocalNum()
   local loctable = {}
   
   for i=0,200,1 do
      print("local " .. "a" .. i .. "=" .. i) -- ���ճ���ɲ��Ի����
   end 
return loctable
end

--TestLocalNum()  

   

















