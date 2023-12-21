using System;
using System.Collections.Generic;

namespace SyncAzureDurableFunctions.DataAccess;

public class Row : IRow {
  private readonly Dictionary<string, object> _values;

  public Row(Dictionary<string, object> values) {
    _values = values;
  }

  public object Get(string columnName) {
    return _values[columnName];
  }

  public decimal GetDecimal(string columnName) {
    return (decimal)_values[columnName];
  }

  public bool GetBoolean(string columnName) {
    if (_values[columnName] is bool) {
      return (bool)_values[columnName];
    }
    else if (_values[columnName] is string) {
      if (bool.TryParse((string)_values[columnName], out bool result)) {
        return result;
      }

      return string.Equals((string)_values[columnName], "Y", StringComparison.CurrentCultureIgnoreCase);
    }

    return Convert.ToBoolean(_values[columnName]);
  }

  public bool? GetNullableBoolean(string columnName) {
    return _values[columnName] != null
             ? GetBoolean(columnName)
             : (bool?)null;
  }

  public DateTime GetDateTime(string columnName) {
    return _values[columnName] != null ? (DateTime)_values[columnName] : DateTime.MinValue;
  }

  public DateTime? GetNullableDateTime(string columnName) {
    return _values[columnName] != null ? (DateTime)_values[columnName] : (DateTime?)null;
  }

  public short GetShort(string columnName) {
    return _values[columnName] != null ? (short)_values[columnName] : (short)0;
  }

  public short? GetNullableShort(string columnName) {
    return _values[columnName] != null ? (short)_values[columnName] : (short?)null;
  }

  public int GetInt(string columnName) {
    return _values[columnName] != null ? (int)_values[columnName] : 0;
  }

  public string GetString(string columnName) {
    return ((string)_values[columnName])?.Trim() ?? string.Empty;
  }

  public string GetNullableString(string columnName) {
    return _values[columnName] != null ? ((string)_values[columnName]).Trim() : null;
  }

  public byte[] GetByteArray(string columnName) {
    return _values[columnName] != null ? (byte[])_values[columnName] : null;
  }
}
