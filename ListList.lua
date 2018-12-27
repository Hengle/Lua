


-- 实现List的 insert 和 remove
x = 10
List = {}

function List.new()
   return {first =0 , last = -1}
end


-- push

function List.pushleft(list,value)
   local first = list.first - 1
   list.first = first
   list[first] = value
end

function List.pushright(list,value)
   local last = list.last + 1
   list.last = last
   list[last] = value
end

function List.popleft(list)
   local first = list.first
   if first>list.last then error("list is empty") end
   local value = list[first]
   list[first] = nil
   list.first = first + 1
   return value
end

function List.popright(list)
   local last = list.last
   if list.first > last then error("list is empty") end
   local value = list[last]
   list[last] = nil
   list.last = last -1
   return value
end
 
 
-- 对严格意义上的队列来讲，我们只能调用pushright和popleft,从尾部插入头部弹出
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 