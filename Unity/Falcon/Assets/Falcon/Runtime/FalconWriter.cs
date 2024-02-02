using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Falcon {
  public class FalconWriter {
    static private Dictionary<string, string> EscapeStrings = new Dictionary<string, string>(){
      {"\\", @"\\"},
      {"\n", @"\n"},
      {"\r", @"\r"},
      {"\t", @"\t"},
      {"\"", @"\"""},
    };

    static private Regex EscapeStringRegex = new Regex(string.Join("|", EscapeStrings.Keys.Select(k => Regex.Escape(k))));

    static public void Write(StreamWriter streamWriter, object value){
      new FalconWriter(streamWriter, value);
    }

    static public void Write(string filePath, object value){
      using (StreamWriter streamWriter = new StreamWriter(filePath)){
        Write(streamWriter, value);
      }
    }

    static public string EncodeString(string value){
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("\"");
      stringBuilder.Append(EscapeStringRegex.Replace(value, m => EscapeStrings[m.Value]));
      stringBuilder.Append("\"");
      return stringBuilder.ToString();
    }

    private StreamWriter m_StreamWriter;

    private FalconWriter(StreamWriter streamWriter, object value){
      m_StreamWriter = streamWriter;
      WriteFields(value, 0);
    }

    private FieldInfo[] GetFields(Type type){
      return type.GetFields(BindingFlags.Public | BindingFlags.Instance);
    }

    private void WriteIndent(int indentLevel){
      for (int i = 0; i < indentLevel; ++i){
        m_StreamWriter.Write(" ");
      }
    }

    private void WriteLine(){
      m_StreamWriter.Write("\n");
    }

    private void WriteArray(Type type, ICollection collection, int indentLevel){
      if (collection.Count == 0){
          WriteLine();
          return;
      }

      FieldInfo[] fields = GetFields(type);
      foreach (object value in collection){
        WriteIndent(indentLevel);
        if (0 < fields.Length){
          m_StreamWriter.Write(":");
          WriteLine();
        }
        WriteValue(type, value, indentLevel);
      }
    }

    private void WriteValue(Type type, object value, int indentLevel){
      if (type.IsPrimitive){
        m_StreamWriter.Write(value.ToString());
        WriteLine();
        return;
      }

      if (type == typeof(string)){
        m_StreamWriter.Write(EncodeString((string)value));
        WriteLine();
        return;
      }

      if (type.IsArray){
        WriteLine();
        WriteArray(type.GetElementType(), (ICollection)value, indentLevel + 1);
        return;
      }

      if (type.IsGenericType){
        if (type.GetGenericTypeDefinition() == typeof(List<>)){
          WriteLine();
          WriteArray(type.GetGenericArguments()[0], (ICollection)value, indentLevel + 1);
          return;
        }
      }

      WriteFields(value, indentLevel + 1);
    }

    private void WriteField(FieldInfo field, object value, int indentLevel){
      if (value == null) return;
      WriteIndent(indentLevel);
      m_StreamWriter.Write(field.Name);
      m_StreamWriter.Write(":");
      if (0 < GetFields(field.FieldType).Length) WriteLine();
      WriteValue(field.FieldType, value, indentLevel);
    }

    private void WriteFields(object value, int indentLevel){
      FieldInfo[] fields = GetFields(value.GetType());
      if (0 == fields.Length){
        WriteLine();
        return;
      }

      foreach (FieldInfo field in fields){
        WriteField(field, field.GetValue(value), indentLevel);
      }
    }
  }
}