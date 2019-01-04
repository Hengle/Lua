--[[
               #########                       
              ############                     
              #############                    
             ##  ###########                   
            ###  ###### #####                  
            ### #######   ####                 
           ###  ########## ####                
          ####  ########### ####               
         ####   ###########  #####             
        #####   ### ########   #####           
       #####   ###   ########   ######         
      ######   ###  ###########   ######       
     ######   #### ##############  ######      
    #######  #####################  ######     
    #######  ######################  ######    
   #######  ###### #################  ######   
   #######  ###### ###### #########   ######   
   #######    ##  ######   ######     ######   
   #######        ######    #####     #####    
    ######        #####     #####     ####     
     #####        ####      #####     ###      
      #####       ###        ###      #        
        ###       ###        ###               
         ##       ###        ###               
__________#_______####_______####______________

                我们的未来没有BUG                 
* ==============================================================================
* Filename: LoopItemBase
* Created:  2017/11/23
* Author:   To Harden The Mind
* Purpose:  LoopItem操作table的基类
* ==============================================================================
--]]

---@class LoopItemBase
---@field protected index number
---@field protected datas table
---@field protected _grid UILoopGrid
---@field protected _core LoopGridLuaItem
LoopItemBase = class("LoopItemBase")

function LoopItemBase:ctor()
    self.index = 0
    self.datas = nil
    ---@type UICore
    self.ui_core = nil
end

function LoopItemBase:BindUICore()
end


function LoopItemBase:FillItem(index, datas)
    self.index = index
    self.datas = datas
end

function LoopItemBase:OnAppPause(isPause)
    --LogInfo("UI ApplicationPause ! " , tostring(isPause));
end