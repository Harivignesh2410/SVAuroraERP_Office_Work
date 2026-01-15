namespace SVAuroraERP.Domain.HR
{
    [Table("tEmployee")]
    public class Employee
    {
        [Column("PK_EmployeeID"), Key] public int EmployeeID { get; set; }
        [Column("EmployeeCode")] public string? EmployeeCode { get; set; }
        [Column("EmployeeTypeID")] public byte EmployeeTypeID { get; set; }
        [Column("Gender")] public byte Gender { get; set; }
        [Column("FirstName")] public string? FirstName { get; set; }
        [Column("MiddleName")] public string? MiddleName { get; set; }
        [Column("SurName")] public string? SurName { get; set; }
        [Column("AddressLine1")] public string? AddressLine1 { get; set; }
        [Column("AddressLine2")] public string? AddressLine2 { get; set; }
        [Column("City")] public string? City { get; set; }
        [Column("State")] public string? State { get; set; }
        [Column("Zipcode")] public string? Zipcode { get; set; }
        [Column("TelNo1")] public string? TelNo1 { get; set; }
        [Column("TelNo2")] public string? TelNo2 { get; set; }
        [Column("MobileNo")] public string? MobileNo { get; set; }
        [Column("Email")] public string? Email { get; set; }
        [Column("PlaceofBirth")] public string? PlaceofBirth { get; set; }
        [Column("EmergencyRelationshipContactID")] public byte? EmergencyRelationshipContactID { get; set; }
        [Column("EmergencyContactName")] public string? EmergencyContactName { get; set; }
        [Column("EmergencyContactNo")] public string? EmergencyContactNo { get; set; }
        [Column("DOB")] public DateTime? DOB { get; set; }
        [NotMapped] public string? sDOB { get; set; }
        [Column("FatherName")] public string? FatherName { get; set; }
        [Column("FatherDOB")] public DateTime? FatherDOB { get; set; }
        [NotMapped] public string? sFatherDOB { get; set; }
        [Column("MotherName")] public string? MotherName { get; set; }
        [Column("MotherDOB")] public DateTime? MotherDOB { get; set; }
        [NotMapped] public string? sMotherDOB { get; set; }
        [Column("MaritalStatus")] public byte MaritalStatus { get; set; }
        [Column("SpouseName")] public string? SpouseName { get; set; }
        [Column("SpouseDOB")] public DateTime? SpouseDOB { get; set; }
        [NotMapped] public string? sSpouseDOB { get; set; }
        [Column("AnniversaryDate")] public DateTime? AnniversaryDate { get; set; }
        [NotMapped] public string? sAnniversaryDate { get; set; }
        [Column("ChildOneName")] public string? ChildOneName { get; set; }
        [Column("ChildOneDOB")] public DateTime? ChildOneDOB { get; set; }
        [NotMapped] public string? sChildOneDOB { get; set; }
        [Column("ChildTwoName")] public string? ChildTwoName { get; set; }
        [Column("ChildTwoDOB")] public DateTime? ChildTwoDOB { get; set; }
        [NotMapped] public string? sChildTwoDOB { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime? LastUpdatedDate { get; set; }
        [Column("FK_DesignationID")] public int DesignationID { get; set; }
        [Column("FK_BloodGroupID")] public byte? BloodGroupID { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
    }
    [Table("V_Employee")]
    public class VEmployee
    {
        [Column("PK_EmployeeID"), Key] public int EmployeeID { get; set; }
        [Column("EmployeeCode")] public string EmployeeCode { get; set; } = string.Empty;
        [Column("EmployeeTypeID")] public byte EmployeeTypeID { get; set; }
        [Column("Gender")] public byte Gender { get; set; }
        [Column("FirstName")] public string? FirstName { get; set; }
        [Column("MiddleName")] public string? MiddleName { get; set; }
        [Column("SurName")] public string? SurName { get; set; }
        [Column("AddressLine1")] public string? AddressLine1 { get; set; }
        [Column("AddressLine2")] public string? AddressLine2 { get; set; }
        [Column("City")] public string? City { get; set; }
        [Column("State")] public string? State { get; set; }
        [Column("Zipcode")] public string? Zipcode { get; set; }
        [Column("TelNo1")] public string? TelNo1 { get; set; }
        [Column("TelNo2")] public string? TelNo2 { get; set; }
        [Column("MobileNo")] public string? MobileNo { get; set; }
        [Column("Email")] public string? Email { get; set; }
        [Column("PlaceofBirth")] public string? PlaceofBirth { get; set; }
        [Column("EmergencyRelationshipContactID")] public byte? EmergencyRelationshipContactID { get; set; }
        [Column("EmergencyContactName")] public string? EmergencyContactName { get; set; }
        [Column("EmergencyContactNo")] public string? EmergencyContactNo { get; set; }
        [Column("DOB")] public DateTime? DOB { get; set; }
        [NotMapped] public string? sDOB { get; set; }
        [Column("FatherName")] public string? FatherName { get; set; }
        [Column("FatherDOB")] public DateTime? FatherDOB { get; set; }
        [NotMapped] public string? sFatherDOB { get; set; }
        [Column("MotherName")] public string? MotherName { get; set; }
        [Column("MotherDOB")] public DateTime? MotherDOB { get; set; }
        [NotMapped] public string? sMotherDOB { get; set; }
        [Column("MaritalStatus")] public byte MaritalStatus { get; set; }
        [Column("SpouseName")] public string? SpouseName { get; set; }
        [Column("SpouseDOB")] public DateTime? SpouseDOB { get; set; }
        [NotMapped] public string? sSpouseDOB { get; set; }
        [Column("AnniversaryDate")] public DateTime? AnniversaryDate { get; set; }
        [NotMapped] public string? sAnniversaryDate { get; set; }
        [Column("ChildOneName")] public string? ChildOneName { get; set; }
        [Column("ChildOneDOB")] public DateTime? ChildOneDOB { get; set; }
        [NotMapped] public string? sChildOneDOB { get; set; }
        [Column("ChildTwoName")] public string? ChildTwoName { get; set; }
        [Column("ChildTwoDOB")] public DateTime? ChildTwoDOB { get; set; }
        [NotMapped] public string? sChildTwoDOB { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("isDeleted"), DefaultValue(false)] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; } = DateTime.UtcNow;
        [Column("FK_DesignationID")] public int DesignationID { get; set; }
        [Column("DesignationName")] public string? DesignationName { get; set; }
        [Column("FK_BloodGroupID")] public byte? BloodGroupID { get; set; }
        [Column("LastUpdatedDateIST")] public DateTime LastUpdatedDateIST { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }

    [Table("LkupBloodGroup")]
    public class BloodGroup
    {
        [Column("PK_BloodGroupID"), Key] public byte BloodGroupID { get; set; }
        [Column("BloodGroup")] public string? Blood { get; set; }
        [Column("LastupdatedDate")] public DateTime? LastUpdateDate { get; set; } = DateTime.UtcNow;
    }

}
