


-- 计数
Minnum = 0
Maxnum = 5


function AddNum()
  Minnum = Minnum +1
end

function IsMax()
  return Minnum < Maxnum and true or false; -- and优先级大于or
end

function SetMax(n)
 Minnum = 0
 Maxnum = n
end

-- 过滤器
function receive(product)
  local status,value = coroutine.resume(product)-- 返回第一个是TRUE或者false
  print("product--" , product)
  return value
end

function send(x)
  print("send--" ,x)
  coroutine.yield(x)--挂起
end

function producer() -- 生产者
  return coroutine.create( function()
    while IsMax() do
	  local x = "New Producer"
      print("createproduct--" , x)	  
	  AddNum()
	  send(x)
	end
   end)
end

function filter(product) --过滤器
  return coroutine.create(function()
     local line = 1
	 while IsMax() do
	   local x = receive(product) -- 新的消费者
	   x = string.format("%5d %s",line,x)
	   print("filter--" , x)	
	   send(x)
	   line = line + 1
	 end
	end)
end

function consumer(product) -- 消费者
  while IsMax() do 
  local x = receive(product)
  print("consumer--" , x)
  end
end






