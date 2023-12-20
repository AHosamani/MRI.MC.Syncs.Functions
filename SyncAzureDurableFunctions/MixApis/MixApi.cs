using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;

namespace MRI.PandA.Syncs.Functions.MixApis;

public class MixApi {
  public const string ENTRY_TAG = "<entry>";
  public const string DEFAULT_PROPERTY_PARAMETER = "RMPROPID";
  public const string DEFAULT_FORMAT_PARAMETER = "$format";
  public const string DEFAULT_FORMAT_VALUE = "xml";
  public const string DEFAULT_ROWS_PER_REQUEST_PARAMETER = "$top";
  public const int DEFAULT_ROWS_PER_REQUEST_VALUE = 5000;

  public MixApi(string property) {
    Parameters = new List<MixApiParameter>()
    {
      new MixApiParameter() { Parameter = DEFAULT_FORMAT_PARAMETER, Value = DEFAULT_FORMAT_VALUE },
      new MixApiParameter() { Parameter = DEFAULT_PROPERTY_PARAMETER, Value = property },
      new MixApiParameter() { Parameter= DEFAULT_ROWS_PER_REQUEST_PARAMETER, Value = DEFAULT_ROWS_PER_REQUEST_VALUE.ToString() }
    };
  }

  public string Name { get; set; }
  public List<MixApiParameter> Parameters { get; set; }

  public new string ToString() {
    var api = Name;

    foreach (var parameter in Parameters) {
      api += $"&{parameter.ToString()}";
    }

    return api;
  }
}
