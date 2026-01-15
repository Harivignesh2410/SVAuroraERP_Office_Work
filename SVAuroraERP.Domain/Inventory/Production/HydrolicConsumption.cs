
//Added on 2025.06.02 by Harivignesh

using System.Data;

namespace SVAuroraERP.Domain.Inventory.Production
{
    [Table("tHydrolicConsumption")]
    public class HydrolicConsumption
    {
        [Column("PK_HydrolicConsumptionID"), Key] public int HydrolicConsumptionID { get; set; }
        [Column("FK_HydrolicPressureID")] public int HydrolicPressureID { get; set; }
        [Column("FK_StockRequestTransID")] public int StockRequestTransID { get; set; }
        [Column("ActualConsumedQty")] public decimal ActualConsumedQty { get; set; }
        [Column("WastageQty")] public decimal WastageQty { get; set; }
        [Column("WastagePercentage")] public decimal WastagePercentage { get; set; }
        [Column("BalanceQty")] public decimal BalanceQty { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
    }
    
}
