#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Falcon {
  [ScriptedImporter(1, "flcn")]
  public class FalconImporter : ScriptedImporter {
    public override void OnImportAsset(AssetImportContext assetImportContext){
      TextAsset textAsset = new TextAsset(File.ReadAllText(assetImportContext.assetPath));
      assetImportContext.AddObjectToAsset("Main", textAsset);
      assetImportContext.SetMainObject(textAsset);
    }
  }

  [CustomEditor(typeof(FalconImporter))]
  public class FalconImporterEditor : ScriptedImporterEditor {
    private class Selection<K, V> {
      private List<KeyValuePair<K, V>> m_KeyValues = new List<KeyValuePair<K, V>>();

      private K[] m_Keys = new K[0];
      public K[] Keys => m_Keys;

      private V[] m_Values = new V[0];
      public V[] Values => m_Values;

      public void Add(K key, V value){
        m_KeyValues.Add(new KeyValuePair<K, V>(key, value));
        m_Keys = m_KeyValues.Select(x => x.Key).ToArray();
        m_Values = m_KeyValues.Select(x => x.Value).ToArray();
      }
    }

    private class Selected<K, V, O> {
      public Selection<K, V> Selection;

      public int Index;

      public K Key => Selection.Keys[Index];

      public V Value => Selection.Values[Index];

      public O Object;
    }

    static private Selection<string, Type> SelectionConfig = new Selection<string, Type>();

    private Selected<string, Type, Config> m_SelectedConfig = new Selected<string, Type, Config>();

    private CandiedUI.HelpBox m_HelpBox = new CandiedUI.HelpBox();

    static FalconImporterEditor(){
      SelectionConfig.Add("Falcon.Config", typeof(Config));
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()){
        foreach (Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Config)))){
          SelectionConfig.Add($"{type.Namespace}.{type.Name}", type);
        }
      }
    }

    public override void OnEnable(){
      base.OnEnable();
      if (ConfigManager.Instance.ConfigUtility == null){
        ConfigManager.Instance.ConfigUtility = new JsonConfigUtility();
      }
      FalconImporter falconImporter = target as FalconImporter;
      m_SelectedConfig.Object = ConfigManager.Instance.Load(falconImporter.assetPath, typeof(Config));
      m_SelectedConfig.Selection = SelectionConfig;
      m_SelectedConfig.Index = 0;
    }

    public override void OnInspectorGUI(){
      base.OnInspectorGUI();
      if (SelectionConfig.Values.Length == 0) return;

      FalconImporter falconImporter = target as FalconImporter;
      m_SelectedConfig.Index = EditorGUILayout.Popup("ConfigType", m_SelectedConfig.Index, SelectionConfig.Keys);
      if (m_SelectedConfig.Index == 0) return;

      Type configType = m_SelectedConfig.Value;
      if (m_SelectedConfig.Object.GetType() != configType){
        m_SelectedConfig.Object = ConfigManager.Instance.Load(falconImporter.assetPath, configType, SetHelpBox);
      }
      CandiedUI.Horizontal(() => {
        CandiedUI.Button("Load", () => {
          m_HelpBox.Set("Load completed", CandiedUI.MessageType.Info);
          m_SelectedConfig.Object.Load(SetHelpBox);
        });
        CandiedUI.Button("Reset", () => {
          m_HelpBox.Set("Reset completed", CandiedUI.MessageType.Info);
          m_SelectedConfig.Object.Reset(ConfigManager.Instance.Create(configType));
        });
      });
      m_SelectedConfig.Object.OnGUI();
      CandiedUI.Button("Save", () => {
        m_HelpBox.Set("Save completed", CandiedUI.MessageType.Info);
        ConfigManager.Instance.Save(m_SelectedConfig.Object, SetHelpBox);
        AssetDatabase.ImportAsset(m_SelectedConfig.Object.FilePath);
      });
      m_HelpBox.OnGUI();
    }

    private void SetHelpBox(Exception exception){
      m_HelpBox.Set(exception.Message, CandiedUI.MessageType.Error);
    }
  }
}
#endif