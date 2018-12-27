


-- dofile 是用来加载文件并执行的

-- 命令行-l会调用require将会在指定的环境变量路径下搜索文件，lua -i -la -lb 先执行a文件再执行b文件


-- 字符串表示,单引号和双引号是一样的
a = "bobos";
b = 'bobos';
c = [[
bobos
]];

--c这种方式表示的字符串是不识别转义字符的，且第一个换行符被省略

print(a,b,c);



-- lua 会自动在string和number中转换
print("1"+2);
--print("hello"+2); --会报错

-- 虽然可以转换但字符和数字是不同的，10==“10” 这样永远都是错误的，可以使用tonumber或tostring来相互转换
print("10" == 10); --false
print(tonumber("10") == 10); -- true
print("10" == tostring(10)); -- true



-- type可以确定类型
print("--type--")
print(type("1"));
print(type(1));