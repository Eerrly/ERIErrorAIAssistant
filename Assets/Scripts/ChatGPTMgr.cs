using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading;

[System.Serializable] public class ChatGPTPostData
{
    public string model;
    public string prompt;
    public float temperature;
    public int max_tokens;
}

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

    private static string chatGPTKey = "";

    private const string chatGPTUri = "https://api.openai.com/v1/completions";
    private const string model = "text-davinci-003";
    private const float temperature = 0.5f;
    private const int max_tokens = 1024;

    private ChatGPTPostData chatGPTPostData;
    private int tryAgainTimes = 0;
    private static System.Action<string> PrintLogMethod;

    private static void Initialize()
    {
        var keyPath = Path.Combine(UnityEngine.Application.dataPath, "Resources/key.txt");
        if (File.Exists(keyPath))
        {
            chatGPTKey = UnityEngine.Resources.Load<UnityEngine.TextAsset>("key").text;
        }
        PrintLogMethod = UnityEngine.Debug.Log;
    }

    private void CallDebugLog(string result)
    {
        PrintLogMethod(result);
    }

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

    private void ThreadStart(string msg)
    {
        Thread thread = new Thread(new ParameterizedThreadStart(RequestToChatGPT));
        thread.Start(msg);
    }

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
            if(tryAgainTimes < 3)
            {
                tryAgainTimes++;
                ThreadStart((string)msg);
            }
            CallDebugLog(ex.Message);
        }
    }
}
#endif