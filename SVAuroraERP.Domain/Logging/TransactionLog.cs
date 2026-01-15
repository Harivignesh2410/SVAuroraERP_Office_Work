namespace SVAuroraERP.Domain.Logging
{
    [Table("tTransactionLog")]
    public class TransactionLog
    {
        [Column("PK_TransactionLogID"), Key] public long TransactionLogID { get; set; }
        [Column("FK_LoginAuditID")] public long LoginAuditID { get; set; }
        [Column("TableName"), MaxLength(150)] public string? TableName { get; set; } = string.Empty;
        [Column("LogID"), MaxLength(50)] public string? LogID { get; set; }
        [Column("FK_ActionTypeID")] public byte? ActionTypeID { get; set; }
    }
}