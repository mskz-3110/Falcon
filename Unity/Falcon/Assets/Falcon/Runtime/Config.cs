using System.IO;
using System.Reflection;

namespace Falcon {
  public class Config {
    private string m_FilePath;
    public string FilePath {
      get => m_FilePath;
      set => m_FilePath = value;
    }

    public bool Exists(){
      return File.Exists(m_FilePath);
    }

    public void Reset(Config config){
      foreach (FieldInfo fieldInfo in config.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)){
        fieldInfo.SetValue(this, fieldInfo.GetValue(config));
      }
    }

    public void Load(ConfigManager.ExceptionEvent onException = null){
      Reset(ConfigManager.Instance.Load(m_FilePath, GetType(), onException));
    }

    public void Save(ConfigManager.ExceptionEvent onException = null){
      ConfigManager.Instance.Save(this, onException);
    }

    public virtual void OnGUI(){}
  }
}