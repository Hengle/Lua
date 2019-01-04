local LogInfo = LogInfo;
local LogError = LogError;

if not UITabBase then
	UITabBase = {};
end

local _M = UITabBase;

function _M:Create(name)
    local t = {}  
    setmetatable(t,{__index=self});
    t.inited_ = false;	
    t.tab_name = name;
    t.events = {};
    t.msg = {};
	t.transform = nil;
    t.game_object = nil;
	t._gameObject = nil;
    t.is_show = false;
    t.check_error = nil;
    t.reload_event_id = nil;
    t.dynic_obj = {};
    t.loading_dynic_obj = {};
    t.timers = {};
	return t;
end

function _M:Init(transform, parent)
	parent:AddChildPanel(self);
	self._gameObject = parent._gameObject;
	self.transform = transform;
    self.game_object = transform.gameObject;
	self:InitCore();
	self:InitLocal();
	self.inited_ = true;
end

function _M:InitCore()
    LogError("[InitCore] need over wirte in sub class-------->" .. self.tab_name);
end

function _M:Uninit()
    if self.inited_ == false then 
		return;
	end
	self:Close();
    self:UninitCore();
    if self.reload_event_id then
        RemoveEventHandler("OnLuaReload", self.reload_event_id);
		self.reload_event_id = nil;
    end
    self.inited_ = false;	
    self.events = {};
    self.msg = {};
	self.transform = nil;
    self.game_object = nil;
    self.is_show = false;
    self.check_error = nil;
    self.dynic_obj = {};
end

function _M:UninitCore()
    LogError("[UITabBase:UninitCore] need over wirte in sub class-------->" .. self.tab_name);
end

function _M:RegisterCallback()
    LogError("[RegisterCallback] need over wirte in sub class-------->" .. self.tab_name);
end

function _M:UnRegisterCallback()
    for i,v in pairs(self.events) do
        RemoveEventHandler(v, i);
    end
    self.events = {};
    
    for i,v in pairs(self.msg) do
        RemoveMessageHandler(i);
    end
    self.msg = {};
end

function _M:AddUIEvent(event_name, callback)
    local event_id = AddEventHandler(event_name, callback);
    if self.events[id] then
        LogError("[AddUIEvent] dunplicate event_id: " .. event_id);
        return ;
    end
    self.events[event_id] = event_name;
end

function _M:AddMessage(msg_name, func)
    SetMessageHandler(msg_name, func);
    self.msg[msg_name] = 1;
end

function _M:InitLocal()
   LogError("[UITabBase:InitLocal] need over wirte in sub class-------->" .. self.tab_name); 
end

function _M:Refresh()
    LogError("[UITabBase:Refresh] need over wirte in sub class-------->" .. self.tab_name);
end

function _M:PreShow()
    --LogInfo("[UITabBase:PreShow] can over wirte in sub class-------->" .. self.tab_name);
    return true;
end

function _M:Preset()
    LogInfo("[UITabBase:Preset] can over wirte in sub class-------->" .. self.tab_name);
end

function _M:FollowingShow()
    --LogInfo("[UITabBase:FollowingShow] can over wirte in sub class-------->" .. self.tab_name);
end

function _M:Show()
    if self.inited_ ~= true then
        LogError("Show inited_== false, tab_name=" .. self.tab_name);
		return;
    end
    
    if self:IsShow() == true then
	    self:Refresh();
        return ;
    end
    
    if not self:PreShow() then
        LogError("PreShow Failed, tab_name=" .. self.tab_name);
        return;
    end
    self:Preset();
    self:RegisterCallback();
    self.game_object:SetActive(true);
    self:Refresh();
    self.is_show = true;
    if self.reload_event_id then
		LogError("remove existing OnLuaReload handler. event_id: " .. self.reload_event_id);
        RemoveEventHandler("OnLuaReload", self.reload_event_id);
    end	
    self.reload_event_id = AddEventHandler("OnLuaReload", function()
        self:AddUITimer(0.001, function()
            self:UnRegisterCallback();
            self:RegisterCallback();
            return 1;
        end);
        return 0;
    end
    );
    self:FollowingShow();
end

function _M:Close()
    if self.inited_ ~= true then
        -- LogError("Close inited_== false, tab_name=" .. self.tab_name);
        return ;
    end
    --LogInfo(self.tab_name .. " : " .. self.game_object.name .. " : " .. debug.traceback());
	if self.is_show ~= true then
        -- LogError("Close inited_== false, tab_name=" .. self.tab_name);
		self.game_object:SetActive(false);
        return ;
    end
    self:CloseCore();
    self:UnRegisterCallback();
    self:RemoveUITimers();
    self.game_object:SetActive(false);
    self.is_show = false;
end

function _M:CloseCore()
	--LogInfo("CloseCore, tab_name=" .. self.tab_name);
end

function _M:IsShow()
    return self.is_show;
end

function _M:Check()
    return self.check_error == ErrorCode.ERR_OK;
end

function _M:ShowUIParticleRT(path, go)
    
    --[[local particleInfo = UIManager:GetParticle(path);
    if not particleInfo then
        CreateInstanceAsync(nil, path, true,
            function(res, instance)
                if UIManager:AddParticle(path, instance.transform, 128, UnityEngine.CameraClearFlags.SolidColor) == true then
                    instance:SetActive(true);
                    particleInfo = UIManager:GetParticle(path);
                    local texture = GetTexture(go.transform);
                    if not texture then
                        texture= AddComponent(go, UITexture.GetClassType());
                    end
                    texture.mainTexture = particleInfo.renderTexture;
                    SetCardViewerCamera(particleInfo.cameraTransform, particleInfo.transform);
                end             
            end
            );
    else
        local texture = GetTexture(go.transform);
        if not texture then
            texture= AddComponent(go, UITexture.GetClassType());
        end
        GetTexture(go.transform).mainTexture = particleInfo.renderTexture;
    end
    ]]--
end

function _M:RemoveUIParticleRT(path, go)
    go.gameObject:SetActive(false);
    UIManager:ReleaseParticle(path, go);
end

function _M:ShowUIParticle(path, go, vec, onCreate)
    self:CreateDynicObject(path, go, vec, onCreate);
end

function _M:CreateDynicObject(path, parent, vec, onCreate)
    local local_path = path .. parent:GetInstanceID();
	local instanceID = self.dynic_obj[local_path];
    if instanceID then
		local go = GetGameObjectByID(instanceID);
		if vec then
			go.transform.localPosition = vec;
		end
        go:SetActive(false);
        go:SetActive(true);
		if onCreate then
			onCreate(path, go);
		end			
    else
        CreateInstanceAsync(self, path, true,
			function(res, instance)
				instance:SetActive(true);
				instance.transform.parent = parent.transform;
				if(vec == nil) then
					vec = Vector3.New(0,0, -100);
				else
					vec = Vector3.New(vec.x, vec.y, -100);
				end
				instance.transform.localPosition = vec;				
				instance.transform.localScale = Vector3.New(320,320,320);		
				self.dynic_obj[local_path] = instance:GetInstanceID();
				if onCreate then
					onCreate(res, instance);
				end				
			end
        );
    end
end

function _M:RemoveDynicObject(path, parent)
    local local_path = path .. parent:GetInstanceID();
    local instanceID = self.dynic_obj[local_path];
    if instanceID then
		DestroyInstance(instanceID);
        self.dynic_obj[local_path] = nil;
    end
end

function _M:AddUITimer(repeat_sec, func)
    local timer_id = AddTimer(repeat_sec, func);
	self.timers[timer_id] = repeat_sec;
	return timer_id;
end

function _M:RemoveUITimer(timer_id)
	if self.timers[timer_id] ~= nil then
		self.timers[timer_id] = nil;
		RemoveTimer(timer_id);
		return true;
	end
	return false;
end

function _M:RemoveUITimers()
	for timer_id, _ in pairs(self.timers) do
		RemoveTimer(timer_id);
	end
	self.timers = {};
end


