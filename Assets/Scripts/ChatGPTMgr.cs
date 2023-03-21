using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading;

/// <summary>
/// 请求ChatGPT数据类
/// </summary>
[System.Serializable] public class ChatGPTPostData
{
    public string model;
    public string prompt;
    public float temperature;
    public int max_tokens;
}

/// <summary>
/// ChatGPT返回数据类
/// </summary>
[System.Serializable] public class ChatGPTCallBackData
{
    public string id;
    public string created;
    public string model;
    public List<TextSample> choices;

    [System.Serializable]
    public class TextSample
    {
        public string text;
        public string index;
        public string finish_reason;
    }
}

#if CHATGPT_ERRORAIASSISTANT
public class ChatGPTMgr
{
    private static ChatGPTMgr _ins;
    public static ChatGPTMgr Ins
    {
        get
        {
            if(_ins == null)
            {
                _ins = new ChatGPTMgr();
                Initialize();
            }
            return _ins;
        }
    }

    // OpenAI ChatGPT API Key
    private static string chatGPTKey = "";

    private const string chatGPTUri = "https://api.openai.com/v1/completions";
    // ChatGPT-3 模型
    private const string model = "text-davinci-003";
    private const float temperature = 0.5f;
    private const int max_tokens = 1024;

    private ChatGPTPostData chatGPTPostData;
    private int tryAgainTimes = 0;
    // 打印，可以自定义打印的方法
    private static System.Action<string> PrintLogMethod;

    /// <summary>
    /// ChatGPT管理器初始化
    /// </summary>
    private static void Initialize()
    {
        var keyPath = Path.Combine(UnityEngine.Application.dataPath, "Resources/key.txt");
        if (File.Exists(keyPath))
        {
            chatGPTKey = UnityEngine.Resources.Load<UnityEngine.TextAsset>("key").text;
        }
        PrintLogMethod = UnityEngine.Debug.Log;
    }

    /// <summary>
    /// 执行打印函数
    /// </summary>
    /// <param name="result">需要打印的内容</param>
    private void CallDebugLog(string result)
    {
        PrintLogMethod(result);
    }

    /// <summary>
    /// 开始准备发消息
    /// </summary>
    /// <param name="msg">消息内容</param>
    /// <param name="debugFunction">打印函数</param>
    public void Send(string msg, System.Action<string> debugFunction = null)
    {
        if (string.IsNullOrEmpty(chatGPTKey) || string.IsNullOrEmpty(msg))
        {
            return;
        }
        if (debugFunction != null)
        {
            PrintLogMethod = debugFunction;
        }
        tryAgainTimes = 0;
        ThreadStart(msg);
    }

    /// <summary>
    /// 启动线程发消息（为了不阻塞主线程）
    /// </summary>
    /// <param name="msg">消息内容</param>
    private void ThreadStart(string msg)
    {
        Thread thread = new Thread(new ParameterizedThreadStart(RequestToChatGPT));
        thread.Start(msg);
    }

    /// <summary>
    /// 使用C# HttpWebRequest 来请求ChatGPT
    /// </summary>
    /// <param name="msg">消息内容</param>
    private void RequestToChatGPT(object msg)
    {
        try
        {
            chatGPTPostData = new ChatGPTPostData()
            {
                model = model,
                temperature = temperature,
                prompt = (string)msg,
                max_tokens = max_tokens,
            };
            var bytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(chatGPTPostData));
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(chatGPTUri);
            request.Method = "POST";
            request.ContentType = "application/json; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Headers.Add("Authorization", string.Format("Bearer {0}", chatGPTKey));
            using (var stream = request.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string result = reader.ReadToEnd();
                string ans = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatGPTCallBackData>(result).choices[0].text.Trim();
                CallDebugLog(ans);
            }
        }
        catch(System.Exception ex)
        {
            // 在部分时候出现网络异常，将再次递归请求ChatGPT，超过3次则报出错误！
            if(tryAgainTimes < 3)
            {
                tryAgainTimes++;
                ThreadStart((string)msg);
                return;
            }
            CallDebugLog(ex.Message);
        }
    }
}
#endif