

-- --wrap������ʹ��------
print("----------------wrap------------------")

--wrap��Create������
--1 ��ͬ�Ķ��Ǵ���һ��Coroutine
--2 ����1��wrap����ͨ��resumeȥ�õ���һ������ֵ(������Ϣ)
--3 ����2��wrap���������ֱ�ӵ��ú���ת��coroutine����create���ȹ�����Ҫresume����׬��coroutine
--4 ����3��wrap����ʹ��Coroutine.State(Э��)���鿴״̬

print("----------------1------------------")
co = coroutine.wrap (function ()
    print("co")
end)

print(co) -- thread:xx
print(co())-- co

print("----------------2------------------")
co2 = coroutine.wrap(function()
      print("--before yield---")
      print("co",coroutine.yield())
	  print("--after yield---")
	  end)
	  
print(co2)	  --thread:xx
print(co2())  --before yield-- ������print����
print(co2(1,2,3))--co 1,2,3  + --after yield --  ���ú������´�print������ʼִ��


-- �������Ҫ���coroutine��״̬��wrap�ȽϷ���,����Ҳ����
