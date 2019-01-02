

-- 9.1 协同的基础
print("-- 9.1 --")

-- Lua的所有协同函数存放于coroutine table中
-- 协同有三个状态：挂起态（suspended）、运行态（running）、停止态（dead）

-- create的参数是匿名函数,创建出来是默认挂起的状态
co = coroutine.create(function ()
    print("hi")
end)
print(co)     --> thread: 0x8071d98


-- status检查协同的状态
print(coroutine.status(co)) -->suspended

-- 函数coroutine.resume使协同程序由挂起状态变为运行态
coroutine.resume(co)            --> hi

--本例中，协同程序打印出"hi"后，任务完成，便进入终止态：
print(coroutine.status(co))     --> dead


-- yield函数，它可以将正在运行的代码挂起
print("----------------yield------------------")

co = coroutine.create(function ()
    for i=1,2 do
       print("co", i)
       coroutine.yield()
    end
end)

print(coroutine.status(co)) -- suspended

coroutine.resume(co)            --> co   1,使用了yield来挂起
print(coroutine.status(co))     --> suspended
coroutine.resume(co)            --> co   2
print(coroutine.status(co))     --> suspended
coroutine.resume(co)            --> 已经不满足函数条件不会输出
print(coroutine.status(co))     --> dead

coroutine.resume(co) --> 已经结束了不能再次激活dead的协程


-- resume把参数传递给协同的主程序
print("----------------1 resume pass parameter to main program------------------")
co = coroutine.create(function (a,b,c)
    print("co", a,b,c)
end)
coroutine.resume(co, 1, 2, 3)      --> co  1  2  3


-- 数据由yield传给resume。true表明调用成功，true之后的部分，即是yield的参数
print("----------------2 yield pass parameter to resume,true is success------------------")
co = coroutine.create(function (a,b)
    coroutine.yield(a + b, a - b)
end)
print(coroutine.resume(co, 20, 10))    --> true  30  10


-- resume的参数，会被传递给yield。
print("----------------3 resume pass parameter to yield------------------")

co = coroutine.create (function ()
    print("co", coroutine.yield())
end)

print(coroutine.status(co)) --挂起
coroutine.resume(co)   --运行到print挂起
print(coroutine.status(co))   --挂起      
coroutine.resume(co, 4, 5,"rerer")      --> 运行完print函数,co  4  5 -- 4,5就是resume传递给yield的参数,注意输出了co和yield传递的参数后,程序就结束了
print(coroutine.status(co))

-- 以上再重点执行顺序
-- 1 coroutine.create后则挂起状态
-- 2 第一个resume执行到print的第二个参数进入挂起状态，这个时候挂起在print函数，下次进入会重新执行print函数
-- 3 第二个resume传递了参数给yield，继续执行print，并完成了print函数
-- 4 执行完后进入dead状态

--最后一个例子，协同代码结束时的返回值，也会传给resume：
print("----------------4 end parameter can pass to resume ------------------")

co = coroutine.create(function ()
    return 6, 7
end)

print(coroutine.resume(co))     --> true  6  7

-- 所以综上所述：resume可以传递参数给yield，也可以在函数结束的时候把参数传递给resume；注意resume的第一个参数是true(调用成功)或false



-- 9.2 管道和过滤器
print("-- 9.2 --")


-- 过滤器
Filter = loadfile("E:/__Lua/Study/Filter.lua")
Filter()
SetMax(2)

--1种调用
print("-- one call --")
p = producer()
f = filter(p)
consumer(f)


--2种调用
SetMax(5)
print("-- two call --")
consumer(filter(producer()))



-- 9.3 用作迭代器的协同
print("-- 9.3 --")


-- coroutine.wrap 与create相同的是，wrap创建一个协同程序，不同的是wrap不返回协同本身，
-- 而是返回一个函数，当这个函数被调用时将resume协同。wrap中resume协同的时候不会返回错误
-- 代码作为第一个返回结果，一旦有错误发生，将抛出错误

-- 缺点：没有灵活性，没有办法知道wrap所创建的协同的状态，也没有办法检查错误的发生



-- 9.4 非抢占式多线程
print("-- 9.4 --")


-- 用协成来多线程下载，同时下载多个文件，省去tcp连接返回的时间




































