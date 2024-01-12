using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Falcon {
  public class FalconTestEditor {
    static public readonly string EditorDirectoryPath = "./Assets/Falcon/Tests/Editor";

    static public readonly string ParentConfigFilePath = $"{EditorDirectoryPath}/ParentConfig.json";

    static public readonly string ChildConfigFilePath = $"{EditorDirectoryPath}/ChildConfig.json";

    private class ParentConfig : Config {
      public string Key;

      public ParentConfig(){
        FilePath = ParentConfigFilePath;
        Key = "Parent";
      }

      public override void OnGUI(){
        Key = EditorGUILayout.TextField("Key", Key);
      }
    }

    private class ChildConfig : ParentConfig {
      public int Value;

      public ChildConfig(){
        FilePath = ChildConfigFilePath;
        Key = "Child";
        Value = 10;
      }

      public override void OnGUI(){
        base.OnGUI();
        Value = EditorGUILayout.IntField("Value", Value);
      }
    }

    private class TestConfig : Config {
      public string Path;
      public string Name;

      public TestConfig(){
        Path = EditorDirectoryPath;
        Name = "";
      }

      public override void OnGUI(){
        CandiedUI.Horizontal(() => {
          Path = EditorGUILayout.TextField("Path", Path);
          CandiedUI.Button("Select", () => {
            CandiedUI.SelectDirectory("Destination", Path, (path) => Path = path);
          });
          CandiedUI.Button("Open", () => {
            Application.OpenURL($"file://{System.IO.Path.GetFullPath(Path)}");
          });
        });
        Name = EditorGUILayout.TextField("Name", Name);
      }
    }

    static FalconTestEditor(){
      ConfigManager.Instance.ConfigUtility = new JsonConfigUtility();
    }

    [Test]
    public void ConfigTest(){
      var parentConfig = new ParentConfig();
      parentConfig.Load(Log);
      Assert.That(parentConfig.Key == "Parent");
      if (!parentConfig.Exists()){
        parentConfig.Save(Log);
      }

      var childConfig = new ChildConfig();
      childConfig.Load(Log);
      Assert.That(childConfig.Key == "Child");
      Assert.That(childConfig.Value == 10);
      if (!childConfig.Exists()){
        childConfig.Save(Log);
      }

      parentConfig.FilePath = childConfig.FilePath;
      parentConfig.Load(Log);
      Assert.That(parentConfig.Key == "Child");

      AssetDatabase.Refresh();
    }

    private void Log(Exception exception){
      Debug.LogWarning(exception.ToString());
    }
  }
}