using UnityEngine;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Falcon {
  public class CandiedUI {
    public enum MessageType {
      None,
      Info,
      Warning,
      Error
    }

#if UNITY_EDITOR
    static private UnityEditor.MessageType GetUnityMessageType(MessageType messageType){
      switch (messageType){
        case MessageType.Info: return UnityEditor.MessageType.Info;
        case MessageType.Warning: return UnityEditor.MessageType.Warning;
        case MessageType.Error: return UnityEditor.MessageType.Error;
        default: return UnityEditor.MessageType.None;
      }
    }
#endif

    public class HelpBox {
      private string m_Message;
      public string Message => m_Message;

      private MessageType m_MessageType;
      public MessageType MessageType => m_MessageType;

      public HelpBox(){
        Set("", MessageType.None);
      }

      public void Set(string message, MessageType messageType){
        m_Message = message;
        m_MessageType = messageType;
      }

      public void OnGUI(){
#if UNITY_EDITOR
        if (!string.IsNullOrWhiteSpace(m_Message)){
          EditorGUILayout.HelpBox(m_Message, GetUnityMessageType(m_MessageType));
        }
#endif
      }
    }

    static public void Horizontal(Action onAction){
#if UNITY_EDITOR
      EditorGUILayout.BeginHorizontal();
      onAction();
      EditorGUILayout.EndHorizontal();
#endif
    }

    static public void Button(string label, Action onAction){
#if UNITY_EDITOR
      if (GUILayout.Button(label, GUILayout.Width(label.Length * 15))){
        GUI.FocusControl("");
        onAction();
      }
#endif
    }

    static public void SelectDirectory(string title, string defalutDirectoryPath, Action<string> onAction){
#if UNITY_EDITOR
      string directoryPath = EditorUtility.OpenFolderPanel(title, Path.GetFullPath(defalutDirectoryPath), "");
      if (!string.IsNullOrWhiteSpace(directoryPath)){
        onAction(Path.GetRelativePath(".", directoryPath));
      }
      GUIUtility.ExitGUI();
#endif
    }
  }
}