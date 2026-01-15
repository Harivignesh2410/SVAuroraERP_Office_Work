//Added on 2025/12/24 by Harivignesh
namespace SVAuroraERP.Domain.OnlineOrders
{
    public class OnlineHSRPOrder
    {
    }
    [Table("V_OnlineHSRPOrder")]
    public class VOnlineHSRPOrder
    {
        [Key]
        [Column("PK_OnlineHSRPOrderID")]
        public int OnlineHSRPOrderID { get; set; }

        [Column("OrderNo")]
        public string? OrderNo { get; set; }

        [Column("OrderDate")]
        public DateTime OrderDate { get; set; }

        [Column("sOrderDate")]
        public string? sOrderDate { get; set; }

        [Column("FK_OEMID")]
        public int? OEMID { get; set; }

        [Column("OEMName")]
        public string? OEMName { get; set; }

        [Column("VehicleNo")]
        public string? VehicleNo { get; set; }

        [Column("ChasisNo")]
        public string? ChasisNo { get; set; }

        [Column("EngineNo")]
        public string? EngineNo { get; set; }

        [Column("FK_VehicleClassID")]
        public int? VehicleClassID { get; set; }

        [Column("VehicleClassName")]
        public string? VehicleClassName { get; set; }

        [Column("FK_HSRPPlateTypeID")]
        public byte? HSRPPlateTypeID { get; set; }  

        [Column("VehiclePlateType")]
        public string? VehiclePlateType { get; set; }

        [Column("FK_VehiclePlateSizeID")]
        public int? VehiclePlateSizeID { get; set; }

        [Column("VehiclePlateSizeName")]
        public string? VehiclePlateSizeName { get; set; }

        [Column("FK_VehiclePlateColorID")]
        public int? VehiclePlateColorID { get; set; }

        [Column("VehiclePlateColorName")]
        public string? VehiclePlateColorName { get; set; }

        [Column("FK_FitmentTypeID")]
        public byte? FitmentTypeID { get; set; }    

        [Column("FitmentTypeName")]
        public string? FitmentTypeName { get; set; }

        [Column("FK_DealerID")]
        public int? DealerID { get; set; }

        [Column("DealerName")]
        public string? DealerName { get; set; }

        [Column("FK_HSRPOrderID")]
        public int? HSRPOrderID { get; set; }

        [Column("FK_OrderStatusID")]
        public byte OrderStatusID { get; set; }      

        [Column("HSRPOnlineOrderStatus")]
        public string? OrderStatusName { get; set; }

        [Column("ColorCode")]
        public string? ColorCode { get; set; }

        [Column("LastUpdatedDate")]
        public DateTime LastUpdatedDate { get; set; }
    }


    public class OnlineOrderDTRequest : DataTableRequest
    {
        public DateTime? StartDate { get; set; }
        public string? sStartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? sEndDate { get; set; }
        public int? DealerID { get; set; }
        public int? OEMID { get; set; }
    }

    public class Approvedata
    {
        public int? EmbossingStationID { get; set; }
        public int OnlineHSRPOrderID { get; set; }
        public int LastUpdatedBy { get; set; }
    }

    [Table("V_OnlineReplacementOrderDetails")]
    public class VOnlineReplacementOrderDetails
    {
        [Key]
        [Column("OnlineHSRPReplacementOrderID")]
        public int OnlineHSRPReplacementOrderID { get; set; }

        [Column("OrderNo")]
        public string? OrderNo { get; set; }

        [Column("OrderDate")]
        public DateTime OrderDate { get; set; }

        [Column("VehicleNo")]
        public string? VehicleNo { get; set; }

        [Column("ChasisNo")]
        public string? ChasisNo { get; set; }

        [Column("EngineNo")]
        public string? EngineNo { get; set; }

        [Column("OEMID")]
        public int? OEMID { get; set; }

        [Column("VehicleClassID")]
        public int? VehicleClassID { get; set; }

        [Column("HSRPPlateTypeID")]
        public byte? HSRPPlateTypeID { get; set; }

        [Column("VehiclePlateSizeID")]
        public int? VehiclePlateSizeID { get; set; }

        [Column("VehiclePlateColorID")]
        public int? VehiclePlateColorID { get; set; }

        [Column("FitmentTypeID")]
        public byte? FitmentTypeID { get; set; }

        [Column("DealerID")]
        public int? DealerID { get; set; }

        [Column("OrderLastUpdatedDate")]
        public DateTime? OrderLastUpdatedDate { get; set; }

        [Column("ReplacementReasonID")]
        public int? ReplacementReasonID { get; set; }

        [Column("IsFrontPlate")]
        public bool? IsFrontPlate { get; set; }

        [Column("IsRearPlate")]
        public bool? IsRearPlate { get; set; }

        [Column("DocumentPath")]
        public string? DocumentPath { get; set; }

        [Column("FrontImagePath")]
        public string? FrontImagePath { get; set; }

        [Column("RearImagePath")]
        public string? RearImagePath { get; set; }

        [Column("HSRPFrontLaserCode")]
        public string? HSRPFrontLaserCode { get; set; }

        [Column("HSRPRearLaserCode")]
        public string? HSRPRearLaserCode { get; set; }

        [Column("FuelType")]
        public string? FuelType { get; set; }

        [Column("VehicleType")]
        public string? VehicleType { get; set; }

        [Column("VehicleCategory")]
        public string? VehicleCategory { get; set; }

        [Column("EmissionNorm")]
        public string? EmissionNorm { get; set; }

        [Column("OnlineCustomerID")]
        public int? OnlineCustomerID { get; set; }

        [Column("CustomerName")]
        public string? CustomerName { get; set; }

        [Column("CustomerAddress")]
        public string? CustomerAddress { get; set; }

        [Column("CustomerArea")]
        public string? CustomerArea { get; set; }

        [Column("CustomerCity")]
        public string? CustomerCity { get; set; }

        [Column("StateID")]
        public int? StateID { get; set; }

        [Column("CustomerPhoneNo")]
        public string? CustomerPhoneNo { get; set; }

        [Column("CustomerEmail")]
        public string? CustomerEmail { get; set; }

        [Column("CustomerLastUpdatedDate")]
        public DateTime? CustomerLastUpdatedDate { get; set; }

        [Column("StateName")]
        public string? StateName { get; set; }

        [Column("StateCode")]
        public string? StateCode { get; set; }

        [Column("VehicleClassName")]
        public string? VehicleClassName { get; set; }

        [Column("VehicleClassCode")]
        public string? VehicleClassCode { get; set; }

        [Column("PlateTypeName")]
        public string? PlateTypeName { get; set; }

        [Column("PlateSizeName")]
        public string? PlateSizeName { get; set; }

        [Column("PlateSizeCode")]
        public string? PlateSizeCode { get; set; }

        [Column("PlateColorName")]
        public string? PlateColorName { get; set; }

        [Column("PlateColorCode")]
        public string? PlateColorCode { get; set; }

        [Column("ReplacementReasonName")]
        public string? ReplacementReasonName { get; set; }

        [Column("ReplacementReasonCode")]
        public string? ReplacementReasonCode { get; set; }

        [Column("DealerName")]
        public string? DealerName { get; set; }

        [Column("DealerCode")]
        public string? DealerCode { get; set; }

        [Column("DealerAddress1")]
        public string? DealerAddress1 { get; set; }

        [Column("DealerAddress2")]
        public string? DealerAddress2 { get; set; }

        [Column("DealerCity")]
        public string? DealerCity { get; set; }

        [Column("DealerStateName")]
        public string? DealerStateName { get; set; }

        [Column("DealerContactNo")]
        public string? DealerContactNo { get; set; }

        [Column("OEMName")]
        public string? OEMName { get; set; }

        [Column("OEMCode")]
        public string? OEMCode { get; set; }

        [NotMapped]
        public string? sOrderDate { get; set; }
    }

    public class ReplacementOrderDTRequest : DataTableRequest
    {
        public DateTime? StartDate { get; set; }
        public string? sStartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? sEndDate { get; set; }
        public int? DealerID { get; set; }
        public int? OEMID { get; set; }
    }

    public class ApproveReplacementOrderData
    {
        public int? EmbossingStationID { get; set; }
        public int OnlineHSRPReplacementOrderID { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}
