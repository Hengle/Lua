


-- not�Ľ��ֻ����false����true

print(not nil)           --> true

print(not false)         --> true

print(not 0)             --> false

print(not not nil)       --> false


-- ȡ��

temp = 10.333

--����ȡ��
temp_1= math.ceil(temp)
print("temp_1: " .. temp_1)

--����ȡ��
temp_2= math.floor(temp)
print("temp_2: " .. temp_2)

--ȡ��
temp_4_1 = math.modf(temp)
print("temp_4_1: " .. temp_4_1)

--ȡС��
temp_4_2 = math.fmod(temp,8) --ȡģ�����С������ 2.33
print("temp_4_2: " .. temp_4_2)
