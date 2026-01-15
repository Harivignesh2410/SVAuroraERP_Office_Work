namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tState")]
    public class State
    {
        [Column("PK_StateID"), Key] public int StateID { get; set; }
        [Column("StateCode")] public string? StateCode { get; set; }
        [Column("StateName")] public string? StateName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_State")]
    public class VState
    {
        [Column("PK_StateID"), Key] public int StateID { get; set; }
        [Column("StateCode")] public string? StateCode { get; set; }
        [Column("StateName")] public string? StateName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}