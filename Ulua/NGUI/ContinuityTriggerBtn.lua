local LogInfo = LogInfo;
local LogError = LogError;
local table = table;

if not ContinuityTriggerBtn then
	ContinuityTriggerBtn = {};
    ContinuityTriggerBtn.interval_list = 
    {
        0.35;
        0.3;
        0.25;
        0.2;
        0.15;
        0.1;
        0.05;
    };
	ContinuityTriggerBtn.isStart = true;
end

local _M = ContinuityTriggerBtn;

function _M:RegisterCallback(ctrl, inst, tigger_fun, end_fun)
    if ctrl == nil or inst == nil or tigger_fun == nil or end_fun == nil then
        LogError("ContinuityTriggerBtn:RegisterCallback Failed, Param ctrl or inst or tigger_fun or end_fun nil");
        return;
    end
	self.startFunc = function(sender)
		self:StartClick(ctrl, inst, tigger_fun);
	end	
    self.endFunc = function(sender)
        self:EndClick(ctrl, inst, end_fun);
    end	
	RegisterOnMonoStartDrag2(ctrl.gameObject, self.startFunc);
    RegisterOnMonoEndDrag2(ctrl.gameObject, self.endFunc);
end

function _M:StartClick(ctrl, inst, tigger_fun)
	ContinuityTriggerBtn.isStart = true;
    if ctrl then
    else
        LogInfo("ContinuityTriggerBtn:StartClick, ctrl nil");
    end
    if ctrl == nil or inst == nil or tigger_fun == nil then
        LogError("ContinuityTriggerBtn:StartClick Failed, Param ctrl or inst or tigger_fun nil");
        return ;
    end

    local info_key = tostring(ctrl.name);
    if self[info_key] ~= nil then
        LogError("ContinuityTriggerBtn:StartClick, info_key is exist");
        self[info_key] = nil;
    end
    self[info_key] = {};
    self[info_key].now_interval_idx = 1;
    self[info_key].timer_id = AddTimer( self.interval_list[self[info_key].now_interval_idx], 
        function(timer_id)
			if not self.isStart then
				return 1;
			end
			tigger_fun(inst, ctrl.gameObject);
			-- 更新时间间隔
			if self[info_key] then
				local total_index = table.getn(self.interval_list);
				if self[info_key].now_interval_idx < total_index then
					local next_interval_idx = self[info_key].now_interval_idx + 1;
					if next_interval_idx <= 7 then
						ResetTimer(self[info_key].timer_id, self.interval_list[next_interval_idx]);
						self[info_key].now_interval_idx = next_interval_idx;
					end
				end
			end
			return 0;
        end
        );
end

function _M:EndClick(ctrl, inst, end_fun)
	ContinuityTriggerBtn.isStart = false;
    if ctrl == nil or inst == nil or end_fun == nil then
        LogError("ContinuityTriggerBtn:StartClick Failed, Param ctrl or inst or end_fun nil");
        return ;
    end

    local info_key = tostring(ctrl.name);
    if self[info_key] == nil then
        LogError("ContinuityTriggerBtn:EndClick Err, info_key not find");
        return;
    end
	
    RemoveTimer(self[info_key].timer_id);
    end_fun(inst, ctrl.gameObject);
    self[info_key] = nil;
end