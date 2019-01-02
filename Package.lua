


-- 还有一种更方便的方式，改变package主chunk的环境，那么这个package创建的所有函数都共享这个新环境

-- 当你创建一个新的函数时，他从创建他的函数继承了环境变量。
-- 所以，如果一个chunk改变了他自己的环境，这个chunk所有在改变之后定义的函数都共享相同的环境，都会受到影响

local p = {}
pcomp = p
setfenv(1,p)  -- 14.3详细将了setfenev的使用，参数2：指定新的环境，参数1：数字1代表当前函数，数字2代表调用当前函数的函数

function new (r,i)  --环境自动转换为p.new
   return {r=r,i=i}
end

function add(c,d)
   return new(c.r+d.r,c.i+d.i)
end



return pcomp