
-- Utility----


function PALL(f)
print("table--" , f)
for i,v in pairs(f) do
   if type(v) == "table" then
      print("table--" , v)
      for m,n in pairs(v) do
	     print(m,n)
	  end
   end
   print(i,v)
end
end

function PALLINFO(f) 
    printtable(f)
end




function printevery(f)

print("----printevery---")

local num = 0
local tchild = {}

local function findchildtable(f)
    for i,v in ipairs(tchild) do
	  if v == f then
	    return true
	  else
	     return false
	  end
	end
end

local function printother(i,v)
     if type(v) ~= "table" then
       print(i,v)
     else
       print("--error--")
     end
end

local function printtable(f)
    for i,v in pairs(f) do
      if type(v) == "table"  then  
	     if findchildtable(v) ~= true then
	        print("table--",v)
		    num = num +1
		    tchild[num] = v	
	        printtable(v)
		 else
		    print("--same table--")
		  end
	  else
	    printother(i,v)
	  end
	end
end

	  


  return printtable(f)

end
	
   
   
