

-- 11.1 数组

-- 数组下标可以根据需要，从任意值开始
-- 然而习惯上，Lua的下标从1开始。Lua的标准库遵循此惯例，
-- 因此你的数组下标必须也是从1开始，才可以使用标准库的函数。

a = {}
for i=-5, 5 do
    a[i] = 0
end

for i,v in pairs(a) do
 print(i,v)
end




-- 11.2 矩阵和多维数组
print("--11.2--")

N = 2
M = 2

-- 比较冗余的创建，因为每一行必须显示创建一个table
mt = {}           -- create the matrix
for i=1,N do
    mt[i] = {}    -- create a new row
    for j=1,M do
       mt[i][j] = 0
    end
end

-- 表示矩阵的另一方法，是将行和列组合起来
mt2 ={}
for i=1,N do
    for j=1,M do
       mt2[i*M + j] = 0
    end
end



-- 11.3 链表
-- Lua中用tables很容易实现链表，每一个节点是一个table，指针是这个表的一个域（field），并且指向另一个节点（table）。
-- 例如，要实现一个只有两个域：值和指针的基本链表
lista ={value = 2}
listb ={value = 3}
listc ={value = 4}

lista.next = listb
listb.next = listc
listc.next = listd

--遍历上面的list
local l = lista 
while l do
   print(l,l.value)
   l = l.next
   print(l)
end

-- Lua中在很少情况下才需要这些数据结构，因为通常情况下有更简单的方式来替换链表。
-- 比如，我们可以用一个非常大的数组来表示栈，其中一个域n指向栈顶。



-- 11.4 队列和双向队列
print("---- list ----")
package.path = package.path .. ";E:/__Lua/Study/?.lua"
require("ListList") -- 注意如果是List就跟文件内表名一样了就不行


--fl = loadfile("E:/__Lua/Study/List.lua")
--fl()

f = List.new();
List.pushright(f,10);
List.pushright(f,20);
List.pushright(f,30);

for i,v in pairs(f) do
   print(i,v)
end

-- 11.5
print("---- 11.5 ----")


-- 11.6 字符串缓冲
print("---- 11.6 ----")

-- .. 链接符号拼接小的为大字符串比如从文件中逐行读入再拼，效率很低
-- lua提供了io.read(*all)选项，它的读取飞速

-- 为什么这样呢？Lua使用真正的垃圾收集算法，但他发现程序使用太多的内存他就会遍历他所有的数据结构去释放垃圾数据，
-- 一般情况下，这个算法有很好的性能，Lua的快并非偶然的

-- java提供StringBuffer来改善这种情况
-- 老算法：我们最初的算法通过将循环每一行的字符串连接到老串上来解决问题，新的算法避免如此：它连接两个小串成为一个稍微大的串，然后连接稍微大的串成更大的串。。。
-- 新算法；用一个栈，底部保存最大的字符串，小的串入栈，类似汉诺塔













