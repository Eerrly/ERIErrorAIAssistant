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
    /// ͨ�������ȡLog�Ĵ������
    /// </summary>
    private LogEditor()
    {
        m_ConsoleWindowType = Type.GetType("UnityEditor.ConsoleWindow,UnityEditor");
        m_ActiveTextInfo = m_ConsoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
        m_ConsoleWindowFileInfo = m_ConsoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
    }

    /// <summary>
    /// ���û�˫���ʲ�ʱ,Unity�༭��������Ŀ���Ƿ���ڱ������OnOpenAssetAttribute���Եķ���,�Զ����ø÷�����
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
    /// �����˫����Error Log�����Զ����ô�C#�����ʲ���
    /// ������Error Log��Ϊ�գ������ChatGPT���������������ѯ��
    /// </summary>
    /// <returns></returns>
    public bool FindCode()
    {
        var windowInstance = m_ConsoleWindowFileInfo.GetValue(null);
        var activeText = m_ActiveTextInfo.GetValue(windowInstance).ToString();
        if (!string.IsNullOrEmpty(activeText))
        {
            ChatGPTMgr.Ins.Send(string.Format($"{activeText}�����ʲô����?"));
        }
        return true;
    }
}
#endif
