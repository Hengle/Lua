


-- not的结果只返回false或者true

print(not nil)           --> true

print(not false)         --> true

print(not 0)             --> false

print(not not nil)       --> false


-- 取整

temp = 10.333

--向上取整
temp_1= math.ceil(temp)
print("temp_1: " .. temp_1)

--向下取整
temp_2= math.floor(temp)
print("temp_2: " .. temp_2)

--取整
temp_4_1 = math.modf(temp)
print("temp_4_1: " .. temp_4_1)

--取小数
temp_4_2 = math.fmod(temp,8) --取模过后的小数部分 2.33
print("temp_4_2: " .. temp_4_2)
