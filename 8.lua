

-- 8章
print("--8.0--")

-- lua把每一个chunk都作为一个匿名函数处理
-- lua会把代码预编译成中间代码然后再执行
-- dofile：实际上是一个辅助函数，加载并运行
-- loadfile: 编译代码成中间码并且返回编译后的chunk作为一个函数，而不执行代码

function dofile_bobos(filename)
 local f = assert(loadfile(filename));
 return f();
end

-- 如果我们运行一个文件多次的话，loadfile只需要编译一次，而dofile却每次都要编译
-- loadstring与loadfile相似，只不过它不是从文件里读入chunk，而是从一个串中读入
-- loadstring与loadfile,，他们仅仅编译chunk成为自己内部实现的一个匿名函数。通常对他们的误解是他们定义了函数。
-- Lua中的函数定义是发生在运行时的赋值而不是发生在编译时

f = loadstring("i = i + 1")
i = 0;
f();print(i) -- 1
f();print(i) -- 2

-- dofile编译并执行
-- dofile("E:/__Lua/Study/4.lua")

-- loadfile 仅仅编译成内部实现的一个匿名函数，定义却在运行时发生
--[[p = loadfile("E:/__Lua/Study/7.lua") --注意路径正斜杠
p() -- 如果不添这一行定义，则f(2)不会执行，因为还没有定义，必须运行次chuck才被定义
f(2)--]]


-- 8.1
print("--8.1--")

-- require和dofile功能完全一样
-- require会搜索目录加载文件，还会判断是否加载避免重复加载
-- require是加载库更好的函数

-- require关注的问题只有分号和问号,需要把路径加到package.path或package.cpath后面
-- 因为在两个路径都失败后，reqire使用固定路径典型的就是"?;?.lua"

print(package.path)
print(package.cpath)

package.path = package.path .. ";E:/__Lua/Study/?.lua"
p = require("4")
fff()


-- 8.2
print("--8.2--")

-- loadlib函数加载制定的库并连接到Lua





-- 8.3
print("--8.3--")

-- assert,先检查第一个参数，否则第二个参数作为错误信息抛出
assert(0,"0 is true");
--assert(nil,"nil is false"); 

-- print(debug.traceback())




















