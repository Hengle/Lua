

-- 15.1
print("--15.1--")
-- package命名空间避免命名冲突
-- lua虽然一直都用表来实现package,但也有其他不同的方法实现package

-- 可以使用一个局部变量然后将这个变量赋值给最终的包

local p = {}

function p.new1(r,i)
    return {r=r,i=i}
end

complex = p  -- package 是complex，p可以实现多个 function p.x

--15.2
print("--15.2--15.3--")

-- 私有变量，lua中就是使用局部变量
-- 缺点是访问同一个package内的其他公有的实体写法冗余，必须加上前缀P.

-- 有一个有趣的方法可以立刻解决这两个问题。
-- 我们可以将package内的所有函数都声明为局部的，最后将他们放在最终的表中

local function new2(r,i)  -- local function p.new 会报错不允许这种声明
    return {r=r,i=i}
end

complex.new = new2 -- new的功能等于new1

print(complex.new(1,2).r)
print(complex.new1(1,2).r)


--_REQUIREDNAME变量来重命名
-- 记住，当require加载一个文件的时候，它定义了一个变量来表示虚拟的文件名。这个变量就是__REQUIREDNAME
-- 因此，在你的package中可以这样写

local P = {}      -- package
if _REQUIREDNAME == nil then
    complex = P
else
    _G[_REQUIREDNAME] = P

end






--15.4
print("--15.4--")

-- setfenv 改变一个函数的环境，接受函数和新环境作为参数
-- 除了使用函数本身，还可以指定一个数字表示栈顶的活动函数
-- 1 代表当前函数，2 代表调用当前函数的函数
 
 
 --[[
f = 1
setfenv(1,{fg = _G})
--print(f) --会报错因为改变了当前的环境,环境就是表，设置为只能访问该表中的域，所以不能访问_G了

fg.print(f) -- nil
fg.print(fg.f)-- 1

--]]


package.path = package.path .. ";E:/__Lua/Study/?.lua"
require("Package")

print(pcomp)
c1 = pcomp.new(2,3)
c2 = pcomp.new(20,30)
c3 = pcomp.add(c1,c2)
print(c1.r,c2.r,c3.r)


-- 建议:
-- 1 私有成员最好在一个文件内，公有的则不是必须
-- 2 在package外部经常使用某个函数，最好给他们定义一个局部变量名
-- 3 不想一遍一遍重写package名，用一个短的局部local变量来表示
-- 4 可以写一个函数拆开package，将名字放在_G中
-- 5 如果担心打开package有命名冲突，则在赋值前检查_G中是否存在





-- 这个return语句并非必需的，因为package已经赋值给全局变量complex了。
-- 但是，我们认为package打开的时候返回本身是一个很好的习惯。
-- 额外的返回语句并不会花费什么代价，并且提供了另一种操作package的可选方式
return complex
