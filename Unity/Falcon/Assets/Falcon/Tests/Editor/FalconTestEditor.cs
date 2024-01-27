using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Falcon {
  public class FalconTestEditor {
    static public readonly string EditorDirectoryPath = "./Assets/Falcon/Tests/Editor";

    private class TestConfig : Config, IConfigEditor {
      public string Path;
      public string Name;

      public TestConfig(){
        Path = EditorDirectoryPath;
        Name = "";
      }

      void IConfigEditor.OnGUI(){
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

    [Test]
    public void ConfigTest(){
      FalconTest.ConfigTest(EditorDirectoryPath);

      var testConfig = new TestConfig();
      testConfig.FilePath = $"{EditorDirectoryPath}/{typeof(TestConfig).Name}1.flcn";
      testConfig.Name = "Test1";
      if (!testConfig.Exists()) testConfig.Save();

      testConfig.FilePath = $"{EditorDirectoryPath}/{typeof(TestConfig).Name}2.flcn";
      testConfig.Name = "Test2";
      if (!testConfig.Exists()) testConfig.Save();

      AssetDatabase.Refresh();
    }
  }
}