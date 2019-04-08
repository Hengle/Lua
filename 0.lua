
require "bit"
a = 2

b = 1
b_1 = bit.rshift(a,0)

c = bit.band(b,b_1)

print(a.."  /  ".. b .."  /  ".. b_1 .."  /  "..  c .."  /  ")