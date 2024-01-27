using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Falcon {
  public class FalconReader {
    static private Dictionary<string, string> EscapeStrings = new Dictionary<string, string>(){
      {@"\\", "\\"},
      {@"\n", "\n"},
      {@"\r", "\r"},
      {@"\t", "\t"},
      {@"\""", "\""},
    };

    static public object Read(StreamReader streamReader, Type type){
      return new FalconReader(streamReader).Read(type);
    }

    static public object Read(string filePath, Type type){
      using (StreamReader streamReader = new StreamReader(filePath)){
        return Read(streamReader, type);
      }
    }

    static public string DecodeString(string value){
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string s in Regex.Split(value.Substring(1, value.Length - 2), @"(\\.)")){
        if (0 <= s.Length) stringBuilder.Append(EscapeStrings.ContainsKey(s) ? EscapeStrings[s] : s);
      }
      return stringBuilder.ToString();
    }

    private StreamReader m_StreamReader;

    private string m_Line;

    private Regex m_ValueRegex = new Regex("([ ]*)(.+)");

    private Regex m_FieldRegex = new Regex("([ ]*)([a-zA-Z]*?):(.*)");

    private FalconReader(StreamReader streamReader){
      m_StreamReader = streamReader;
    }

    private void SetLine(string line){
      m_Line = (0 < line.Length) ? line : null;
    }

    private string ReadLine(){
      string line = (m_Line != null) ? m_Line : (0 <= m_StreamReader.Peek()) ? m_StreamReader.ReadLine() : null;
      m_Line = null;
      return line;
    }

    private void SkipLines(int indentLevel){
      string line;
      while ((line = ReadLine()) != null){
        Match match = m_ValueRegex.Match(line);
        if (!match.Success || match.Groups[1].Length < indentLevel){
          SetLine(line);
          break;
        }
      }
    }

    private string GetValue(string line, int indentLevel){
      Match match = m_ValueRegex.Match(line);
      return (!match.Success || (match.Groups[1].Length != indentLevel)) ? null : match.Groups[2].ToString();
    }

    private IList CreateList(Type type, int indentLevel){
      IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));
      string line;
      while ((line = ReadLine()) != null){
        string value = GetValue(line, indentLevel);
        if (value == null){
          SetLine(line);
          break;
        }

        if (value == ":"){
          list.Add(CreateValue(type, indentLevel + 1));
        }else{
          SetLine(line);
          list.Add(CreateValue(type, indentLevel));
        }
      }
      return list;
    }

    private object CreateValue(Type type, int indentLevel){
      if (type.IsPrimitive) return Convert.ChangeType(GetValue(ReadLine(), indentLevel), type);
      if (type == typeof(string)) return DecodeString(GetValue(ReadLine(), indentLevel));

      if (type.IsArray){
        type = type.GetElementType();
        IList list = CreateList(type, indentLevel);
        Array array = Array.CreateInstance(type, list.Count);
        list.CopyTo(array, 0);
        return array;
      }
      if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>))){
        return CreateList(type.GetGenericArguments()[0], indentLevel);
      }

      object value = Activator.CreateInstance(type);
      ReadFields(value, indentLevel);
      return value;
    }

    private void ReadField(FieldInfo field, object value, int indentLevel){
      Type type = field.FieldType;
      if (type.IsPrimitive || (type == typeof(string))){
        field.SetValue(value, CreateValue(type, 0));
        return;
      }

      if (type.IsArray){
        field.SetValue(value, CreateValue(type, indentLevel + 1));
        return;
      }

      if (type.IsGenericType){
        if (type.GetGenericTypeDefinition() == typeof(List<>)){
          field.SetValue(value, CreateValue(type, indentLevel + 1));
          return;
        }
      }

      field.SetValue(value, CreateValue(type, indentLevel + 1));
    }

    private void ReadFields(object value, int indentLevel){
      Type type = value.GetType();
      string line;
      while ((line = ReadLine()) != null){
        Match match = m_FieldRegex.Match(line);
        if (match.Success){
          if (match.Groups[1].Length != indentLevel){
            SetLine(line);
            break;
          }

          FieldInfo field = type.GetField(match.Groups[2].ToString());
          if (field != null){
            SetLine(match.Groups[3].ToString());
            ReadField(field, value, indentLevel);
          }else{
            SkipLines(indentLevel + 1);
          }
          continue;
        }
        throw new InvalidDataException($"'{line}'");
      }
    }

    private object Read(Type type){
      object value = Activator.CreateInstance(type);
      ReadFields(value, 0);
      return value;
    }
  }
}