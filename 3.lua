

--  逻辑运算符 and(与) or(或) ~(非) 
--  快速记忆：a&&b 等价于a and b，a || b 等价于 a or b；
--  即是a为true，第一个取决于b；a为false，第二个取决于b
--  三目运算a？b：c；等价于a and b or c

--  优先级3.5：^  | not - |+ - | .. | < > <= >= ~= ==| and |or


--  表的构造方式

-- 第一种,以下三种方式都是一样的
print("---1---")
a = {x=0,y=0};
--a = {['x']=0,['y']=0};
--a = {["x"]=0,["y"]=0};
print(a.x,a.y);  -- 0 0
print(a[x],a[y]);-- nil nil 这种方式x是一个表
print(a['x'],a['y']);--0 0
print(a["x"],a["y"]);--0 0

--如果是以表作为key则只能用[]
key = {name = 11}
a[key] = 333
print(a[key])

---------------------
print("---key name ---")
for key, value in pairs(a) do
    if type(key) == "table" then
       print(key,key.name .. ":" .. value);
	else
	   print(key,value)
	end
end

-- 第二种
print("---2 ---")
b = {"red","green","blue"};
--b = {[1]="red",[2]="green",[3]="blue"};
print(b[1],b[2],b[3]);


-- 总结，表的构造默认序列是从1开始，如果指定了需要则用指定序号来标记该序号的内容
-- 在构造函数中域分隔符号逗号“，”可以用分号“；”替代，通常我们使用分号用来分割不同类型的表元素

c ={"red","green","blue";["x"]=20,["y"]=30};
