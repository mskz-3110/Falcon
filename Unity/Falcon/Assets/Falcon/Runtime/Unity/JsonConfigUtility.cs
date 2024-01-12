namespace Falcon {
  public class JsonConfigUtility : IConfigUtility {
    Config IConfigUtility.Load(string filePath, System.Type type){
      return (Config)UnityEngine.JsonUtility.FromJson(System.IO.File.ReadAllText(filePath), type);
    }

    void IConfigUtility.Save(Config config){
      System.IO.File.WriteAllText(config.FilePath, UnityEngine.JsonUtility.ToJson(config, true));
    }
  }
}