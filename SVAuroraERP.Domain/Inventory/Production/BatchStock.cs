namespace SVAuroraERP.Domain.Inventory.Production
{
    [Table("V_BatchStock")]
    public class BatchStock
    {
        [Column("PK_BatchStockID"), Key] public int BatchStockID { get; set; }
        [Column("FK_ItemID")] public int ItemID { get; set; }
        [Column("BatchNo")] public string? BatchNo { get; set; }
        [Column("BatchQuantity")] public decimal BatchQuantity { get; set; }
        [Column("ConsumedQty")] public decimal ConsumedQty { get; set; }
        [Column("BalanceQty")] public decimal BalanceQty { get; set; }
        [Column("ItemName")] public string? ItemName { get; set; }
        [Column("FK_ComponentTypeID")] public int ComponentTypeID { get; set; }
        [Column("ComponentTypeName")] public string? ComponentTypeName { get; set; }
        [Column("FK_SizeID")] public int SizeID { get; set; }
        [Column("SizeName")] public string? SizeName { get; set; }
        [Column("FK_ColorID")] public int ColorID { get; set; }
        [Column("ColorName")] public string? ColorName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        //Added on 2025.03.27 by Harivignesh
        [Column("WareHouseID")] public int WareHouseID { get; set; }
        [Column("RackLocationID")] public int RackLocationID { get; set; }
        [Column("WareHouseName")] public string? WareHouseName { get; set; }
        [Column("RackLocationName")] public string? RackLocationName { get; set; }
        //Added on 2025.04.07
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        //Added on 2025.05.27 by Harivignesh

        //Commented on 2025.05.06 it is conflicting duplicate data in BatchStock
        //[Column("FK_ProcessTypeID")] public byte ProcessTypeID { get; set; }
        //[Column("ProcessTypeName")] public string? ProcessTypeName { get; set; }

        [Column("ProbableProductionQuantity")] public decimal ProbableProductionQuantity { get; set; }
        [Column("FK_UnitID")] public int UnitID { get; set; }
        [Column("UnitName")] public string? UnitName { get; set; }

        //Added on 2025.06.05 by Harivignesh
        [Column("ProdConsumedQty")] public decimal ProdConsumedQty { get; set; }
        [Column("ProdBalanceQty")] public decimal ProdBalanceQty { get; set; }
        [Column("ProdWastageQty")] public decimal ProdWastageQty { get; set; }
        [Column("FK_StatusID")] public byte StatusID { get; set; }
        [Column("Status")] public string? Status { get; set; }
        [Column("ProbableProdConsumedQty")] public decimal ProbableProdConsumedQty { get; set; }
        // Added on 2025.06.14 by Harivignesh
        [Column("ColorCode")] public string? ColorCode { get; set; }
     //   [Column("HydrolicProductionDate")] public string? HydrolicProductionDate { get; set; }
    }
}