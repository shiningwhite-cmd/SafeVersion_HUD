import json  
import os
import socket  
import threading  
  

current_directory = os.getcwd()  
print("当前工作目录：", current_directory)

# 读取JSON文件  
with open(r'Assets/DataProcess/JsonProcess/JsonFiles/Bbox_Video_fe0a.json') as file:  
    data = json.load(file)  
  

# 获取JSON数据的长度  
json_length = len(data)  
  
# 打印JSON数据的长度  
print("JSON文件长度：", json_length)  
 
  
HOST = 'localhost'  # 主机名  
PORT = 1111  # 端口号  
  
# 创建一个TCP socket  
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)  
server_socket.bind((HOST, PORT))  # 绑定主机和端口  
server_socket.listen(1)  # 监听连接  
  
print('等待连接...')  
  
# 接受连接  
client_socket, address = server_socket.accept()  
print('连接成功:', address)  
  
event = threading.Event() 

GetMessages = False

for i in range(0, json_length):
    while(GetMessages == False):
        unitydata = client_socket.recv(1024) 
        if(unitydata != None):
            GetMessages = bool(unitydata.decode())

    if(GetMessages): 
        
        if(i+1<10):
            index = '000'+str(i+1)
        else:
            index = '00'+str(i+1)

        print(index)  

        # 访问JSON数据  
        bbox_data = data[index]['person']
        pos_data = data[index]['car']  

        value_str = ""
        # 打印bbox数据  
        if(pos_data != {}):
            # print(bbox_data)
            for key, value in pos_data.items():  
                print(f'Key: {key}')  
                print(f'Value: {value}') 
                # string_list = value[0]
                # number_list = value[1]
                
                # print("String List:", string_list)  
                # print("Number List:", number_list) 
                print('---') 

                if value is not None:
                    value_str = value_str + json.dumps(value) + "|#|"
        else:
            value_str = value_str + json.dumps(None) + "|#|"
        
        value_str = value_str + "|*|"

        if(bbox_data != {}):
            # print(bbox_data)
            for key, value in bbox_data.items():  
                print(f'Key: {key}')  
                print(f'Value: {value}') 
                # string_list = value[0]
                # number_list = value[1]
                
                # print("String List:", string_list)  u
                # print("Number List:", number_list) 
                print('---') 

                if value is not None:
                    value_str = value_str + json.dumps(value) + "|#|"
        else:
            value_str = value_str + json.dumps(None) + "|#|"

        # 发送数据给Unity  
        client_socket.send(value_str.encode()) 
        GetMessages = False 

  
# 关闭连接  
client_socket.close()  
server_socket.close()  