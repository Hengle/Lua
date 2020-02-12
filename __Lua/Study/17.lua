
package.path = package.path .. ";E:/Book/lua/Study/Lua/?.lua"
require("__Util")

-- weak

-- 1 任何在全局变量中声明的对象，都不是Lua认为的垃圾，即使你的程序中根本没有用到他们，不用了需要赋值nil，在使用全局变量之前也记得nil一次
--2 Weak表是一种用来告诉Lua一个引用不应该防止对象被回收的机制.一个weak引用是指一个不被Lua认为是垃圾的对象的引用
--3 表的weak性由他的metatable的__mode域来指定的
--4 要注意，只有对象才可以从一个weak table中被收集

------------------------------------------
b = {}
--setmetatable(b,{__mode = "k"})

key1 = {name = "kk11"}
b[key1] = 1


key2 = {name = "kk22"}
b[key2] = 2

key1 = nil
key2 = nil

collectgarbage()-- 强制进行一次垃圾收集

for key, value in pairs(b) do

    print(key,key.name .. ":" .. value);

end

------------------------------------------
--1 
-- 变量与值：Lua是一个dynamically typedlanguage，也就是说在Lua中，变量没有类型，它可以是任何东西，而值有类型，
-- 所以Lua中没有变量类型定义这种东西。另外，Lua中所有的值都是第一类值(first-class values)。

--2
-- Lua有8种基本类型：nil、boolean、number、string、function、userdata、thread、table。其中Nil就是nil变量的类型，
--nil的主要用途就是一个所有类型之外的类型，用于区别其他7中基本类型。

--3
--对象objects:Tables、functins、threads、userdata。对于这几种值类型，其变量皆为引用类型（变量本身不存储类型数据，而是指向它们）。
--赋值、参数传递、函数返回等都操作的是这些值的引用，并不产生任何copy行为

--4 
--weak table的定义--弱引用
--弱表的使用就是使用弱引用，很多程度上是对内存的控制。

--5 
--weak表是一个表，它拥有metatable，并且metatable定义了__mode字段；
--weak表中的引用是弱引用(weakreference)，弱引用不会导致对象的引用计数变化。
--换言之，如果一个对象只有弱引用指向它，那么gc会自动回收该对象的内存。

--6
--_mode字段可以取以下三个值：k、v、kv。k表示table.key是weak的，也就是table的keys能够被自动gc；
--v表示table.value是weak的，也就是table的values能被自动gc；kv就是二者的组合。
--任何情况下，只要key和value中的一个被gc，那么这个key-valuepair就被从表中移除了

--7  weak的使用
--在编程环境中，有时你并不确定手动给一个键值赋nil的时机，而是需要等所有使用者用完以后进行释放，在释放以前，是可以访问这个键值对的。
--这种时候，weak表就派上用场了





















