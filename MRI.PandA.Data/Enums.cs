using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRI.PandA.Data
{
    public enum ImportType : int
    {
        All,
        Property,
        UnitType,
        Unit,
        Media,
        Amenity,
        Rent,
        Availability
    }

    public enum SourceType : int
    {
        AlloyApi,
        MixApi,
        RealPageApi,
        YardiApi
    }

    public enum DestinationType : int
    {
        MarketConnect,
        Vaultware
    }
}
