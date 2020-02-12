

-- 一般都会把local func放在文件的开头处

-- Local function 和 function 区别
--1 使用function声明的函数为全局函数，在被引用时可以不会因为声明的顺序而找不到 
--2 使用local function声明的函数为局部函数，在引用的时候必须要在声明的函数后面
--3 local function使用必须要先声明，再使用

--[[
function Test()
   test1()
   test2()
end

local function test1()
   print("---test1--")
end

function test2()
   print("--test--")
end



Test() -- attemp to call global 'test1'
--]]


local function test1()
   print("---test1--")
end

function test2()
   print("---test2--")
end


function Test()
   test1()
   test2()
end

Test()