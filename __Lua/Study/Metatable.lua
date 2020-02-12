
--13.1
print("--13.1--")

-- Metatable

-- ipaire 和 paire 的区别
-- pairs 可以遍历表中所有的key，并且除了迭代器本身以及遍历表本身还可以返回nil
-- ipairs 则不能返回nil只能返回数字0，如果遇到nil则退出。它只能遍历到表中出现的第一个不是整数的key

Set = {}

function Set.new()
   local set = {} -- lua是大小写敏感的
   for _,l in ipairs(t) do
     set[l] = true
   end
   return set
end

function Set.union(a,b)
   local res = Set.new{}
   for k in pairs(a) do
       res[k] = true
   end
   for k in pairs(b) do
       res[k] = true
   end
   
   return res
end

function Set.intersection(a,b)
   local res = Set.new{} -- 注意这里是{}不是();类似传递一个空表进去
   for k in pairs(a) do
       res[k] = b[k]
   end
   return res
end

-- 打印函数输出
function Set.tostring (set)
    local s = "{"
    local sep = ""
    for e in pairs(set) do
       s = s .. sep .. e
       sep = ", "
    end
    return s .. "}"
end

function Set.print (s)
    print(Set.tostring(s))
end

function Set.printiv(set)
   print("--print i,v----")
   for i,v in pairs(set) do
     print(i,v)
	end
end
   


-- 现在我们需要+执行两个集合的并操作，将所有集合共享一个metatable，
-- 并且为这个metatable添加如何处理相加操作

-- 第一步，我们定义一个普通的表，用来作为metatable。
-- 为避免污染命名空间，我们将其放在set内部
Set.mt = {}

-- 第二步，修改set.new函数，增加一行，创建表的时候同时指定对应的metatable。
function Set.new(t)
   local set ={}
   setmetatable(set,Set.mt) -- set.new创建所有的结合都有相同的metatable了
   for _,l in ipairs(t) do 
      set[l] = true 
   end
   return set
end


s1 = Set.new{10,20,30,40}
s2 = Set.new{30,1}

print(getmetatable(s1))
print(getmetatable(s2))

-- 第三步，给metatable增加_add函数
Set.mt.__add = Set.union --注意是两个下划线

s3 = s1 + s2
Set.print(s3);
Set.printiv(s3)


-- 对于每一个算术运算符，metatable都有对应的域名与其对应，除了__add、__mul外，还有__sub(减)、__div(除)、__unm(负)、__pow(幂)，
-- 我们也可以定义__concat定义连接行为。


-- 定义相乘
Set.mt.__mul = Set.intersection
Set.printiv((s1+s2)*s1)



-- 如果两个操作数有不同的metatable，则lua选择metatable的原则：
-- 如果第一个参数存在带有__add域的metatable,lua使用它作为metamethod，和第二个参数无关
-- 若第二个参数也存在带有__add域的metatable，第一个参数没有，则使用第二个，两个值都没有则报错。例子如下

sa = Set.new{1,2,3}
sb = {2,3,4}
sc = sa+sb
Set.printiv(sc) -- 等于sa

print(sc == sa) -- false两个表不一样，但是里面值是一样的

sd = sb+sa
Set.printiv(sd) -- sd也跟sa值一样

--se = sb+sb --会报错



--13.2
print("--13.2--")


-- metamethods:__eq(等于)，__lt(小于),__le(小于等于)；对于大于 - 不等于 -大于等于 这三个
-- lua是把它们都同等转换为前三个

-- 例如：大多机子无法排列浮点数因为它不是一个数字

Set.mt.__le = function(a,b)  -- 小于等于
   for k in pairs(a) do 
      if not b[k] then
	     return false 
	  end
   end
   return true
 end
 
Set.mt.__lt = function (a,b) -- 小于
   return a <= b and not (b <= a)
end

 
Set.mt.__eq = function(a,b)  -- 等于，通过集合的包含来定义集合相等
    return a <= b and b <= a
end

print("--13.2--metamethods--")
s11 = Set.new{2, 4}
s22 = Set.new{4,2,10}

print(s11 <= s22)          --> true
print(s11 < s22)           --> true
print(s11 >= s11)          --> true
print(s11 > s11)           --> false
print(s11 == s22 * s11)     --> true


-- 与算术运算的metamethods不同，关系元算的metamethods不支持混合类型运算
-- 因为Lua总是认为字符串和数字是不等的，而不去判断它们的值。
-- 仅当两个有共同的metamethod的对象进行相等比较的时候，Lua才会调用对应的metamethod


--13.3
print("--13.3")

-- setmetatable/getmetatable函数也会使用metafield，在这种情况下，可以保护metatables。
-- 假定你想保护你的集合使其使用者既看不到也不能修改metatables。
-- 如果你对metatable设置了__metatable的值，getmetatable将返回这个域的值，而调用setmetatable 将会出错：


--Set.mt.__metatable = "not your business"  --设置了这个则会报错
s1 = Set.new{}
print(getmetatable(s1))     --> not your business

setmetatable(s1, {})
--stdin:1: cannot change protected metatable





   