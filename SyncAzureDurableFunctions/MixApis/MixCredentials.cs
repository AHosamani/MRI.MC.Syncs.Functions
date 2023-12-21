using System;
using System.Text;

namespace MRI.PandA.Syncs.Functions.MixApis;

public class MixCredentials {
  private readonly string _database;
  private readonly string _clientId;
  private readonly string _userName;
  private readonly string _password;
  private readonly string _apiKey;

  public MixCredentials(string database, string clientId, string userName, string password, string apiKey) {
    _database = database;
    _clientId = clientId;
    _userName = userName;
    _password = password;
    _apiKey = apiKey;
  }

  public override string ToString() {
    return $"{_clientId}/{_database}/{_userName}/{_apiKey}:{_password}";
  }

  public string ToAuthorizationString() {
    return Convert.ToBase64String(Encoding.UTF8.GetBytes(ToString()));
  }
}
