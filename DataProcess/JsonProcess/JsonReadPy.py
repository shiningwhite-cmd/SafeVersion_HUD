import json  
import os
import socket  
import threading  
  

current_directory = os.getcwd()  
print("当前工作目录：", current_directory)

# 读取JSON文件  
def open_json_file(file_path):
     with open(file_path) as file: 
             data = json.load(file) 
             return data 
# 文件名  在这里改变视频名
file_name = "cd26264b-22001629" 

# 构建文件路径  
# person_file_path = f"./Assets/DataProcess/JsonProcess/JsonFiles/"+file_name+".json" 
file_path = f"./Assets/DataProcess/JsonProcess/JsonFiles/all_"+file_name+".json" 
screening_file_path = f"./Assets/DataProcess/JsonProcess/JsonFiles/llm_select_interval_v1.json" 

# 打开JSON文件  
data = open_json_file(file_path) 
person_data = open_json_file(file_path)
screening_data = open_json_file(screening_file_path) 
  

# 获取JSON数据的长度  
json_length = len(data)  
  
# 打印JSON数据的长度  
print("JSON文件长度：", json_length)  
 
  
HOST = 'localhost'  # 主机名  
PORT = 11111  # 端口号  
  
# 创建一个TCP socket  
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)  
server_socket.bind((HOST, PORT))  # 绑定主机和端口  
server_socket.listen(1)  # 监听连接  
  
print('等待连接...')  
  
# 接受连接  
client_socket, address = server_socket.accept()  
print('连接成功:', address)  
  
event = threading.Event() 


value_str = ""
value_str = value_str + json.dumps(json_length) + "|*|"

value_str = value_str + "{\"riskList\":" + json.dumps(screening_data[file_name]["ids"]) + ", "

value_str = value_str + "\"starting_frame\": ["
for id in screening_data[file_name]["ids"]:
    if(json.dumps(screening_data[file_name][str(id)]["starting_frame"]) == "null"):
        value_str = value_str + "-1 ,"
    else:
        value_str = value_str + json.dumps(screening_data[file_name][str(id)]["starting_frame"]) + ","
value_str = value_str + "0], "

value_str = value_str + "\"ending_frame\": ["
for id in screening_data[file_name]["ids"]:
    if(json.dumps(screening_data[file_name][str(id)]["ending_frame"]) == "null"):
        value_str = value_str + "-1 ,"
    else:
        value_str = value_str + json.dumps(screening_data[file_name][str(id)]["ending_frame"]) + ","
value_str = value_str + "0]}|*|"

for i in range(0, json_length-1):
        
    if(i+1<10):
        index = '000'+str(i+1)
    elif(i+1<100):
        index = '00'+str(i+1)
    elif(i+1<1000):
         index = '0'+str(i+1)
    else:
         index = str(i+1)

    # print(index)  

    # 访问JSON数据  
    bbox_data = person_data[index]['person']


    if(bbox_data != {}):
        # print(bbox_data)
        for key, value in bbox_data.items():  
            # print(f'Key: {key}')  
            # print(f'Value: {value}') 
            # print('---') 

            if value is not None:
                value_str = value_str + "{\"riskID\":" + key + ",\"list\":" + json.dumps(value) + "}" + "|#|"
    else:
        value_str = value_str + json.dumps(None) + "|#|"

    # 发送数据给Unity  
    value_str = value_str + "|*|"

value_str = value_str + "|$|"

for i in range(0, json_length-1):
        
    if(i+1<10):
        index = '000'+str(i+1)
    elif(i+1<100):
        index = '00'+str(i+1)
    elif(i+1<1000):
         index = '0'+str(i+1)
    else:
         index = str(i+1)

    # print(index)  
    cars_data = [data[index]['car'], data[index]['motorcycle'], data[index]['bus'], data[index]['truck'] ]

    if(cars_data != []):
        for car_data in cars_data:
        # print(bbox_data)
            for key, value in car_data.items():  
                # print(f'Key: {key}')  
                # print(f'Value: {value}') 
                # print('---') 

                if value is not None:
                    value_str = value_str + "{\"riskID\":" + key + ",\"list\":" + json.dumps(value) + "}" + "|#|"
    else:
        value_str = value_str + json.dumps(None) + "|#|"

    # 发送数据给Unity  
    value_str = value_str + "|*|"
        

chunk_size = 1024
chunks = [value_str[i:i+chunk_size] for i in range(0, len(value_str), chunk_size)]

# 逐个发送块
for chunk in chunks:
    # print("xxxxx")
    # print(chunk)
    client_socket.send(chunk.encode())


# 关闭连接  
client_socket.close()  
server_socket.close()  