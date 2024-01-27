# Falcon
Fast and low cost configuration reading and writing

## Versions

|Version|Summary|
|:--|:--|
|1.1.0|Improved convenience & Supported dotnet|
|1.0.0|Initial version|

## Unity
`https://github.com/mskz-3110/Falcon.git?path=/Unity/Falcon/Assets/Falcon/Runtime#v1.1.0`

## Dotnet
`dotnet add package FalconConfig -v 1.1.0`

## Usage
```cs
using Falcon;

private class TestConfig : Config {
  public string Name;
  public int Value;

  public TestConfig(){
    FilePath = $"{GetType().Name}.flcn"; // TestConfig.flcn
    Name = "Test";
    Value = 1;
  }
}

var testConfig = new TestConfig();
testConfig.Save();
/* Contents of written TestConfig.flcn
Name:"Test"
Value:1
*/
testConfig.Load();
```
