

function printtb(tb)

  local s = ""
  for i,v in ipairs(tb) do
     if type(v) == "table" then
	    s = i .. " --- " 
	    for m,n in pairs(v) do
		  s = s .."-key:" .. m .. "-val:" .. n
		end
		print(s)
		s = ""
	end
  end
end


function cmp(a,b)
    if a.e == b.e then
	   if a.q ==  b.q then
	     return a.t <b.t
	   else
	     return a.q > b.q
	   end
	
	else
	   return a.e > b.e
	end
	 
end

par = {}

par[1] = {e =1 ,q =2, t = 3}
par[2] = {e =0 ,q=10, t = 1}
par[3] = {e =0 ,q=4, t = 2}
par[4] = {e =1 ,q=5, t = 6}
par[5] = {e =0 ,q=5, t = 6}

print("--original--")
printtb(par)
print(par[1].q)
print("--qulity--")
table.sort(par, cmp)
printtb(par)