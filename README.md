# ERIErrorAIAssistant
## 介绍：
***双击Unity的ErrorLog，使用ChatGPT来对其进行纠错(可以让策划，测试，程序使用，方便调试错误，快速定位)***
- - -
### 环境
+ Unity2019-2020 [Unity官方地址](https://unity.com/)
+ Visual Studio 
### 使用
+ 将脚本放入 Unity 项目中
+ `Resources/key.txt` 中填写 [OpenAI Key](https://platform.openai.com/account/api-keys)
- - -
### 功能
<details>
<summary>✔️ErrorLog回调</summary>
  
  - 获取到Unity面板ErrorLog的点击回调
</details>

<details>
<summary>✔️接入ChatGPT</summary>
  
  - 通过HttpWebRequest来请求ChatGPT
  - 多线程请求
</details>


