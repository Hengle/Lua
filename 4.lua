

-- lua可以对多个变量同时赋值，x，y = y,x;交换两个值
x = 10; y =20;
print(x,y);

x,y = y,x;
print(x,y);

--变量个数>值的个数，按变量个数补足nil;变量个数<值的个数，多余的值会被忽略
a,b,c = 0,1
print(a,b,c);

a,b,c = 0;
print(a,b,c);


--条件表达式，lua认为false和nil为假
function fff()
    if(0) then
	print("true-.-|||");
	else
	print("false");
	end
end
print(fff());

--有时候为了调试或者其他目的需要在block的中间使用return或者break，
--可以显式的使用do..end来实现：do return end

function foo ()

    do return end        -- OK

end


--for将用exp3作为step从exp1（初始值）到exp2（终止值），执行loop-part。其中exp3可以省略，默认step=1
print("-------------1------------------");
for i=1,10,2 do  -- 1,3,5,7,9
 print(i);
end

print("--------------2-----------------");
for i=0,10,92 do  -- 1
 print(i);
end

print("--------------More-----------------");
-- 泛型for i,v in ipairs(a) do print(v) end
a = {1,2,3,4;x="a",y="b";"bobox";function(x) return x end;}

for i,v in ipairs(a) do -- 除了x下标和y下标的其他全部输出了，函数的是输出的内存位置
 print(i,v);
end







