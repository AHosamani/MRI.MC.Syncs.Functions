 using System;
 using System.Collections.Generic;
namespace SyncAzureDurableFunctions.Data.DomainModel;

 public enum AvailabilityStatus: int
 {
     Unavailable, 
     Available, 
     OnHold,
     MakeReady,
     Administrative
 }

 public class Unit : IdentifiableContent
 {
    public string UnitId { get; set; } 
    public string BuildingId { get; set; }
    public string ExcludeFromMarketing { get; set; }
    public string FloorplanId { get; set; }
    public int VacancyClass { get; set; }
    public bool Blocked { get; set; }
    public bool Available { get; set; }
    public DateTime? MakeReadyDate { get; set; }
    public DateTime? VacateDate { get; set; }
    public string AmenityTotal { get; set; }
    public DateTime UtcDate { get; set; }
    public string IsAvailableNow { get; set; }

 }
//     public class Unit : IdentifiableContent
//     {
//         public string Name { get; set; }
//         public string ShortDescription { get; set; }
//         public string LongDescription { get; set; }
//         public Address Address { get; set; }
//         public List<Rent> Rents { get; set; }
//         public Rent DefaultLeaseTermRent { get; set; }
//         public Rent NonOptimizedRent { get; set; }
//         public decimal SecurityDeposit { get; set; }
//         public AvailabilityStatus Status { get; set; }
//         public DateTime? MakeReadyDate { get; set; }
//         public DateTime? VacateDate { get; set; }
//         public List<Special> Specials { get; set; }
//         public List<Amenity> Amenities { get; set; }
//         public List<Media> Media { get; set; }
//         public string FloorplanId { get; set; }
//         public string FloorplanName { get; set; }
//         public string BuildingId { get; set; }
//         public string EntryFloor { get; set; }
//         public LivingSpace LivingSpace { get; set; }
//         public bool Blocked { get; set; }
//         public bool IsOnline { get; set; }
//         public decimal? EnergyEfficiencyRating { get; set; }
//         public IRow ExtraData { get; set; }
//         public override bool Equals(object obj)
//         {
//             return base.Equals(obj as Unit);
//         }
//         public override int GetHashCode()
//         {
//             return base.GetHashCode();
//         }
//     }
