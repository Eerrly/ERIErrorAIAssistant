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

    /// <summary>
    /// 通过反射获取Log的窗口类的
    /// </summary>
    private LogEditor()
    {
        m_ConsoleWindowType = Type.GetType("UnityEditor.ConsoleWindow,UnityEditor");
        m_ActiveTextInfo = m_ConsoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
        m_ConsoleWindowFileInfo = m_ConsoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
    }

    /// <summary>
    /// 当用户双击资产时,Unity编辑器会检查项目中是否存在被标记了OnOpenAssetAttribute属性的方法,自动调用该方法。
    /// </summary>
    /// <param name="instanceID"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    [UnityEditor.Callbacks.OnOpenAssetAttribute(-1)]
    private static bool OnOpenAsset(int instanceID, int line)
    {
        LogEditor.GetInstacne().FindCode();
        return false;
    }

    /// <summary>
    /// 如果是双击了Error Log，会自动调用打开C#代码资产。
    /// 如果这个Error Log不为空，则调用ChatGPT管理器，对其进行询问
    /// </summary>
    /// <returns></returns>
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
