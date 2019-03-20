TablesUtil = {}
--id ,等级
--return two param : first is AttributePool Item,second is attribute value ,Search by level
function TablesUtil:ParseAttributePoolTable(id,level)
    if id == nil then
        LogError("TablesUtil:ParseAttributePoolTable error!!! id == nil !!!!");
        return nil, nil;
    end

    local configRoot = Tables.GetAttributePoolTable()
    if configRoot then
        local config = configRoot[id]
        if config then
            if not level then
                level = config.default_level
            end
            local min = nil
            local max = nil 
            local value = nil
            for i = 1,5 do
                min = config["param_min"..i]
                max = config["param_max"..i]
                if level >= min and level <= max then
                    value = (config["param1"..i] * level + config["param2"..i])
                    return config,value
                end
            end
            return config,nil
		else
			LogError("AttributePool not found, id: " , id);
        end
    end
    return nil,nil
end

function table.clear(tb)
    if tb then
        for k in pairs(tb) do
            tb[k] = nil;
        end
    end
end
---@param tb table
---@param isArray boolean tb 是否是数组型
function table.RemoveValue(tb,value,isArray)
    if tb then
        if isArray then
            for k,v in pairs(tb) do
                if v == value then
                    table.remove(tb,k);
                    break;
                end
            end
        else
            for k,v in pairs(tb) do
                if v == value then
                    tb[k] = nil;
                    break;
                end
            end
        end
    end
end
---@param tb table
function table.InsertDifferentValue(tb,value)
    if not tb then return end
    local find = false
    for k,v in pairs(tb) do
        if v == value then
            find = true
            break
        end
    end
    if not find then
        table.insert(tb,value)
    end
end
---@param tb table
function table.IsNillOrEmpty(tb)
    if tb then
        for k,v in pairs(tb) do
            return false;
        end
        return true;
    end
    return true;
end

function table.ContainValue(t,value,param1,param2)
    local containIndex = 0;
    if t then
        for i, v in pairs(t) do
            if not param1 then
                if v == value then
                    containIndex = i;
                    break;
                end
            elseif not param2 then
                if v[param1] == value then
                    containIndex = i;
                    break;
                end
            else
                if v[param1][param2] == value then
                    containIndex = i;
                    break;
                end
            end
        end
    end
    return containIndex;
end

function table.Getlen(t)
    local count = 0;
    if t then
        for i, v in pairs(t) do
            count = count + 1;
        end
    end
    return count;
end

function threeOperator(exprCondition,expr1,expr2)
    if exprCondition then
        return expr1
    else
        return expr2
    end
end

return TablesUtil;
