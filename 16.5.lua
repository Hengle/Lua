

-----   Single-Method

package.path = package.path .. ";E:/Book/lua/Study/Lua/?.lua"
require("__Util")



function newObject (value)

    return function (action, v)

       if action == "get" then return value

       elseif action == "set" then value = v

       else error("invalid action")

       end

    end

end


d = newObject(0)

print(d("get"))      --> 0

d("set", 10)

print(d("get"))      --> 10

print(d)

-- 每一个对象是用一个单独的闭包，代价比起表来小的多
--优势：虽然没有继承，但有私有性，即是访问对象状态的唯一方式是通过它的内部方法

 





















