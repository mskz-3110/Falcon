using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Falcon {
  public class FalconTestEditor {
    public const string EditorDirectoryPath = "./Assets/Falcon/Tests/Editor";

    public readonly string ParentConfigFilePath = $"{EditorDirectoryPath}/ParentConfig.json";

    public readonly string ChildConfigFilePath = $"{EditorDirectoryPath}/ChildConfig.json";

    private class ParentConfig : Config {
      public string Key;

      public ParentConfig(){
        Key = "Parent";
      }

      public override void OnGUI(){
        Key = EditorGUILayout.TextField("Key", Key);
      }
    }

    private class ChildConfig : ParentConfig {
      public int Value;

      public ChildConfig(){
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
        UnityUI.Horizontal(() => {
          Path = EditorGUILayout.TextField("Path", Path);
          UnityUI.Button("Select", () => {
            UnityUI.SelectDirectory("Destination", Path, (path) => Path = path);
          });
          UnityUI.Button("Open", () => {
            Application.OpenURL($"file://{System.IO.Path.GetFullPath(Path)}");
          });
        });
        Name = EditorGUILayout.TextField("Name", Name);
      }
    }

    [Test]
    public void ConfigTest(){
      var parentConfig = UnityConfigManager.LoadJson<ParentConfig>(ParentConfigFilePath, Log);
      Assert.That(parentConfig.Key == "Parent");
      if (!parentConfig.Exists()){
        UnityConfigManager.SaveJson(parentConfig, Log);
      }

      var childConfig = UnityConfigManager.LoadJson<ChildConfig>(ChildConfigFilePath, Log);
      Assert.That(childConfig.Key == "Child");
      Assert.That(childConfig.Value == 10);
      if (!childConfig.Exists()){
        UnityConfigManager.SaveJson(childConfig, Log);
      }

      parentConfig.FilePath = ChildConfigFilePath;
      UnityConfigManager.OverwriteJson(parentConfig, Log);
      Assert.That(parentConfig.Key == "Child");

      AssetDatabase.Refresh();
    }

    private void Log(Exception exception){
      Debug.LogWarning(exception.ToString());
    }
  }
}