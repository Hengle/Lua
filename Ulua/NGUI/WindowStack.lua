---@class WindowStack
WindowStack = class("UIManager.WindowStack")
--mainView :打开界面的核心界面，其他辅助界面自动打开，关闭时只要关闭核心界面即可，辅助界面自动关闭
function WindowStack:ctor()
    self.initViews = {};            -- 所有加载ui
    self.openedViews = {};          -- 显示ui(可以用于自动返回)
    self.openingViews = {};         -- 正在打开的ui
    self.params = nil;
    self.mainView = nil;
    self.isSkipCloseAim = false;
    self.isSkipOpenAim = false;

    -- 当前操作是否是自动操作
    self.isManuAuto = false;
end

-- region public
function WindowStack:InitViews(params,views,initFinishFunc)
    local allInitCount = #views;
    if allInitCount == 0 then
        DEBUG_UI_LOG("[error UI_FLOW WindowStack:InitStack] count is 0");
        return;
    end
    local initCount = 0;
    -- 默认加载时需要显示的ui 如果后面不在设置 就 使用加载时的ui显示
    self:AddOpeningViews(views):SetParams(params);
    for i = 1, allInitCount do
        self:AddInitView(views[i]);
        views[i]:Init(function()
            initCount = initCount + 1;
            DEBUG_UI_LOG("[UI_FLOW WindowStack:InitStack] name " , self:GetMain():GetName() , initCount,allInitCount);
            if allInitCount == initCount then
                if initFinishFunc then
                    initFinishFunc();
                end
            end
        end);
    end
    return self;
end
function WindowStack:OpenStack(isReveal)
    self:OpenView(isReveal);

    self:OpenNextTut();
end
function WindowStack:CloseView(view,closeFinishedFunc)
    self:CloseTut(view);

    if view == self:GetMain() then
        self:CloseViewStack(closeFinishedFunc);
    else
        self:CloseViewCore(view,closeFinishedFunc);
    end
end
-- endregion

-- region private
function WindowStack:OpenView(isReveal)
    local openingViews = self:GetOpeningViews();
    if openingViews then
        for i, v in  ipairs(openingViews) do
            self:AddOpenedView(v);
            v:OpenWindow(self.isSkipOpenAim,nil,isReveal);
        end
        table.clear(openingViews);
    end
end
function WindowStack:CloseViewStack(closeFinishedFunc)
    DEBUG_UI_LOG("[UI_FLOW WindowStack:CloseViewStack] name a " , self:GetMain():GetName() , tostring(self:GetDestroy()));
    -- 可能会多次关闭
    if not self:GetDestroy() then
        local openingViews = self:GetOpeningViews();
        if openingViews and #openingViews > 0 then
            return;
        end
    end

    DEBUG_UI_LOG("[UI_FLOW WindowStack:CloseViewStack] name b " , self:GetMain():GetName() , tostring(self:GetDestroy()));

    -- 自动流程中才会清空 要保留等待返回
    local openedViews = nil;
    if self:GetManu() then
        self.isManuAuto = false;
        openedViews = {};
        for i, v in ipairs(self:GetOpenedView()) do
            table.insert(openedViews,v);
        end
    end

    local views = nil;
    if self:GetDestroy() then
        views = self:GetInitView();
    else
        views = self:GetOpenedView();
    end
    if views then
        -- 这里是防止 循环的时候出错
        local tempViews = {};
        for i, v in ipairs(views) do
            table.insert(tempViews,v);
        end
        for k,v in pairs(tempViews) do
            if v == self:GetMain() then
                self:CloseViewCore(v,closeFinishedFunc);
            else
                self:CloseViewCore(v);
            end
        end
    end

    if openedViews then
        self:AddOpeningViews(openedViews);
    end
end
function WindowStack:CloseViewCore(view,closeFinishedFunc)
    self:RemoveOpenedView(view);
    if self:GetDestroy() then
        self:RemoveInitView(view);
    end
    view:CloseWindow(self.isSkipCloseAim,self:GetDestroy(),closeFinishedFunc);
end
-- endregion

-- region set get
function WindowStack:SetParams(params)
    for i, v in ipairs(self:GetOpeningViews()) do
        v:SetOpenParam(params);
    end
    return self;
end
function WindowStack:SetMainView(mainView)
    self.mainView = mainView;
    return self
end
function WindowStack:GetMain()
    return self.mainView;
end
function WindowStack:AddInitView(view)
    DEBUG_UI_LOG("[UI_FLOW WindowStack:AddInitView] name " , view:GetName());
    if table.ContainValue(self.initViews,view) == 0 then
        table.insert(self.initViews,view);
    end
    if not self.mainView then
        self.mainView = view;
    end
    return self;
end
function WindowStack:GetInitView()
    return self.initViews;
end
function WindowStack:RemoveInitView(view)
    DEBUG_UI_LOG("[UI_FLOW WindowStack:RemoveInitView] name " , view:GetName());
    table.RemoveValue(self.initViews,view,true);
end
function WindowStack:AddOpenedView(view)
    DEBUG_UI_LOG("[UI_FLOW WindowStack:AddOpenedView] name " , view:GetName());
    if table.ContainValue(self.openedViews,view) ~= 0 then
        table.RemoveValue(self.openedViews,view,true);
    end
    table.insert(self.openedViews,view);
    return self;
end
function WindowStack:GetOpenedView()
    return self.openedViews;
end
function WindowStack:RemoveOpenedView(view)
    DEBUG_UI_LOG("[UI_FLOW WindowStack:RemoveOpenedView] name " , view:GetName());
    table.RemoveValue(self.openedViews,view,true);
end
function WindowStack:AddOpeningViews(views)
    if views then
        for i, v in ipairs(views) do
            if table.ContainValue(self.openingViews,v) == 0 then
                DEBUG_UI_LOG("[UI_FLOW WindowStack:AddOpeningViews] name " , v:GetName());
                if not self.mainView then
                    self.mainView = v;
                end
            end
        end
    end
    self:SetOpeningViews(views);
    return self;
end
function WindowStack:SetOpeningViews(views)
    self.openingViews = views;
end
function WindowStack:GetOpeningViews()
    return self.openingViews;
end
function WindowStack:SetSkipCloseAim(isSkipCloseAim)
    self.isSkipCloseAim = isSkipCloseAim;
    return self;
end
function WindowStack:SetSkipOpenAim(isSkipOpenAim)
    self.isSkipOpenAim = isSkipOpenAim;
    return self;
end
function WindowStack:SetDestroy(isDestory)
    self.isDestory = isDestory;
    return self;
end
function WindowStack:GetDestroy()
    return self.isDestory;
end
function WindowStack:SetManu(manu)
    self.isManuAuto = manu;
end
function WindowStack:GetManu()
    return self.isManuAuto;
end
function WindowStack:IsShow()
    return self:GetMain():IsShow();
end
function WindowStack:IsMainHallView()
    local mainView = self:GetMain();
    return mainView._name == ViewConst.UIPanel_Mainmap;
end
-- endregion

-- region tut
function WindowStack:CloseTut(view)
    local tutView = UIManager:GetView(ViewConst.UIPanel_Tut);
    if view == tutView then
        return;
    end
    if UIManager:CheckUIPanelIgnoerFullScreen(view) then
        if UITutManager:GetDePendUIView() ~= view then
            return;
        end
    end
    UITutManager:CloseTut();
end
function WindowStack:OpenNextTut()
    local tutView = UIManager:GetView(ViewConst.UIPanel_Tut);
    if self:GetMain() == tutView then
        return;
    end
    if UIManager:CheckUIPanelIgnoerFullScreen(self:GetMain()) then
        if UITutManager:GetNextDePendUIView() ~= self:GetMain() then
            return;
        end
    end

    UITutManager:OpenNextTut();
end
-- endregion