using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Falcon {
  public class FalconTest {
    [Serializable]
    public class BaseConfig : Config {
      public BaseConfig(){
        FilePath = $"{GetType().Name}.flcn";
      }

      public void AddRootPath(string rootPath){
        FilePath = $"{rootPath}/{FilePath}";
      }
    }

    [Serializable]
    public class SimpleClass {
      public string Name;

      public SimpleClass(){
        Name = $"{GetType().Name}";
      }

      public SimpleClass(string name){
        Name = name;
      }
    }

    [Serializable]
    public struct SimpleStruct {
      public string Name;

      public SimpleStruct(string name){
        Name = name;
      }
    }

    [Serializable]
    public class ChildConfig : BaseConfig {
      public int Value;

      public ChildConfig(){
        Value = 3;
      }
    }

    [Serializable]
    public class ParentConfig : BaseConfig {
      public ChildConfig Child;
      public int Value;

      public ParentConfig(){
        Child = new ChildConfig();
        Value = 2;
      }
    }

    [Serializable]
    public class GrandConfig : BaseConfig {
      public ParentConfig Parent;
      public int Value;

      public GrandConfig(){
        Parent = new ParentConfig();
        Value = 1;
      }
    }

    public class SupportedTypesConfig : BaseConfig {
      public bool Bool;
      public byte Byte;
      public short Short;
      public int Int;
      public long Long;
      public float Float;
      public double Double;
      public string String;
      public SimpleClass Class;
      public SimpleStruct Struct;
      public string[] StringArray;
      public List<string> StringList;
      public List<SimpleClass> ClassList;
      public GrandConfig GrandConfig;
      public BaseConfig BaseConfig;
      public SimpleClass NullClass;

      public SupportedTypesConfig(){
        Byte = 1;
        Short = 2;
        Int = 3;
        Long = 4;
        Float = 5.5f;
        Double = 6.6;
        String = "\n";
        Class = new SimpleClass();
        Struct = new SimpleStruct("\t");
        StringArray = new string[]{"AIUEO", " Key:Value ", "'"};
        StringList = new List<string>(){"あいうえお"};
        ClassList = new List<SimpleClass>(){
          new SimpleClass("\":\""),
          new SimpleClass("\\n"),
          new SimpleClass("A:1")
        };
        GrandConfig = new GrandConfig();
        BaseConfig = new BaseConfig();
      }

      public void Assert(){
        Debug.Assert(!Bool);
        Debug.Assert(Byte == 1);
        Debug.Assert(Short == 2);
        Debug.Assert(Int == 3);
        Debug.Assert(Long == 4);
        Debug.Assert(Float == 5.5f);
        Debug.Assert(Double == 6.6);
        Debug.Assert(String == "\n");
        Debug.Assert(Class.Name == $"{typeof(SimpleClass).Name}");
        Debug.Assert(Struct.Name == "\t");
        Debug.Assert(StringArray.Length == 3);
        Debug.Assert(StringArray[0] == "AIUEO");
        Debug.Assert(StringArray[1] == " Key:Value ");
        Debug.Assert(StringArray[2] == "'");
        Debug.Assert(StringList.Count == 1);
        Debug.Assert(StringList[0] == "あいうえお");
        Debug.Assert(ClassList.Count == 3);
        Debug.Assert(ClassList[0].Name == "\":\"");
        Debug.Assert(ClassList[1].Name == "\\n");
        Debug.Assert(ClassList[2].Name == "A:1");
        Debug.Assert(GrandConfig.Parent.Child.Value == 3);
        Debug.Assert(GrandConfig.Parent.Value == 2);
        Debug.Assert(GrandConfig.Value == 1);
        Debug.Assert(BaseConfig != null);
        Debug.Assert(NullClass == null);
      }
    }

    static public void ConfigTest(string rootPath){
      var supportedTypesConfig = new SupportedTypesConfig();
      supportedTypesConfig.FilePath = $"{rootPath}/{supportedTypesConfig.FilePath}";
      supportedTypesConfig.Assert();
      supportedTypesConfig.Load(Log);
      supportedTypesConfig.Assert();
      if (!supportedTypesConfig.Exists()){
        supportedTypesConfig.Save(Log);
        supportedTypesConfig.Assert();
      }

      var grandConfig = new GrandConfig();
      grandConfig.AddRootPath(rootPath);
      Debug.Assert(grandConfig.Parent.Child.Value == 3);
      Debug.Assert(grandConfig.Parent.Value == 2);
      Debug.Assert(grandConfig.Value == 1);
      grandConfig.Load(Log);
      if (!grandConfig.Exists()){
        grandConfig.Parent.Child.Value += 10;
        grandConfig.Parent.Value += 10;
        grandConfig.Value += 10;
        grandConfig.Save(Log);
      }
      Debug.Assert(grandConfig.Parent.Child.Value == 13);
      Debug.Assert(grandConfig.Parent.Value == 12);
      Debug.Assert(grandConfig.Value == 11);
    }

    static public void Log(Exception exception){
#if UNITY_EDITOR
      UnityEngine.Debug.LogError($"{exception}");
#else
      Console.WriteLine($"{exception}");
#endif
    }
  }
}