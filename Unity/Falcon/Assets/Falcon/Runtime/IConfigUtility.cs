using System;

namespace Falcon {
  public interface IConfigUtility {
    Config Load(string filePath, Type type);

    void Save(Config config);
  }
}