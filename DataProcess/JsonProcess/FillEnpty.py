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
file_name = "easy_1" 

# 构建文件路径  
file_path = f"./Assets/DataProcess/JsonProcess/JsonFiles/b1c9c847-3bda4659.json" 
# 打开JSON文件  
data = open_json_file(file_path) 
emptyFrame = {"person":{},
              "car":{}}

for i in range(0, 1200):
    if(i+1<10):
        index = '000'+str(i+1)
    elif(i+1<100):
        index = '00'+str(i+1)
    elif(i+1<1000):
         index = '0'+str(i+1)
    else:
         index = str(i+1)

    if(index not in data):
        data[index]=emptyFrame
        print(index)

with open(file_path, 'w')as file:
     json.dump(data, file, indent=4)