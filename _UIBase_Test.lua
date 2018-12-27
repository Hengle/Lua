

package.path = package.path .. ";E:/Book/lua/Study/?.lua"
require("_Class")
require("_UIBase")

-- 测试HandleOpenParam的作用，

-- 基类key ：self.param
-- 基类接口: SetOpenParm()  参数param
-- 基类重写接口:HandleOpenParam()   参数param

-- handleOpenparam ： 如果没有基类则返回，如果有基类则调用基类的HandleOpenParam重写函数
-- ui的左右：就是保存打开ui的次序，这样进战斗后退出来还是之前的顺序



