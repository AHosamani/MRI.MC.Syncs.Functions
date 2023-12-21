using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MRI.PandA.Data.DataModel;

namespace MRI.PandA.Data;

public interface IPropertyDataService {
  public Task<Property> GetProperty(int propertyId);
  public Task<IEnumerable<Property>> GetPropertiesToSync();
}

public class PropertyDataService : IPropertyDataService {

  private readonly IDbConnection _connection;

  public PropertyDataService(IDbConnection connection)
  {
    _connection = connection;
  }

  public async Task<Property> GetProperty(int propertyId) {
    return await Task.FromResult<Property>(null);
  }

  public async Task<IEnumerable<Property>> GetPropertiesToSync()
  {
    var command = @"SELECT * FROM [Client] c
                    JOIN [Property] p ON c.[Id] = p.[ClientId]
                    JOIN [SalesForceLicenses] s ON p.[Id] = s.[PropertyId]
                    WHERE p.[IsActive] = 1
                    AND s.[Status] = @status
                    AND CAST(s.[StartDate] AS DATE) <= CAST(@date AS DATE) AND CAST(s.[EndDate] AS DATE) >= CAST(@date AS DATE)
                    ORDER BY c.[Id], p.[Id]";

    var parameters = new { status = "Active", date = DateTime.UtcNow };
    
    return await _connection.QueryAsync<Property>(command, parameters);
  }

  public async Task<bool> PropertyHasCurrentLicense(string propertyId)
  {
    var command = @"SELECT TOP 1 1 FROM SalesForceLicenses
                    WHERE [PropertyId] = @propertyId
                    AND [Status] = @status
                    AND CAST(StartDate AS DATE) <= CAST(@date AS DATE)
                    AND CAST(EndDate AS DATE) >= CAST(@date AS DATE)";
    
    var parameters = new { propertyId, status = "Active", date = DateTime.UtcNow };

    return await _connection.ExecuteScalarAsync<bool>(command, parameters);
  }

  public PropertyPreferences GetPropertyPreferences(string propertyId)
  {
    var command = @"SELECT * FROM Preferences_Property WHERE PropertyId = @propertyId";

    var parameters = new { propertyId };
    
    return _connection.Query<PropertyPreferences>(command, parameters).FirstOrDefault();
  }

  public Dictionary<string,string> GetPropertyInfo(string propertyId) {
    var command = "SELECT [id],[clientid],[refid] FROM [dbo].[Property] WHERE id = @propertyId";
    var parameters = new { propertyId };
    var res = _connection.Query<dynamic>(command, parameters);
    Dictionary<string, string> list = new Dictionary<string, string>();
    foreach (var i in res) {
      list.Add("propertyId", i.id);
      list.Add("clientid", i.clientid);
      list.Add("ids", i.refid);
    }
    return list;
  }

  public void UpdatePropertyInformation(string propertyId, string propertyName, int addressId)
  {
    var command = @"UPDATE [Property] SET
                        [NamePms] = @propertyName,
                        [AddressId] = @addressId
                    WHERE [Id] = @propertyId";
    
    var parameters = new { propertyId, propertyName, addressId };

    _connection.Execute(command, parameters);
  }
}