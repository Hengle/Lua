



a = 1e-5
b = tonumber(a)
c_1 = math.pi
c_2 = math.sin(c_1)
print(c_1 .. "---" .. c_2 )

p = 9.1
p_1 = math.ceil(p)
print("---" .. p_1)
--[[
a = {}
a[1]={}
a[1].Ha = 3
a[1].Ne = 4

for i,v in ipairs(a) do

print(i,v) 

end

print(a[1].Ha)
print(a[1].Ne)

--]]


a = {}

a[5] = 3
a[9] = 10


for i,v in pairs(a) do
local m = v%2
print (i,v,m)

end


local a1 = math.ceil(9.1)
local a2 = math.ceil(9.0)
local a3 = math.ceil(9.9)
local a4 = math.ceil(9.5)

print(a1,a2,a3,a4)