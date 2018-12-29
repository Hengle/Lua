
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

