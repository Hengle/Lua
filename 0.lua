
--[[
require "bit"
a = 2

b = 1
b_1 = bit.rshift(a,0)

c = bit.band(b,b_1)

print(a.."  /  ".. b .."  /  ".. b_1 .."  /  "..  c .."  /  ")
--]]

--[[
c = 10.12345
local a , b = math.modf(c);

print(a,"---",b)


local f = math.pow(10, 2)
local d = b *f
local d1,d2 = math.modf(d)
local r = a .. "." .. d1
print(d1,"---",d2,"---",r)
--]]

local c = math.mod(20,101)
print(c)