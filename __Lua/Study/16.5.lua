

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

-- ÿһ����������һ�������ıհ������۱������С�Ķ�
--���ƣ���Ȼû�м̳У�����˽���ԣ����Ƿ��ʶ���״̬��Ψһ��ʽ��ͨ�������ڲ�����

 





















