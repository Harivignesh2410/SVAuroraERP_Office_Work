//Added on 2025.04.29 by Harivignesh
namespace SVAuroraERP.Domain.Inventory.Dispatch
{
    [Table("tNumberPlateDispatch")]
    public class NumberPlateDispatch
    {
        [Column("PK_NumberPlateDispatchID"), Key] public int NumberPlateDispatchID { get; set; }
        [Column("DispatchNo")] public string? DispatchNo { get; set; }
        [Column("DispatchDate")] public DateTime DispatchDate { get; set; }
        [NotMapped] public string? sDispatchDate { get; set; }
        [Column("ModeofTransportID")] public byte ModeofTransportID { get; set; }
        [Column("FK_CourierID")] public int? CourierID { get; set; }
        [Column("OwnVehicleDetails")] public string? OwnVehicleDetails { get; set; }
        [Column("DocketNo")] public string? DocketNo { get; set; }
        [Column("DocketBookingDate")] public DateTime DocketBookingDate { get; set; }
        [NotMapped] public string? sDocketBookingDate { get; set; }
        [Column("FK_EmbossingStationID")] public int EmbossingStationID { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public List<NumberPlateDispatchTrans> NumberPlateDispatchTrans { get; set; }
        [Column("StatusID")] public byte StatusID { get; set; }
    }

    [Table("V_NumberPlateDispatch")]
    public class VNumberPlateDispatch
    {
        [Column("PK_NumberPlateDispatchID"), Key] public int NumberPlateDispatchID { get; set; }
        [Column("DispatchNo")] public string DispatchNo { get; set; } = string.Empty;
        [Column("DispatchDate")] public string DispatchDate { get; set; } = string.Empty;
        [Column("ModeofTransportID")] public byte ModeofTransportID { get; set; }
        [Column("ModeofTransportName")] public string? ModeofTransportName { get; set; } = string.Empty;
        [Column("FK_CourierID")] public int FK_CourierID { get; set; }
        [Column("TransportDetails")] public string? TransportDetails { get; set; } = string.Empty;
        [Column("DocketNo")] public string? DocketNo { get; set; } = string.Empty;
        [Column("DocketBookingDate")] public string? DocketBookingDate { get; set; } = string.Empty;
        [Column("FK_EmbossingStationID")] public int EmbossingStationID { get; set; }
        [Column("EmbossingStationName")] public string? EmbossingStationName { get; set; } = string.Empty;
        [NotMapped] public List<VNumberPlateDispatchTrans> NumberPlateDispatchTrans { get; set; }
        [Column("StatusID")] public byte StatusID { get; set; }
        [Column("StatusName")] public string? StatusName { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("OwnVehicleDetails")] public string? OwnVehicleDetails { get; set; }
    }
    public class AcknowledgeRequest
    {
        public List<int> PackingTransIDs { get; set; }
    }

}
