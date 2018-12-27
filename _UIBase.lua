
--region ctor

---@type UIBaseView
UIBaseView = class("UIBaseView", nil)
interface(UIBaseView,AsyncInterface)

local TRGGER_ACTION_IN = 1;
local TRGGER_ACTION_OUT = 10;
local TRGGER_ACTION_IDLE = 0;

local ANIM_END_UNINIT = 1;
local ANIM_END_HIDE = 2;

function UIBaseView:ctor(name)

    self._initOkFlag = false;
    ---@type string
    self._name = name;
    self._reload_event_id = nil;
    self.check_error = nil;
    self._isOpen = false;--是否打开了界面（异步加载，有可能还没有显示面板,隐藏也算打开，只有销毁才算false）
    self._isShow = false;--是否显示了界面（面板已显示）
    self._resident = false; --常驻内存
    self.useCommonBkg = false
    ---@type UIPanel
    self._panel = nil;
    ---@type Animator
    self._baseAnim = nil;
    ---@type AnimationEvent
    self._animEvent = nil;
    ---@type Transform
    self._transform = nil;
    ---@type GameObject
    self._gameObject = nil;
    ---@type UICore
    self.ui_core = nil;
    ---@type UIBaseCSView
    self._csBaseView = nil;

    self._animEndType = 0;
    self._currentDepth = 0;--界面深度值
    self._initOkDelegate = nil;
    self._asyncRequest = nil;
    self._loadingGameObjectCount = 0;
    self._isSkipShowAnim = false;
    self._isSkipUninitAnim = false;
    self._triggerAction = TRGGER_ACTION_IDLE;
    self._isRegisterCallback = false;
    self._closeFinishiFunc = nil; --关闭或者卸载回调

    ---@type UIBaseView[]
    self.childViews = nil

    self._isNotUseLoadAtlas = false; -- 是否不加载atlas与cs中依赖公用
    self._isHide = false;
    ---@type number
    self.pre_luastepgc_mem = 0;
    self.pre_luastepgc_time = 0;

    self.isReveal = false;

    --@type table
    self.params = nil;  -- 将window statck 中的参数 转移到uibase 中

    if name and ViewPath[name] then
        self._abName = ViewPath[name][1];
        UIManager:RegisterUIScript(StringManager.GetStrID(name), self)
    end

end

--endregion

function UIBaseView:OnAppPause(isPause)
    --LogInfo("UI ApplicationPause ! " , tostring(isPause));
end

--region api
-- isReveal是否
function UIBaseView:OpenWindow(bSkipAnim, openFinishedFunc,isReveal)
    self.isReveal = isReveal;
    --bSkipAnim = true

    UIParam.AddUI(self);
    --if not self._isOpen then
        self:Init(function()
            self:Show(bSkipAnim)
            if openFinishedFunc then
                openFinishedFunc()
            end
        end)
    --[[else
        if self._isHide or self._isShow then
            self:Show(bSkipAnim)
            if openFinishedFunc then
                openFinishedFunc()
            end
        end
    end
    self._isOpen = true]]
end

function UIBaseView:CloseWindow(bSkipAnim, isUninit,closeFinishFunc)
    self:PlayLeaveAudio();
    self._closeFinishiFunc = closeFinishFunc;
    self._bInNotCache = false;
    if isUninit then
        self:Uninit(bSkipAnim)
    else
        self:Hide(bSkipAnim)
    end
end

function UIBaseView:SetOpenParam(params)
    self.params = params or self.params;
    self:HandleOpenParam(self.params)
end

---@return boolean
function UIBaseView:Init(initOkDelegateFunc)
    -- 通用背板
    self:InitCommonBkg();

    self._isBaseUninit = false;
    if self._initOkFlag == true then

        if initOkDelegateFunc then
            initOkDelegateFunc();
        end
        return true;
    end

    -- 全屏colliderbox
    if initOkDelegateFunc ~= nil then
        self:ShowMainColliderBox();
    end

    if self:InitAfter() then
        self._initOkDelegate = initOkDelegateFunc;
    end

    return true;
end

---@param bSkipAnim boolean @是否跳过 打开动画
function UIBaseView:Uninit(bSkipAnim)
    self:Log("[UI_FLOW UIBaseView:Uninit] name " , self._name);

    UIParam.RemoveUI(self)
    self:CancelAsyncRequestPanel()

    if self._initOkFlag == false then
        return;
    end
    self:Log("[UI_FLOW UIBaseView:Uninit] name " , self._name);
    -- 防止多次卸载导致depth变化
    if self._isBaseUninit == true then
        return;
    end

    self._isBaseUninit = true;

    self._initOkFlag = false;

    bSkipAnim = self:IsStopAnim() or bSkipAnim

    --[[if self._baseAnim and self._isShow then
        if not bSkipAnim and self._animEvent then
            self._triggerAction = TRGGER_ACTION_OUT;
        else

            if not self._animEvent then
                LogError(self._abName, " animEvent is nil");
            end

            self._triggerAction = TRGGER_ACTION_IDLE;
            self:UninitAfter();
        end
        self._animEndType = ANIM_END_UNINIT;
        self:SetAnimAction();
    else]]
        self:UninitAfter();
    --end
end

---@param msgName string|number
---@param func fun()
---@param autoRemove boolean @后台不缓存消息
function UIBaseView:RegisterMessage(msgName, func, autoRemove,alwaysListen)
    if not msgName then
        print_error("[error][RegisterMessage] msgName is nil,view:"..self._name)
        return
    end
    if not self._notiList then
        self._notiList = {}
        self._cacheNotification = {}
    end
    local temp = self._notiList[msgName]
    if not temp then
        self._notiList[msgName] = {}
        temp = self._notiList[msgName]
    end
    temp.func = func
    if func == nil then
        print_error(msgName.."  register ,func is nil")
    end
    temp.autoRemove = autoRemove
    temp.alwaysListen = alwaysListen
    SetMessageHandler2(msgName, self, self.BaseHandleMessage)
end

function UIBaseView:BaseHandleMessage(msg)
    local name = msg.msg_name__
    local info = self._notiList[name]
    if not self:IsShow() then
        if info.alwaysListen then
            info.func(self, msg)
        elseif not info.autoRemove then
            --if  UIManager:IsViewInOpenedStack(self._view) then
            self._cacheNotification[name] = msg
            --end
        end
    else
        info.func(self, msg)
    end
end
--endregion

--region get set
---@return string
function UIBaseView:GetName()
    return self._name
end

---@return string
function UIBaseView:GetABName()
    return self._abName
end

---@return Transform
function UIBaseView:GetTransform()
    return self._transform
end

---@return number
function UIBaseView:GetCurrentDepth()
    return self._currentDepth
end

function UIBaseView:SetBaseActiveCore(flag)
    self:Log("[UI_FLOW UIBaseView:SetBaseActiveCore] name " , self._name,flag);
    self._gameObject:SetActive(flag);
end

function UIBaseView:SetShowFlag(isShow)
    self._isShow = isShow;
end

---@return boolean
function UIBaseView:IsDestroy()
    local isDestroy = not IsNotUninitInSetting(self) or self._bInNotCache or self._isNotUseLoadAtlas;
    return isDestroy;
end

---@return boolean
function UIBaseView:IsShow()
    return self._isShow
end
function UIBaseView:IsHide()
    return self._isHide
end
--是否常驻内存
---@return boolean
function UIBaseView:IsResident()
    return self._resident;
end
---是否正在异步加载
function UIBaseView:IsAsyncRequestLoading()
    if self._asyncRequest then
        return true
    else
        return false
    end
end
---取消异步加载
function UIBaseView:CancelAsyncRequestPanel()
    if self._asyncRequest ~= nil and self._asyncRequest ~= 0 then
        CancelInstanceCreateRequest(self._asyncRequest, self._abName);
        UIManager:RemoveAsyncPanelInit(self);
        self._asyncRequest = nil;
        --UIManager:HandleCancelAsyncRequestListener(self._name)
    end
end
---@return GameObject
function UIBaseView:GetGameObject()
    return self._gameObject;
end

-- 如果参数后面有变化 子类需要重写
function UIBaseView:GetParams()
    return self.params;
end
--endregion

--region 流程函数
function UIBaseView:BaseOnCreate()
    self:OnCreate()
end

function UIBaseView:BaseOnAnimInEnd()
    self:OnAnimInEnd()
end

function UIBaseView:BaseOnAnimOutEnd()
    self:OnAnimOutEnd()
end

function UIBaseView:BaseRegisterCallback()
    if self._isRegisterCallback then
        return
    end
    self._isRegisterCallback = true
    self:RegisterCallback()
    -- 每个页面都注册一个红点更新事件
    self:RegisterNewPointCallBack();
end

function UIBaseView:BaseUnRegisterCallback()
    self._isRegisterCallback = false
    self:UnRegisterCallback()
    if self._notiList then
        for k, v in pairs(self._notiList) do
            RemoveMessageHandler2(k, self, self.BaseHandleMessage)
        end
    end
    self._cacheNotification = nil
    self._notiList = nil
end

function UIBaseView:BaseOnEnable(isEnable)
    if isEnable then
        --UIManager:HandleUIOpenListener(self:GetName())
        self:ShowCommonBkg();
        self:OnShow(self.isReveal)
        if self._cacheNotification then
            for k, v in pairs(self._cacheNotification) do
                self:BaseHandleMessage(v)
            end
            self._cacheNotification = {}
        end
        -- 红点显示
        self:UpdateNewPointAll();
    else
        --UIManager:HandleUICloseListener(self:GetName())
        -- 原因一再打开相同界面时会存在问题
        self:HideCommonBkg();
        local isKillCall = self:OnHide();
        if not isKillCall then
            self:BaseCallCloseFinish();
        end
    end
end

function UIBaseView:BaseCallCloseFinish()
    if self._closeFinishiFunc then
        self._closeFinishiFunc();
        self._closeFinishiFunc = nil;
    end
end

function UIBaseView:BaseOnDestroy()
    self:OnDestroy()
    -- 清理table缓存
    self:DestroyCacheTable();
    self:AsyncInterface_Destroy()
end

function UIBaseView:ShowMainColliderBox()
    self:Log("[UI_FLOW UIBaseView:ShowMainColliderBox] name " , self._name);
    MainColliderBoxManager:OpenMainColliderBox(self);
end

function UIBaseView:HideMainColliderBox()
    self:Log("[UI_FLOW UIBaseView:HideMainColliderBox] name " , self._name);
    MainColliderBoxManager:CloseMainColliderBox(self);
end

--返回的子界面，父界面 自己 缓存
---@param core UICore
---@param childTb UIBaseView
---@return UIBaseView
function UIBaseView:RegisterChildView(core, childTb)
    if not self.childViews then
        self.childViews = {}
    end
    self:InitChildView(core,childTb)
    table.insert(self.childViews, childTb)
    return childTb
end
function UIBaseView:InitChildView(core, childTb)
    if core then
        core:Init(childTb)
        childTb._gameObject = core.gameObject;
    else
        --print("child core is nil")
    end
    childTb.parent = self
    childTb:OnCreate()

end
-- this func can over write in sub clss
function UIBaseView:OnCreate()
end
-- this func can over write in sub clss
function UIBaseView:HandleOpenParam(params)
    if not self.childViews then
        return
    end
    for _,value in pairs(self.childViews) do
        if value.HandleOpenParam then
            value:HandleOpenParam(params)
        end
    end
end
-- this func can over write in sub clss
function UIBaseView:OnAnimInEnd()
    if not self.childViews then
        return
    end
    for _,value in pairs(self.childViews) do
        if value.OnAnimInEnd then
            value:OnAnimInEnd()
        end
    end
end
-- this func can over write in sub clss
function UIBaseView:OnAnimOutEnd()
    if not self.childViews then
        return
    end
    for _,value in pairs(self.childViews) do
        if value.OnAnimOutEnd then
            value:OnAnimOutEnd()
        end
    end
end
-- this func can over write in sub clss
function UIBaseView:RegisterCallback()
    if not self.childViews then
        return
    end
    for _,value in pairs(self.childViews) do
        if value.RegisterCallback then
            value:RegisterCallback()
        end
    end
end
-- this func can over write in sub clss
function UIBaseView:UnRegisterCallback()
end
-- this func can over write in sub clss
function UIBaseView:OnShow()
    if not self.childViews then
        return
    end
    for _,value in pairs(self.childViews) do
        value._isShow = true
        value._isHide = false
        if value.OnShow then
            value:OnShow(self.isReveal)
        end
    end
end

function UIBaseView:OnHide()
    if not self.childViews then
        return
    end
    for _,value in pairs(self.childViews) do
        value._isShow = false
        value._isHide = true
        if value.OnHide then
            value:OnHide()
        end
    end
end

function UIBaseView:OnDestroy()
    if not self.childViews then
        return
    end
    for _,value in pairs(self.childViews) do
        if value.BaseUnRegisterCallback then
            value:BaseUnRegisterCallback()
        end
        if value.RemoveUITimers then
            value:RemoveUITimers()
        end
        if value.OnDestroy then
            value:OnDestroy()
        end
        if value.AsyncInterface_Destroy then
            value:AsyncInterface_Destroy()
        end
    end
    table.clear(self.childViews);
end
--endregion

--region 资源加载

---@return boolean
function UIBaseView:IsInited()
    if (self._initOkFlag == true and (self._isLoadUIAtlas or self._isNotUseLoadAtlas)) then
        return true;
    end
    return false;
end

---@param panelAbName string
---@param sourceAbName string
---@return boolean
function UIBaseView:IsSameAbName(panelAbName, sourceAbName)
    if panelAbName ~= nil then
        panelAbName = string.lower(panelAbName);
        if sourceAbName == nil then
            sourceAbName = self._abName;
        end
        sourceAbName = string.lower(sourceAbName);
        local startPos, endPos = string.find(sourceAbName, panelAbName);
        if startPos ~= nil and endPos ~= nil then
            return true;
        end
    end
    return false;
end

---@return boolean
function UIBaseView:InitAfter()
    if self._asyncRequest ~= nil then
        self:HideMainColliderBox();
        return false;
    end
    self._asyncRequest = CreateUIPanelAsync(self._abName,
            function(name, instance)
                self._asyncRequest = nil;
                UIManager:RemoveAsyncPanelInit(self);
                self:OnCreateInstance(instance);
            end
    );
    UIManager:AddAsyncPanelInit(self);
    return true;
end

---@param instance GameObject
function UIBaseView:OnCreateInstance(instance)
    if not instance then
        LogError("[ERROR]->no ", self._abName);
        return;
    end

    self:Log("[UI_FLOW UIBaseView:OnCreateInstance] " , self._name);
    self._initOkFlag = true;

    UIBaseCSView.Init(instance, self, self._abName)
    self:InitAnimate()
    self:InitEventHandle()

    self:BaseOnCreate()
    if self._loadingGameObjectCount ~= 0 then
        return
    end
    if not self._initOkDelegate then
        return
    end

    self:LoadUIAtlas(self._initOkDelegate);
end

function UIBaseView:InitEventHandle()
    if self._reload_event_id then
        LogError("remove existing OnLuaReload handler. event_id: ", self._reload_event_id);
        RemoveEventHandler("OnLuaReload", self._reload_event_id);
    end

    ---@return number
    local function Tick()
        self:BaseUnRegisterCallback();
        self:BaseRegisterCallback();
        return 1;
    end

    ---@return number
    local function EventHandle()
        self:AddUITimer(0.001, Tick);
        return 0;
    end

    self._reload_event_id = AddEventHandler("OnLuaReload", EventHandle);
end

function UIBaseView:InitAnimate()
    --[[if self._baseAnim  then
        if not self._baseAnim.runtimeAnimatorController then
            self._baseAnim = nil;
        end
    end

    if self._baseAnim and self._animEvent == nil then
        self._animEvent = self._gameObject:AddComponent(AnimEvent.GetClassType());
    end
    if self._baseAnim and self._animEvent then
        SetAnimMessageHandler(self._baseAnim:GetInstanceID(), function()
            self:OnBaseAnimEnd();
        end);
    end]]
end

function UIBaseView:LoadUIAtlas(func)
    self:Log("[UI_FLOW UIBaseView:LoadUIAtlas] name start " , self._name);
    if (self._isLoadUIAtlas or self._isNotUseLoadAtlas) then
        self:Log("[UI_FLOW UIBaseView:LoadUIAtlas] name finish " , self._name);
        self:HideMainColliderBox();
        func(self)
        return;
    end
    self._isLoadUIAtlas = true;
    GameLua.LoadUIAtlas(self._gameObject, function()
        self:Log("[UI_FLOW UIBaseView:LoadUIAtlas] name finish " , self._name);
        self:HideMainColliderBox();
        if func then
            func(self)
        end
    end );
end

function UIBaseView:ReleaseUIAtlas()
    if not self._isLoadUIAtlas or self._isNotUseLoadAtlas then
        return;
    end
    GameLua.ReleaseUIAtlas(self._gameObject);
    self._isLoadUIAtlas = false;
end

---@param bSkipAnim boolean @是否跳过UI过场动画
function UIBaseView:Show(bSkipAnim)
    self:Log("[UI_FLOW UIBaseView:Show] name " , self._name);
    if self._initOkFlag == false then
        return;
    end
    self._isBaseUninit = false;
    if self._isShow then
        self:BaseOnEnable(true);
        return;
    end

    local isstopanim = self:IsStopAnim()
    bSkipAnim = isstopanim or bSkipAnim;
    self._isSkipShowAnim = bSkipAnim
    --[[if self._baseAnim then
        if not bSkipAnim then
            self._triggerAction = TRGGER_ACTION_IN;
        else
            self._triggerAction = TRGGER_ACTION_IDLE;
            self:StopRuningAnimation(TRGGER_ACTION_IN)
        end
    end]]

    self:PlayEnterAudio();

    self:CalcPanelDepth()
    self:BaseRegisterCallback();
    self:ShowAfter();
end

function UIBaseView:CalcPanelDepth()
    --fix 深度移到prefab中计算
    local currentDepth = UIParam.GetCurrentUIDepth(self)
    self._currentDepth = currentDepth
    self._csBaseView:CalcPanelDepth(currentDepth)
end

function UIBaseView:ShowAfter()
    self:LoadUIAtlas(self.ShowAfterLoadUIAtlas);
end

function UIBaseView:ShowAfterLoadUIAtlas()
    self:Log("[UI_FLOW UIBaseView:ShowAfterLoadUIAtlas] name " , self._name);
    if not self:IsInited() then
        return;
    end
    if self.ui_core and self._triggerAction == TRGGER_ACTION_IDLE then
        self.ui_core:RelocateMainTransForm();
    end
    self:SetBaseActiveCore(true);
    self:SetShowFlag(true);
    self:SetAnimAction();
    self:BaseOnEnable(true)
end

function UIBaseView:AddLoadingGameObjectCount()
    self._loadingGameObjectCount = self._loadingGameObjectCount + 1;
end

function UIBaseView:ReduceLoadingGameObjectCount()
    if self._loadingGameObjectCount <= 0 then
        return
    end

    self._loadingGameObjectCount = self._loadingGameObjectCount - 1;
    if self._loadingGameObjectCount == 0 then
        local fun = self._initOkDelegate
        if fun then
            fun()
        end
    end
end

function UIBaseView:UninitAfter()
    self:ClearOnBaseAnimEndTimer();
    self:BaseUnRegisterCallback();
    self:RemoveUITimers();
    -- 红点隐藏
    self:RemoveNewPoint();

    if self._reload_event_id then
        RemoveEventHandler("OnLuaReload", self._reload_event_id);
        self._reload_event_id = nil;
    end
    self.check_error = nil;
    self:SetShowFlag(false);
    self._isOpen = false
    self._initOkDelegate = nil;
    self._isSkipUninitAnim = false;
    self._triggerAction = TRGGER_ACTION_IDLE;
    self.parentView = nil
    self._resident = false
    self._isHide = false;
    self.params = nil;
    self:BaseOnEnable(false)
    if self._gameObject then
        self:SetBaseActiveCore(false);
		self:ReleaseUIAtlas();
        local isDestroy = self:IsDestroy();
        if isDestroy then
            self:Log("[UI_FLOW UIBaseView:UninitAfter true] name " , self._name);
            self:BaseOnDestroy()
            DestroyUIPanel(self._gameObject:GetInstanceID());
            self._gameObject = nil;
            self._transform = nil;
            self._panel = nil;
            self._baseAnim = nil;
            self.ui_core = nil;
            self._animEvent = nil;
            self._allPanels = nil;
        end
    end
	
	self:RemoveProperties();
	
    --end
    self:UIGCCollect();
end

function UIBaseView:UIGCCollect()
    if UIManager.is_uninit_all then
        return;
    end
    local now = GetTime();
    if self.pre_luastepgc_time == 0 or now - self.pre_luastepgc_time > 1000 then
        self.pre_luastepgc_time = now;
        local memadd = 0;
        ---@type number
        local curmem = CalcLuaUsedMemory();
        if self.pre_luastepgc_mem > 0 then
            memadd = curmem - self.pre_luastepgc_mem;
        else
            self.pre_luastepgc_mem = curmem;
        end
        if memadd > 0 then
            -- step size越大耗时越大，这里限制一下step上限
            if memadd > 1000000 then
                memadd = 1000000;
            end
            if memadd > 0 then
                GCCollect(false, false, true, memadd);
                self.pre_luastepgc_mem = CalcLuaUsedMemory();
            end
        end
    end
end

function UIBaseView:Close()
    self:Hide(true)
end

---@param bSkipAnim boolean @是否跳过动画
function UIBaseView:Hide(bSkipAnim)
    self:Log("[UI_FLOW UIBaseView:Hide] name " , self._name);
    UIParam.RemoveUI(self)
    if self._initOkFlag == false or self._isShow == false then
        self:CancelAsyncRequestPanel()
        self:BaseCallCloseFinish();
        return;
    end

    bSkipAnim = self:IsStopAnim() or bSkipAnim
    --[[if self._baseAnim then
        if not bSkipAnim then
            -- 判断当前动画状态
            if self._triggerAction == TRGGER_ACTION_IN then
                self._triggerActionNext = TRGGER_ACTION_OUT;
                return;
            else
                self._triggerAction = TRGGER_ACTION_OUT;
            end
        else
            self._triggerAction = TRGGER_ACTION_IDLE;
            self:HideAfter();
        end
        if self._isShow == true then
            self._animEndType = ANIM_END_HIDE;
            self:SetAnimAction();
        end
    else]]
        self:HideAfter();
    --end
end

function UIBaseView:HideAfter()
    -- 红点隐藏
    self:RemoveNewPoint();
    self._isHide = true;
    self:SetShowFlag(false);
    self:SetBaseActiveCore(false);
    self:BaseOnEnable(false);
    self:ClearOnBaseAnimEndTimer();
end
--endregion

--region 动画
function UIBaseView:SetAnimAction()
    --[[if self._baseAnim and self._isShow == true then
        if self._triggerAction ~= TRGGER_ACTION_IDLE then
            local hasAnim = self:HasInOutAnimation(self._triggerAction)
            if hasAnim then
                self._baseAnim:SetInteger("trigger_action", self._triggerAction);
                self:ClearOnBaseAnimEndTimer()

                ---@return number
                local function tick()
                    self:OnBaseAnimEnd();
                    return 1;
                end

                self.onBaseAnimEndTimer = self:AddUITimer(3, tick)
                if self._animEvent and self._triggerAction ~= TRGGER_ACTION_IDLE then
                    self._animEvent:ReStart();
                end
            else
                self:OnBaseAnimEnd();
            end
        end
    end]]
end

---@return boolean
function UIBaseView:IsStopAnim()
    ---@type boolean
    local stop = false
    if ClientConfig["anim_stop"] and ClientConfig["anim_stop"] == 0 and not UIManager:IsUIPanelAnimEffect(self) then
        ---@type boolean
        stop = true
    end
    return stop
end

---@return boolean
function UIBaseView:HasInOutAnimation(triggerAction)
    local has = false
   --[[ if self._baseAnim then
        if triggerAction == TRGGER_ACTION_IN then
            has = self._baseAnim:HasState(0, Animator.StringToHash("core_in"))
        else
            has = self._baseAnim:HasState(0, Animator.StringToHash("core_out"))
        end
    end]]
    return true
end

function UIBaseView:RePlayAnimEnd()
    self:RePlayAnim(1);
end

function UIBaseView:RePlayAnimStart()
    self:RePlayAnim(0);
end

function UIBaseView:RePlayAnim(normalizedtime)
    --[[if self._baseAnim then
        normalizedtime = normalizedtime or 1;
        local stateinfo = self._baseAnim:GetCurrentAnimatorStateInfo(0);
        self._baseAnim:Play(stateinfo.fullPathHash, -1, normalizedtime);
    end]]
end

function UIBaseView:ClearOnBaseAnimEndTimer()
    if self.onBaseAnimEndTimer then
        self:RemoveUITimer(self.onBaseAnimEndTimer);
        self.onBaseAnimEndTimer = nil;
    end
end

function UIBaseView:OnBaseAnimEnd()
    self:ClearOnBaseAnimEndTimer();

    if self._triggerAction == TRGGER_ACTION_IN then
        self:BaseOnAnimInEnd();

        self._triggerAction = TRGGER_ACTION_IDLE;
        if self._triggerActionNext == TRGGER_ACTION_OUT then
            self:Hide();
            self._triggerActionNext = TRGGER_ACTION_IDLE;
        end
    elseif self._triggerAction == TRGGER_ACTION_OUT then
        self._triggerAction = TRGGER_ACTION_IDLE;
        if self._animEndType == ANIM_END_UNINIT then
            self:UninitAfter();
        elseif self._animEndType == ANIM_END_HIDE then
            self:HideAfter();
        end
        self:BaseOnAnimOutEnd();
    end
end

function UIBaseView:DarkMainCamera()
    SceneScript:OpenLobbyCameraAndLight(false)
end

function UIBaseView:LightMainCamera()
    SceneScript:OpenLobbyCameraAndLight(true)
end

function UIBaseView:StopRuningAnimation(trigger)
    --[[if self._baseAnim then
        self._baseAnim:SetInteger("trigger_action", trigger);
        self:RePlayAnimEnd()
        self:ClearOnBaseAnimEndTimer()
        self._triggerAction = TRGGER_ACTION_IDLE
    end]]
end

---@return number
---@param repeat_sec number
---@param func fun()
function UIBaseView:AddUITimer(repeat_sec, func)
    ---@type number
    local timer_id = AddTimer(repeat_sec, func);
    if not self.timers then
        self.timers = {};
    end
    self.timers[timer_id] = repeat_sec;
    return timer_id;
end

---@return boolean
function UIBaseView:RemoveUITimer(timer_id)
    if timer_id and self.timers and self.timers[timer_id] ~= nil then
        self.timers[timer_id] = nil;
        RemoveTimer(timer_id);
        return true;
    end
    return false;
end

function UIBaseView:RemoveUITimers()
    if self.timers then
        for timer_id, _ in pairs(self.timers) do
            RemoveTimer(timer_id);
        end
        self.timers = nil;
    end
end
--endregion

--region 基类函数

---@param materialId number
---@param materialType number
---@param mateiralCount number
---@param materialCore UICore
---@param materialIcon IconPlugin
function UIBaseView:RefreshMatrial(materialId, materialType, materialCount, materialCore, materialIcon)
    if not materialType or (materialType == 0) or (materialId == 0) then
        materialCore.gameObject:SetActive(false)
    else
        materialCore.gameObject:SetActive(true)
        materialIcon:RefreshAwardOrMaterial(materialType, materialId, materialCount, true)
    end
end

---@param materialId number
---@param materialType number
---@param mateiralCount number
---@param materialCore UICore
---@param materialIcon IconPlugin
function UIBaseView:RefreshMatrialEmptyMode(materialId, materialType, materialCount, materialCore, materialIcon, isCheckEnough)
    if not materialType or (materialType == 0) then
        materialIcon:SetEmpty()
    else
        materialIcon:ActiveInfos()
        if isCheckEnough == nil then
            materialIcon:RefreshAwardOrMaterial(materialType, materialId, materialCount, true)
        else
            materialIcon:RefreshAwardOrMaterial(materialType, materialId, materialCount, isCheckEnough)
        end
    end
end

function UIBaseView:Log(p1,p2,p3,p4,p5)
    if self._name == ViewConst.UIPanel_MainColliderBox then
        return;
    end
    DEBUG_UI_LOG(p1,p2,p3,p4,p5)
end
--endregion

-- region audio
function UIBaseView:PlayEnterAudio()
    local uipanelSetting = GetUIPanelSettingTable();
    local inputname = string.lower(ExtendCSharp.FromABNameToRaw(self._abName));
    if (uipanelSetting and uipanelSetting[inputname] ~= nil) then
        PlaySoundAsync(uipanelSetting[inputname].showaudio);
    end
end

function UIBaseView:PlayLeaveAudio()
    if self._isShow then
        local uipanelSetting = GetUIPanelSettingTable();
        local inputname = string.lower(ExtendCSharp.FromABNameToRaw(self._abName));
        if (uipanelSetting[inputname] ~= nil) then
            PlaySoundAsync(uipanelSetting[inputname].closeaudio);
        end
    end
end
-- endregion

--region cache table
--[[
    获取缓存
    界面缓存池系统(管理自建table)
    通过层级管理
--]]
-- 预设值缓存大小(非必须)
function UIBaseView:InitCacheTable(cacheCount)
    if not cacheCount then
        return;
    end
    ---@type table
    local tempCacheTables;
    if cacheCount == 1 then
        tempCacheTables = {{}};
    elseif cacheCount == 2 then
        tempCacheTables = {{},{}};
    elseif cacheCount == 3 then
        tempCacheTables = {{},{},{}};
    elseif cacheCount == 4 then
        tempCacheTables = {{},{},{},{}};
    elseif cacheCount == 5 then
        tempCacheTables = {{},{},{},{},{}};
    elseif cacheCount == 6 then
        tempCacheTables = {{},{},{},{},{},{}};
    elseif cacheCount == 7 then
        tempCacheTables = {{},{},{},{},{},{},{}};
    elseif cacheCount == 8 then
        tempCacheTables = {{},{},{},{},{},{},{},{}};
    else
        tempCacheTables = {{},{},{},{},{},{},{},{}};
        for i = 9, cacheCount do
            table.insert(tempCacheTables,{});
        end
    end

    self:InitCacheTableCore(tempCacheTables);
end
-- 获取缓存
---@param layer number
---@return table
function UIBaseView:GetCacheTable(layer)
    ---@type number
    layer = layer or 0;
    ---@type table
    local tempCache;
    -- 获取缓存
    if self.uiCacheTables then
        ---@param v table
        for k, v in pairs(self.uiCacheTables) do
            if v.layer == nil then
                tempCache = v;
                break;
            end
        end
    end
    -- 加入缓存
    if not tempCache then
        tempCache = {};
        self:AddCacheTable(tempCache);
    end
    tempCache.layer = layer;

    return tempCache;
end

-- 释放缓存 通过table
function UIBaseView:ReleaseCacheTable(t)
    if not t then
        return;
    end
    if self.uiCacheTables then
        for k, v in pairs(self.uiCacheTables) do
            if v == t then
                self:ReleaseCacheTableCore(self.uiCacheTables[k]);
                break;
            end
        end
    end
end

-- 释放缓存 通过层级
function UIBaseView:ReleaseCacheTableByLayer(layer)
    if not self.uiCacheTables then
        return;
    end
    for k, v in pairs(self.uiCacheTables) do
        if not layer and (layer and v.layer == layer)then
            self:ReleaseCacheTableCore(v);
        end
    end
end

-- 删除缓存 通过层级
function UIBaseView:DestroyCacheTable(layer)
    if not self.uiCacheTables then
        return;
    end
    if not layer then
        self.uiCacheTables = nil;
    else
        for k, v in pairs(self.uiCacheTables) do
            if v.layer == layer then
                self.uiCacheTables[k] = nil;
            end
        end
    end
end

function UIBaseView:ReleaseCacheTableCore(t)
    if not t then
        return;
    end
    for k, v in pairs(t) do
        t[k] = nil;
    end
end

function UIBaseView:AddCacheTable(t)
    if not self.uiCacheTables then
        self.uiCacheTablesIndex = 0;
        self.uiCacheTables = {};
    end
    self.uiCacheTablesIndex = self.uiCacheTablesIndex + 1;
    self.uiCacheTables[self.uiCacheTablesIndex] = t;
end

function UIBaseView:InitCacheTableCore(t)
    self.uiCacheTablesIndex = #t;
    self.uiCacheTables = t;
end
--endregion

-- region common bkg
function UIBaseView:InitCommonBkg()
    if not self.useCommonBkg then
        return;
    end
    UIManager:InitCommonBkg(self.commonBkgPos);
end
function UIBaseView:ShowCommonBkg()
    if not self.useCommonBkg then
        return;
    end
    UIManager:ShowCommonBkg(self.commonBkgPos);
end
function UIBaseView:HideCommonBkg()
    if not self.useCommonBkg then
        return;
    end
    UIManager:HideCommonBkg();
end
function UIBaseView:AddInPlayerRoot(trans)
    return UIManager:AddInPlayerRoot(trans);
end
-- endregion

-- region 红点处理
function UIBaseView:RegisterNewPointCallBack()
    self:RegisterMessage(MSGConst.Msg_Notify,self.BaseRefreshNewPoint,true)
end
function UIBaseView:BaseRefreshNewPoint(msg)
    if msg.notifyType==27 or msg.notifyType==29 or msg.notifyType==31 or msg.notifyType==33 or msg.notifyType==17 then
        self:UpdateNewPointAll();
    else
        UIBaseViewNewPoint:BaseRefreshNewPoint(self,msg);
    end
end
function UIBaseView:UpdateNewPointAll(custom1,custom2)
    self:RemoveNewPoint();
    self:AddUITimer(0.001,function()
        UIBaseViewNewPoint:UpdateNewPointAll(self,custom1,custom2);
        return 1;
    end);
end
function UIBaseView:RemoveNewPoint()
    UIBaseViewNewPoint:RemoveNewPoint(self);
end

function UIBaseView:AddProperty(pro_name, default_value)
	self[pro_name] = default_value;
	if not self._properties then 
		self._properties = {};
	end
	table.insert(self._properties, pro_name);
end

function UIBaseView:RemoveProperties()
	if self._properties then 
		for i,v in pairs(self._properties) do
			self[v] = nil;
		end
	end
	self._properties = {};
end

function UIBaseView:insertPropertyList(pro_name)
	if not self._properties then 
		self._properties = {};
	end
	table.insert(self._properties, pro_name);
end
-- endregion