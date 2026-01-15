namespace SVAuroraERP.Domain.Master
{
    [Table("tMachine")]
    public class Machine
    {
        [Column("PK_MachineID"), Key] public int MachineID { get; set; }
        [Column("FK_MachineTypeID")] public byte MachineTypeID { get; set; }
        [Column("MachineCode")] public string? MachineCode { get; set; }
        [Column("MachineName")] public string? MachineName { get; set; }
        [Column("Description")] public string? Description { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("lkupMachineType")]
    public class MachineType
    {
        [Column("PK_MachineTypeID"), Key] public byte MachineTypeID { get; set; }
        [Column("MachineTypeName")] public string? MachineTypeName { get; set; }
        [Column("OrdinalNo")] public byte OrdinalNo { get; set; }
    }
    [Table("V_Machine")]
    public class VMachine
    {
        [Column("PK_MachineID"), Key] public int MachineID { get; set; }
        [Column("FK_MachineTypeID")] public byte MachineTypeID { get; set; }
        [Column("MachineTypeName")] public string? MachineTypeName { get; set; }
        [Column("MachineCode")] public string? MachineCode { get; set; }
        [Column("MachineName")] public string? MachineName { get; set; }
        [Column("Description")] public string? Description { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}
