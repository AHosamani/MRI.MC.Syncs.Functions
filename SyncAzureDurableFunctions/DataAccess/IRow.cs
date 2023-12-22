using System;

namespace SyncAzureDurableFunctions.DataAccess;

public interface IRow {
  object Get(string columnName);
  decimal GetDecimal(string columnName);
  bool GetBoolean(string columnName);
  bool? GetNullableBoolean(string columnName);
  DateTime GetDateTime(string columnName);
  DateTime? GetNullableDateTime(string columnName);
  short GetShort(string columnName);
  short? GetNullableShort(string columnName);
  int GetInt(string columnName);
  string GetString(string columnName);
  string GetNullableString(string columnName);
  byte[] GetByteArray(string columnName);
}