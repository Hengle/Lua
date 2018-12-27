


-- 还有一种更方便的方式，改变package主chunk的环境，那么这个package创建的所有函数都共享这个新环境

local p = {}
pcomp = p
setfenv(1,p)  -- 14.3详细将了setfenev的使用

function new (r,i)  --环境自动转换为p.new
   return {r=r,i=i}
end

function add(c,d)
   return new(c.r+d.r,c.i+d.i)
end



return pcomp