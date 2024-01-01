# Falcon
Fast and low cost configuration reading and writing

## Unity
`https://github.com/mskz-3110/Falcon.git?path=/Unity/Falcon/Assets/Falcon/Runtime#v1.0.0`

|Version|Summary|
|:--|:--|
|1.0.0|Initial version|

## Usage
```cs
using Falcon;

private class TestConfig : Config {
  public string Name;

  public TestConfig(){
    Name = "";
  }

#if UNITY_EDITOR
  public override void OnGUI(){
    Name = UnityEditor.EditorGUILayout.TextField("Name", Name);
  }
#endif
}

var testConfig = UnityConfigManager.LoadJson<TestConfig>(/* ファイルパス */);
```
