using System;
using System.IO;
using UnityEngine;

namespace Falcon {
  public class UnityConfigManager {
    public delegate void ExceptionEvent(Exception exception);

    static public T LoadJson<T>(string filePath, ExceptionEvent onException = null) where T : Config, new() {
      T config;
      try{
        config = JsonUtility.FromJson<T>(File.ReadAllText(filePath));
      }catch (Exception exception){
        onException?.Invoke(exception);
        config = new T();
      }
      config.FilePath = filePath;
      return config;
    }

    static public Config LoadJson(string filePath, Type type, ExceptionEvent onException = null){
      Config config;
      try{
        config = (Config)JsonUtility.FromJson(File.ReadAllText(filePath), type);
      }catch (Exception exception){
        onException?.Invoke(exception);
        config = (Config)Activator.CreateInstance(type);
      }
      config.FilePath = filePath;
      return config;
    }

    static public void OverwriteJson(Config config, ExceptionEvent onException = null){
      try{
        JsonUtility.FromJsonOverwrite(File.ReadAllText(config.FilePath), config);
      }catch (Exception exception){
        onException?.Invoke(exception);
      }
    }

    static public void SaveJson(Config config, ExceptionEvent onException = null){
      try{
        string directoryPath = Path.GetDirectoryName(config.FilePath);
        if (!Directory.Exists(directoryPath)){
          Directory.CreateDirectory(directoryPath);
        }
        File.WriteAllText(config.FilePath, JsonUtility.ToJson(config, true));
      }catch (Exception exception){
        onException?.Invoke(exception);
      }
    }
  }
}