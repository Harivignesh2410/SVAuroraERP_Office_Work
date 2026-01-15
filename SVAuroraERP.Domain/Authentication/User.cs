using SVAuroraERP.Domain.Online.Master;

namespace SVAuroraERP.Domain.Authentication
{
    [Table("tUser")]
    public class User
    {
        [Column("PK_UserID"), Key] public int UserID { get; set; }
        [Column("FK_RoleID")] public int RoleID { get; set; }
        [Column("FirstName"), MaxLength(100), Required(ErrorMessage = "Please enter First Name")] public string FirstName { get; set; } = string.Empty;
        [Column("LastName"), MaxLength(100), Required(ErrorMessage = "Please enter Last Name")] public string LastName { get; set; } = string.Empty;
        [Column("UserName"), MaxLength(150), Required(ErrorMessage = "Please enter User Name")] public string UserName { get; set; } = string.Empty;
        [Column("PasswordHash"), MaxLength(150), Required(ErrorMessage = "Please enter User Password")] public string PasswordHash { get; set; } = string.Empty;
        [Column("Email")] public string? Email { get; set; } = string.Empty;
        [Column("FK_LandingPageID")] public byte LandingPageID { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateDate")] public DateTime lastupdateddate { get; set; } = DateTime.UtcNow;
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("UserProfilePicURL")] public string? UserProfilePicURL { get; set; } = string.Empty;
        [NotMapped] public long LoginAuditID { get; set; }
    }

    [Table("V_UserLoginData")]
    public class UserLoginData
    {
        [Column("PK_UserID"), Key] public int UserID { get; set; }
        [Column("FirstName")] public string? FirstName { get; set; }
        [Column("LastName")] public string? LastName { get; set; }
        [Column("UserName")] public string UserName { get; set; } = string.Empty;
        [Column("FK_LandingPageID")] public byte LandingPageID { get; set; }
        [Column("PageName")] public string? PageName { get; set; }
        [Column("PageURL")] public string? PageURL { get; set; }
        [Column("FK_RoleID")] public int RoleID { get; set; }
        [Column("RoleName")] public string RoleName { get; set; } = string.Empty;
        [Column("LastLoginDate")] public DateTime? LastLoginDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }
        [Column("Email")] public string Email { get; set; } = string.Empty;
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("UserProfilePicURL")] public string? UserProfilePicURL { get; set; } = string.Empty;
        [Column("FK_ApplicationID")] public byte ApplicationID { get; set; }
        [Column("ApplicationName")] public string ApplicationName { get; set; } = string.Empty;
        [Column("FK_EmbossingStationID")] public int EmbossingStationID { get; set; }
        [Column("EmbossingStationName")] public string EmbossingStationName { get; set; } = string.Empty;
        [Column("FK_HSRPUserID")] public int HSRPUserID { get; set; }
        public VHSRPUser? HSRPUser { get; set; }
    }
    public class ChangePassword
    {
        [Required] public string CurrentPassword { get; set; } = string.Empty;
        [Required] public string NewPassword { get; set; } = string.Empty;
        [Required] public string ConfirmPassword { get; set; } = string.Empty;
        [Required] public int UserID { get; set; } = 0;
    }
    public class ChangePasswordRequest
    {
        public int UserID { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UserDataTableRequest:DataTableRequest
    {
        public byte ApplicationID { get; set; }
    }

}