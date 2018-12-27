

local declareNames = {}

function declare(name,initvalue)
    rawset(_G,name,initvalue)
	declareNames[name] = true
end

setmetatable(_G,{
    __newindex = function(t,n,v)
	if not declaredNames[n] then
	   error("attempt to write to undeclare var" .. n,2)
	else
	   rawset(t,n,v)
	end
end,
	
	
	__index =  function(_,n)
	if not declareNames[n] then
	   error("attempt to read undeclared var" .. n,2)
	else
	   return nil
	end
end,})

