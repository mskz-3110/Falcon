using System;

namespace Falcon {
  public class ConfigUtility : IConfigUtility {
    Config IConfigUtility.Load(string filePath, Type type){
      return (Config)FalconReader.Read(filePath, type);
    }

    void IConfigUtility.Save(Config config){
      FalconWriter.Write(config.FilePath, config);
    }
  }
}