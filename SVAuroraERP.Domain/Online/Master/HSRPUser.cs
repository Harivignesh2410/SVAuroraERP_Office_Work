using System.Reflection.Metadata;
using SVAuroraERP.Domain.Authentication;

namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tHSRPUser")]
    public class HSRPUser
    {
        [Column("PK_HSRPUserID"), Key] public int HSRPUserID { get; set; }
        [Column("HSRPUserCode")] public string? HSRPUserCode { get; set; }
        [Column("CompanyName")] public string? CompanyName { get; set; }
        [Column("Address1")] public string? Address1 { get; set; }
        [Column("Address2")] public string? Address2 { get; set; }
        [Column("FK_DistrictID")] public int DistrictID { get; set; }
        [Column("City")] public string? City { get; set; }
        [Column("Pincode")] public string? Pincode { get; set; }
        [Column("GSTIN")] public string? GSTIN { get; set; }
        [Column("ContactPerson")] public string? ContactPerson { get; set; }
        [Column("ContactNo")] public string? ContactNo { get; set; }
        [Column("FK_HSRPUserTypeID")] public byte HSRPUserTypeID { get; set; }
        [Column("FK_OEMID")] public int? OEMID { get; set; }
        [Column("DeliveryAddress1")] public string? DeliveryAddress1 { get; set; }
        [Column("DeliveryAddress2")] public string? DeliveryAddress2 { get; set; }
        [Column("FK_DeliveryDistrictID")] public int DeliveryDistrictID { get; set; }
        [Column("DeliveryCity")] public string? DeliveryCity { get; set; }
        [Column("DeliveryPincode")] public string? DeliveryPincode { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("FK_DealerID")] public int? DealerID { get; set; }
        [Column("FK_EmbossingStationID")] public int? EmbossingStationID { get; set; }
        // Added On 2025.09.10
        [Column("OnlineOEMName")] public string? OnlineOEMName { get; set; }
        [Column("IsOEMEnabledOnline")] public bool IsOEMEnabledOnline { get; set; }
        // Added On 2025.09.11
        [Column("IsDealerEnabledOnline")] public bool IsDealerEnabledOnline { get; set; }
        // Added On 2025.09.24
        [Column("PasswordHash")] public string? PasswordHash { get; set; }
        [Column("FK_USERID")] public int? UserID { get; set; }
        [NotMapped] public User? Userdata { get; set; }
    }

    [Table("V_HSRPUser")]
    public class VHSRPUser
    {
        [Column("PK_HSRPUserID"), Key] public int HSRPUserID { get; set; }
        [Column("FK_HSRPUserTypeID")] public byte HSRPUserTypeID { get; set; }
        [Column("HSRPUserTypeName")] public string? HSRPUserTypeName { get; set; }
        [Column("HSRPUserCode")] public string? HSRPUserCode { get; set; }
        [Column("CompanyName")] public string? CompanyName { get; set; }
        [Column("Address1")] public string? Address1 { get; set; }
        [Column("Address2")] public string? Address2 { get; set; }
        [Column("FK_StateID")] public int StateID { get; set; }
        [Column("StateName")] public string? StateName { get; set; }
        [Column("FK_DistrictID")] public int DistrictID { get; set; }
        [Column("DistrictName")] public string? DistrictName { get; set; }
        [Column("City")] public string? City { get; set; }
        [Column("Pincode")] public string? Pincode { get; set; }
        [Column("GSTIN")] public string? GSTIN { get; set; }
        [Column("ContactPerson")] public string? ContactPerson { get; set; }
        [Column("ContactNo")] public string? ContactNo { get; set; }
        [Column("FK_OEMID")] public int OEMID { get; set; }
        [Column("OEMName")] public string? OEMName { get; set; }
        [Column("DeliveryAddress1")] public string? DeliveryAddress1 { get; set; }
        [Column("DeliveryAddress2")] public string? DeliveryAddress2 { get; set; }
        [Column("DeliveryStateID")] public int DeliveryStateID { get; set; }
        [Column("DeliveryStateName")] public string? DeliveryStateName { get; set; }
        [Column("FK_DeliveryDistrictID")] public int DeliveryDistrictID { get; set; }
        [Column("DeliveryDistrictName")] public string? DeliveryDistrict { get; set; }
        [Column("DeliveryCity")] public string? DeliveryCity { get; set; }
        [Column("DeliveryPincode")] public string? DeliveryPincode { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("FK_DealerID")] public int? DealerID { get; set; }
        [Column("EmbossingStationName")] public string? EmbossingStationName { get; set; }
        [Column("FK_EmbossingStationID")] public int? EmbossingStationID { get; set; }
        [Column("DealerName")] public string? DealerName { get; set; }
        [Column("OEMCompanyName")] public string? OEMCompanyName { get; set; }
        // Added On 2025.09.10        
        [Column("IsOEMEnabledOnline")] public bool IsOEMEnabledOnline { get; set; }
        [Column("OnlineOEMName")] public string? OnlineOEMName { get; set; }

        // Added On 2025.09.11
        [Column("IsDealerEnabledOnline")] public bool IsDealerEnabledOnline { get; set; }

        //Below used for DeliverymyHSRP.com
        [Column("OEMOnlineEnabledFlag")] public bool OEMOnlineEnabledFlag { get; set; }
        [Column("OEMOnlineName")] public string? OEMOnlineName { get; set; }


        [Column("FK_USERID")] public int? UserID { get; set; }
        [Column("UserName")] public string? UserName { get; set; }
        [Column("PasswordHash")] public string? PasswordHash { get; set; }
        [Column("Email")] public string? Email { get; set; }
        [Column("FK_RoleID")] public int? RoleID { get; set; }
        [Column("RoleName")] public string? RoleName { get; set; }
        [Column("FK_ApplicationID")] public byte? ApplicationID { get; set; }
        [Column("ApplicationName")] public string? ApplicationName { get; set; }
        [Column("FK_LandingPageID")] public byte? LandingPageID { get; set; }
        [Column("PageName")] public string? PageName { get; set; }
        [NotMapped,Column("LoginAuditID")] public long? LoginAuditID { get; set; }
    }

    [Table("LkupHSRPUserType")]
    public class HSRPUserType
    {
        [Column("PK_HSRPUserTypeID"), Key] public byte HSRPUserTypeID { get; set; }
        [Column("HSRPUserTypeName")] public string? HSRPUserTypeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("LkupApplication")]
    public class LkupApplication
    {
        [Column("PK_ApplicationID"), Key] public byte ApplicationID { get; set; }
        [Column("ApplicationName")] public string? ApplicationName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_DealerByOEM")]
    public class VDealerByOEM
    {
        [Column("PK_HSRPUserID"), Key] public int HSRPUserID { get; set; }
        [Column("FK_HSRPUserTypeID")] public byte HSRPUserTypeID { get; set; }

        [Column("HSRPUserTypeName")] public string? HSRPUserTypeName { get; set; }

        [Column("HSRPUserCode")] public string? HSRPUserCode { get; set; }

        [Column("CompanyName")] public string? CompanyName { get; set; }

        [Column("Address1")] public string? Address1 { get; set; }

        [Column("Address2")] public string? Address2 { get; set; }

        [Column("FK_StateID")] public int StateID { get; set; }

        [Column("StateName")] public string? StateName { get; set; }

        [Column("FK_DistrictID")] public int DistrictID { get; set; }

        [Column("DistrictName")] public string? DistrictName { get; set; }

        [Column("City")] public string? City { get; set; }

        [Column("Pincode")] public string? Pincode { get; set; }

        [Column("GSTIN")] public string? GSTIN { get; set; }

        [Column("ContactPerson")] public string? ContactPerson { get; set; }

        [Column("ContactNo")] public string? ContactNo { get; set; }

        [Column("FK_OEMID")] public int OEMID { get; set; }

        [Column("OEMName")] public string? OEMName { get; set; }

        [Column("DeliveryAddress1")] public string? DeliveryAddress1 { get; set; }

        [Column("DeliveryAddress2")] public string? DeliveryAddress2 { get; set; }

        [Column("DeliveryStateID")] public int DeliveryStateID { get; set; }

        [Column("DeliveryStateName")] public string? DeliveryStateName { get; set; }

        [Column("FK_DeliveryDistrictID")] public int DeliveryDistrictID { get; set; }

        [Column("DeliveryDistrictName")] public string? DeliveryDistrictName { get; set; }

        [Column("DeliveryCity")] public string? DeliveryCity { get; set; }

        [Column("DeliveryPincode")] public string? DeliveryPincode { get; set; }

        [Column("IsActive")] public bool IsActive { get; set; }

        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }

        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }

        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
    public class OEMDataTableRequest : DataTableRequest
    {
        public int OEMID { get; set; }
    }
    public enum HSRPUserTypeEnum : byte
    {
        Admin = 1,
        EmbossingStation = 2,
        OEM = 3,
        Dealer = 4,
        DealerSubUsers = 5,
        EmbossingSubUsers = 6,
        OEMSubUsers = 7
    }
    public class HSRPUserRequest : DataTableRequest
    {
        public byte? UserTypeID {get; set;}
    }
}