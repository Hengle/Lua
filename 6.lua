

-- 6 章
print("--6.0--");

-- 第一类值指：在Lua中函数和其他值（数值、字符串）一样，
-- 函数可以被存放在变量中，也可以存放在表中，可以作为函数的参数，还可以作为函数的返回值
-- 词法定界指：嵌套的函数可以访问他外部函数中的变量。这一特性给Lua提供了强大的编程能力。


-- lua中的print其实是一个匿名的函数
a = {p = print,b = print,}
a.p("Hello World")

a.b = math.sin;
a.p(a.b(1)); -- math.sin(1)
sin = a.p;   -- 注意a这个表示从来没有变动的，即是p这个值一直都是print
sin(10, 20); -- 10  20--]]


-- 函数定义实际上是一个赋值语句，将类型为function的变量赋值给一个变量
-- 我们使用fuction(x) ... end和使用{}创建一个表一样
-- 所以 

function foo(x)
  return x;
end
print(foo("2"));


foo_1 = function(x) 
   return x;
end
print(foo_1("2"));

-- 函数可以作为参数传入，这类函数就是高级函数作为第一类值处理
-- 例如sort的排序
network = {

    {name = "grauna",    IP = "210.26.30.34"},

    {name = "arraial",   IP = "210.26.30.23"},

    {name = "lua",       IP = "210.26.23.12"},

    {name = "derain",    IP = "210.26.23.20"},

}

table.sort(network, function (a,b)

    return (a.name > b.name) -- 按字母降序排列

end)

for i,v in ipairs(network) do
  print(i,v,v.name,v.IP); 
end 
   



-- 6.1
print("--6.1--");

-- 再次说明此法界定：当一个函数内部嵌套另一个函数定义时，
-- 内部的函数体可以访问外部的函数的局部变量，这种特征我们称作词法定界
-- 技术上来讲，闭包指值而不是指函数
-- 简单的说，闭包是一个函数以及它的upvalues


function newCounter()

    local i = 0
    return function()     -- anonymous function
       i = i + 1
        return i
    end
end


c1 = newCounter()  -- 相当于这种写法 foo = function()等价于 function foo()
print(c1())  --> 1 -- 1保存于upvalues，因为调用匿名函数的时候i超出了作用范围，所以闭包使用upvalue来保存这个值
print(c1())  --> 2

print(newCounter())  --> 函数地址



-- 闭包可以重定义函数实现很实用
oldSin = math.sin
math.sin = function (x)

    return oldSin(x*math.pi/180)

end

-- 6.2 非全局函数
print("--6.2--");

--1
lib = {}
lib.foo = function (x) return x; end

--2 
lib ={foo = function (x) return x; end,}

--3 
lib = {}
function lib.foo(x) return x;end;



-- 因为lua把chunk当作函数处理，在chunk内可以声明局部函数，词法定界保证里其他函数可以调用此函数


local f= function()
    return 1;
end

local g= function()
   return f()
end

print(f()); -- 1
print(g()); -- 1


--[[ 
--错误
local fact = function (n)
    if n == 0 then
       return 1
    else
       return n*fact(n-1)   -- 会报错没有找到全局变量fact的函数
    end
end
print(fact(5))

--]]

-- 修正：必须在函数前先声明，这样fact(n-1)调用的是一个局部函数
local fact 
fact = function (n)
    if n == 0 then
       return 1
    else
       return n*fact(n-1)   -- 会报错没有找到全局变量fact的函数
    end
end
print(fact(5))




-- 所以上面的f，g最好是如下先定义
local f,g;

f= function()
    return 1;
end

g= function()
   return f()
end

print(f()); -- 1
print(g()); -- 1



-- 6.3 正确的尾调用
print("--6.3--");


-- 尾调用是一种类似在函数结尾的goto调用，当函数最后一个动作是调用另外一个函数时，
-- 我们称这种调用尾调用,上面的 g函数最后调用 f()就是一个尾调用

-- 这种情况下当被调用函数g结束时程序不需要返回到调用者f；
-- 所以尾调用之后程序不需要在栈中保留关于调用者的任何信息。
-- 一些编译器比如Lua解释器利用这种特性在处理尾调用时不使用额外的栈，
-- 我们称这种语言支持正确的尾调用


function foo (n)

    if n > 0 then return foo(n - 1) end -- 注意跟上面的local fact对比，注意差别

end

-- 不是尾调用
function g(x)
 return x;
end

-- 1 不得不丢弃g的返回值
function f (x)
    g(x)
    return
end
print(1);

-- 2 必须做加法
function f (x)   
    return g(x)+1;
end
print(1);

-- 3 必须返回一个值
function f (x)  
    return x or g(x)
end
print(1);

-- 4 必须返回一个值
function f (x) 
    return (g(x));
end
print(1);


-- 尾调用直接可以理解成goto，在状态机的变成领域中调用时非常有用的；
-- 因为状态机要求函数记住每一个状态，状态只在改变时候goto















