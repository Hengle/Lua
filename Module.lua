


 --------- lua提供的module函数----------
 
 
-- 1  module("name")

-- 等同于下列语句

local modname = "name"  --定义模块名
local M = {}            -- 定义用于返回的模块表
_G[modname] = M         --将模块加入到全局表中
package.loaded[modname] = M --将模块表加入到package.loaded中，防止多次加载
setfenv(1,M) --将模块表设置为函数表，这使得模块中的所有操作是以在模块表中的，这样定义函数直接定义在模块表中

-- 注解: 当前的chunk改变了环境，这个chunk所有在改变之后定义的函数都共享相同的环境，都会受到影响。
-- 设置了环境为M的表，那么此后的所有声明就都在这个M的环境，这就是Modle的关键所在