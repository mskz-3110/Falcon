using System;
using System.IO;

namespace Falcon {
  public class ConfigManager {
    static private ConfigManager s_Instance;
    static public ConfigManager Instance => s_Instance ??= new ConfigManager();

    public delegate void ExceptionEvent(Exception exception);

    public IConfigUtility ConfigUtility;

    public Config Create(Type type){
      return (Config)Activator.CreateInstance(type);
    }

    public Config Load(string filePath, Type type, ExceptionEvent onException = null){
      Config config;
      try{
        config = ConfigUtility.Load(filePath, type);
      }catch (Exception exception){
        onException?.Invoke(exception);
        config = Create(type);
      }
      config.FilePath = filePath;
      return config;
    }

    public T Load<T>(string filePath, ExceptionEvent onException = null) where T : Config {
      return (T)Load(filePath, typeof(T), onException);
    }

    public void Save(Config config, ExceptionEvent onException = null){
      try{
        string directoryPath = Path.GetDirectoryName(config.FilePath);
        if (!Directory.Exists(directoryPath)){
          Directory.CreateDirectory(directoryPath);
        }
        ConfigUtility.Save(config);
      }catch (Exception exception){
        onException?.Invoke(exception);
      }
    }
  }
}