import socket

TCP_IP = input("Please Enter the ip address:")
TCP_PORT = 6600

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((TCP_IP, TCP_PORT))

while True:
	data = s.recv(1024)
	result = data.decode('ascii') #.split('\n')
	print(result)
	
	#print repr(data)
	
	MESSAGE = input("$ ")
	
	if MESSAGE == "exit":
		break
		
	s.send((MESSAGE + "\n").encode('ascii'))
	
	
s.close()
	