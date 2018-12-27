

--5.1 多返回值
print("---5.1---");

--当函数只有一个参数并且这个参数是字符串或者表构造的时候()可有可无
print("bobos")
print "bobos"



-- 多值返回
function f()
  return "b","c"
end

m,n = f();
print(m,n);


--()则只返回第一个值
print((f())) -- 这个只返回b
j,h = (f());

print(j,h)   -- 返回b，nil


--一个return语句如果使用圆括号将返回值括起来也将导致返回一个值
function f1()
 --return ("b","c"); --会报错error
 return (f());
end

m,n = f1();
print(m,n);


--unpack调用可变参数的函数
arry = {"hello", "ll"};
--string,find()，作用是在一个字符串中找到目标字符串的起始和结束位置（从1开始计数）
print(string.find("hello","ll"));


f2,f3,f4 = unpack(arry);
print(f2,f3,f4); -- hello ll nil
	
--lua中的unpack的实现方式
function unpack__bobos(t, i)

    i = i or 1
    if t[i] then
       return t[i], unpack__bobos(t, i + 1)
    end

end
f2,f3,f4 = unpack__bobos(arry,1);
print(f2,f3,f4);-- hello ll nil


--5.2 可变参数
print("---5.2---");

-- 函数参数列表中使用三点（...）表示函数有可变的参数。Lua将函数的参数放在一个叫arg的表中，
-- 除了参数以外，arg表中还有一个域n表示参数的个数


function g(a,b,...)
  print(a,b,arg[n]);
  print(a,b,arg[1]);
end


g(3); -- 3 nil nil       ----- a=3 b=nil arg={n=0}
g(3,4);-- 3 4 nil        ----- a=3 b=4 arg={n=0}
g(3,4,5,6,7);-- 3 4 nil  ----- a=3 b=4 arg={5,6,7;n=3}



-- 哑元，就是一个下划线
local _,x = string.find("hello","ll"); -- 3,4；但是3被省略了
print(x);


--5.3 命名参数
print("---5.3---");

--Lua的函数参数是和位置相关的，调用时实参会按顺序依次传给形参

--lua把表作为唯一参数来交换表名，给文件重命名
function rename_bobos(arg)
   return os.rename(arg.old,arg.new)
end

rename_bobos {old="temp.lua",new="temp1.lua"};
--rename_bobos({old="temp.lua",new="temp1.lua"}); --上一语句省略了括号



























