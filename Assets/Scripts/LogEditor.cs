using System;
using System.Reflection;

#if CHATGPT_ERRORAIASSISTANT
public class LogEditor
{
    private static LogEditor m_Instance;
    public static LogEditor GetInstacne()
    {
        if (m_Instance == null)
        {
            m_Instance = new LogEditor();
        }
        return m_Instance;
    }
    private Type m_ConsoleWindowType = null;
    private FieldInfo m_ActiveTextInfo;
    private FieldInfo m_ConsoleWindowFileInfo;
    private FieldInfo m_ActiveModeInfo;

    private LogEditor()
    {
        m_ConsoleWindowType = Type.GetType("UnityEditor.ConsoleWindow,UnityEditor");
        m_ActiveTextInfo = m_ConsoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
        m_ConsoleWindowFileInfo = m_ConsoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
    }

    [UnityEditor.Callbacks.OnOpenAssetAttribute(-1)]
    private static bool OnOpenAsset(int instanceID, int line)
    {
        LogEditor.GetInstacne().FindCode();
        return false;
    }

    public bool FindCode()
    {
        var windowInstance = m_ConsoleWindowFileInfo.GetValue(null);
        var activeText = m_ActiveTextInfo.GetValue(windowInstance).ToString();
        if (!string.IsNullOrEmpty(activeText))
        {
            ChatGPTMgr.Ins.Send(string.Format($"{activeText}这个是什么错误?"));
        }
        return true;
    }
}
#endif
