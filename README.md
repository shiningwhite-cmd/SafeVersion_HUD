# SafeVersion_HUD

## 相关信息
- json文件存放路径："Assets/DataProcess/JsonProcess/JsonFiles"
- python虚拟环境文件夹路径："Assets/DataProcess/UnityEnv310"，使用python310环境，运行python代码请使用这个环境，不要用UnityEnv的虚拟环境
- 运行眼动：连接tobii glasses pro 3，并且打开glasses pro controler软件，打开"Assets/DataProcess/EyetrackerProcess/PythonFiles/dataprocessing.py"路径下的文件，运行，待Terminal出现 **"等待Unity连接"** 字样后运行Unity
- 读取json文件：目前只有测试版本，打开"Assets/DataProcess/JsonProcess/JsonReadPy.py"路径下的文件，运行，待Terminal出现 **"等待Unity连接"** 字样后运行Unity
## 接下来的工作（unity端）：
- [x] 从json文件中读取数据（预计采用python读取并实时传信的方式）（unity直接读取遇到麻烦）
- [x] 逐帧处理所读取的数据
- [x] 将数据与视频时间轴对应
- [x] 完成视频demo开发

目前效果：根据目标位置间断式提醒mark显示；
后续工作：平滑变化与真目标追踪；关注一下时间轴问题；
