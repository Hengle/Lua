
for i=1,10,1 do
local j = math.random(1,5)
print (j)
end



-----
local i1 = -1

local i2 = i1 == -1 and false or true

print(i2)


a = {0,1,2,3}



for i,v in pairs(a) do

   if v == 0 then
   table.remove(a,i)
   end

end


for i,v in pairs(a) do

print(i,v) 

end

--[[
a = 10

b = math.modf(a/3)
c = math.fmod(a,3)

print(a,b,c)

for i=2,10,2 do
print (i)
end
--]]

--[[
a = {}
a[1] = 22
a[234] = 3
a[222] = 4
a[2] = 8

table.sort(a)
local siz = #a
print("--" .. siz .. "--")
for i,v in ipairs(a) do
 print(i,v)
end

--]]

--[[
function printtb(tb)

  local s = ""
  for i,v in ipairs(tb) do
     if type(v) == "table" then
	    s = i .. " --- " 
	    for m,n in pairs(v) do
		  s = s .."-key:" .. m .. "-val:" .. n
		end
		print(s)
		s = ""
	end
  end
end


function cmp(a,b)
    if a.e == b.e then
	   if a.q ==  b.q then
	     return a.t <b.t
	   else
	     return a.q > b.q
	   end
	
	else
	   return a.e > b.e
	end
	 
end

par = {}

par[1] = {e =1 ,q =2, t = 3}
par[2] = {e =0 ,q=10, t = 1}
par[3] = {e =0 ,q=4, t = 2}
par[4] = {e =1 ,q=5, t = 6}
par[5] = {e =0 ,q=5, t = 6}

print("--original--")
printtb(par)
print(par[1].q)
print("--qulity--")
table.sort(par, cmp)
printtb(par)

--]]