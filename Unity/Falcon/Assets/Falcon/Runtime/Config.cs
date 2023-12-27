using System.IO;

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

    public virtual void OnGUI(){}
  }
}