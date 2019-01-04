---@class AsyncInterface
AsyncInterface = {
}
---@type AsyncInterface
local _M = AsyncInterface


function _M:AsyncInterface_Destroy()
    self:CancelAsyncLoadAll()
    self:ClearParticle()
end
function _M:AddAsyncLoadId(requestId,resName)
    if not self.requestIdList then
        self.requestIdList = {}
    end
    self.requestIdList[requestId] = resName
end
function _M:CancelAsyncLoadId(requestId)
    if requestId and self.requestIdList and self.requestIdList[requestId] then
        CancelInstanceCreateRequest(requestId,self.requestIdList[requestId])
        self.requestIdList[requestId] = nil
    end
end
function _M:CancelAsyncLoadAll()
    if not self.requestIdList then return end
    for k,v in pairs(self.requestIdList) do
        CancelInstanceCreateRequest(k,v)
    end
    self.requestIdList = {}
end
--region particle

function _M:HideParticle(path)
    if not self.asyncInterface_particleGoList then return end
    local go = self.asyncInterface_particleGoList[path]
    if go then
        go:SetActive(false)
    end
end
function _M:HideAllParticle()
    if not self.asyncInterface_particleGoList then return end
    for k,v in pairs(self.asyncInterface_particleGoList) do
        if v then
            v:SetActive(false)
        end
    end
end

function _M:ShowParticle(path,parent,pos,ignoreByShowAlready)
    if not self.asyncInterface_particleGoList then
        self.asyncInterface_particleGoList = {}
    end
    local go = self.asyncInterface_particleGoList[path]
    local id
    if go then
        local show = true
        if ignoreByShowAlready and go.activeSelf then
            show = false
        end
        if show then
            if pos then
                go.transform.localPosition = pos
            end
            go:SetActive(false)
            go:SetActive(true)
        end
        return
    end
    id,go = TransformUtil:ShowUIParticle(path,parent,pos)
    self.asyncInterface_particleGoList[path] = go;
    return id,go
end
function _M:ClearParticle()

    self.asyncInterface_particleGoList = nil
end
--endregion