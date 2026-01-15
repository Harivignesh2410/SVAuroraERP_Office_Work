using SVAuroraERP.Domain.Inventory.ScrapManagement;
using SVAuroraERP.Domain.OnlineOrders;
namespace SVAuroraERP.Infrastructure.Persistence
{
    //Added on 2024.12.28
    public class SVAuroraERPDbContext(DbContextOptions<SVAuroraERPDbContext> options) : DbContext(options)
    {
        public DbSet<GlobalConfig> GlobalConfig { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserLoginData> vUserLoginData { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<VRole> VRole { get; set; }
        public DbSet<RoleConfiguration> RoleConfiguration { get; set; }
        public DbSet<VRoleConfiguration> VRoleConfiguration { get; set; }
        public DbSet<RoleModule> RoleModule { get; set; }

        public DbSet<LkupModule> LkupModule { get; set; }
        public DbSet<LkupMenuGroup> LkupMenuGroup { get; set; }
        public DbSet<LkupMenuControl> LkupMenuControl { get; set; }
        public DbSet<LkupPageControl> LkupPageControl { get; set; }
        public DbSet<VMenuLayout> VMenuLayout { get; set; }

        public DbSet<TransactionLog> TransactionLog { get; set; }
        //Added on 2025.01.02 by Dhinesh kumar
        public DbSet<Unit> Unit { get; set; }
        public DbSet<VUnit> VUnit { get; set; }
        public DbSet<Size> Size { get; set; }
        public DbSet<VSize> VSize { get; set; }

        public DbSet<Tax> Tax { get; set; }
        public DbSet<VTax> VTax { get; set; }//Added on 2025.01.02 by Harivignesh
        public DbSet<Company> Company { get; set; }
        public DbSet<VCompany> VCompany { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Designation> Designation { get; set; }
        public DbSet<VDesignation> VDesignation { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Item> Item { get; set; }
        public DbSet<VItem> VItem { get; set; }
        public DbSet<VSupplier> VSupplier { get; set; }//Added on 2025.01.04 


        //Added on 2025.01.04
        public DbSet<Color> Color { get; set; }
        public DbSet<VColor> VColor { get; set; }
        //Added on 2025.10.14
        public DbSet<MapPlateColor> MapPlateColor { get; set; }
        public DbSet<VMapPlateColor> VMapPlateColor { get; set; }

        public DbSet<MapPlateSize> MapPlateSize { get; set; }
        public DbSet<VMapPlateSize> VMapPlateSize { get; set; }
        //Added on 2025.01.03 

        public DbSet<VEmployee> VEmployee { get; set; }
        public DbSet<BloodGroup> BloodGroup { get; set; }

        //Added on 2025.01.05 by Sivakumar
        public DbSet<LkupItemCategory> LkupItemCategory { get; set; }

        //Added on 2025.01.06  

        public DbSet<OtherCharges> OtherCharges { get; set; }
        public DbSet<VOtherCharges> VOtherCharges { get; set; }

        //Added on 2025.01.07 by Harivignesh
        public DbSet<PurchaseEntryTrans> PurchaseEntryTrans { get; set; }
        public DbSet<VPurchaseEntryTrans> VPurchaseEntryTrans { get; set; }
        //Added on 2025.01.07 By Dhinesh
        public DbSet<PurchaseEntry> PurchaseEntry { get; set; }
        public DbSet<VPurchaseEntry> VPurchaseEntry { get; set; }

        // Added on 2025.01.21 by Harivignesh (US 44)
        public DbSet<PendingInspection> PendingInspection { get; set; }
        public DbSet<VPendingInwardInspection> VPendingInwardInspection { get; set; }

        public DbSet<ComponentType> ComponentType { get; set; }
        public DbSet<VComponentType> VComponentType { get; set; }

        //Added on 2025.01.30 by Harivignesh
        public DbSet<PurchaseOrder> PurchaseOrder { get; set; }
        public DbSet<VPurchaseOrder> VPurchaseOrder { get; set; }
        public DbSet<PurchaseOrderTrans> PurchaseOrderTrans { get; set; }
        public DbSet<VPurchaseOrderTrans> VPurchaseOrderTrans { get; set; }
        //Added on 2025.02.01 By SivaKumar
        public DbSet<Category> Category { get; set; }
        public DbSet<VCategory> VCategory { get; set; }
        //Added on 2025.02.01 By Harivignesh
        public DbSet<DocumentGroup> DocumentGroup { get; set; }
        public DbSet<VDocumentGroup> VDocumentGroup { get; set; }
        public DbSet<DocumentType> DocumentType { get; set; }
        public DbSet<VDocumentType> VDocumentType { get; set; }
        public DbSet<AppLauncher> AppLauncher { get; set; }
        public DbSet<VApplauncher> VApplauncher { get; set; }

        //Added on 2025.03.04 by Harivignesh
        public DbSet<WareHouse> WareHouse { get; set; }
        public DbSet<VWareHouse> VWareHouse { get; set; }
        public DbSet<RackLocation> RackLocation { get; set; }
        public DbSet<VRackLocation> VRackLocation { get; set; }
        public DbSet<RackLocationSizeCapacity> RackLocationSizeCapacity { get; set; }

        //Added on 2025.03.12 by Harivignesh
        public DbSet<ProcessType> ProcessType { get; set; }
        public DbSet<StockRequest> StockRequest { get; set; }
        public DbSet<StockRequestTrans> StockRequestTrans { get; set; }
        public DbSet<VStockRequestTrans> VStockRequestTrans { get; set; }
        public DbSet<BatchStock> VBatchStock { get; set; }
        //Added on 2025.03.12 by Mani Bharathi
        public DbSet<ProductionConfiguration> ProductionConfiguration { get; set; }
        public DbSet<VProductionConfiguration> VProductionConfiguration { get; set; }
        public DbSet<VProcessType> VProcessType { get; set; }


        //Added on 2025.03.13
        public DbSet<VStockRequest> VStockRequest { get; set; }
        //Added on 2025.03.17 by Harivignesh
        public DbSet<ProductionConsumption> ProductionConsumption { get; set; }
        public DbSet<ProductionInward> ProductionInward { get; set; }

        //Added on 2025.03.22 by Harivignesh
        public DbSet<Machine> Machine { get; set; }
        public DbSet<VMachine> VMachine { get; set; }
        public DbSet<MachineType> MachineType { get; set; }
        public DbSet<LaserNoMarking> LaserNoMarking { get; set; }
        //Added on 2025/04/17 by Harivignesh
        public DbSet<Box> Box { get; set; }
        public DbSet<VBox> VBox { get; set; }
        //Added on 2025/04/18 by Harivignesh
        public DbSet<VStockPacking> VStockPacking { get; set; }
        //Added on 2025/04/21 by Harivignesh
        public DbSet<Packing> Packing { get; set; }
        //Added on 2025/04/21 by Harivignesh
        public DbSet<PackingTrans> PackingTrans { get; set; }
        //Added on 2025/04/23 by Harivignesh
        public DbSet<VPacking> VPacking { get; set; }
        //Added on 2025/04/25 by Harivignesh
        public DbSet<Courier> Courier { get; set; }
        public DbSet<VCourier> VCourier { get; set; }
        //Added on 2025/04/29 by Harivignesh
        public DbSet<NumberPlateDispatch> NumberPlateDispatch { get; set; }
        public DbSet<NumberPlateDispatchTrans> NumberPlateDispatchTrans { get; set; }
        //Added on 2025/05/01 by Harivignesh
        public DbSet<VNumberPlateDispatch> VNumberPlateDispatch { get; set; }
        //Added on 2025/05/02 by Harivignesh
        public DbSet<VPackingTrans> VPackingTrans { get; set; }
        public DbSet<VehicleClass> VehicleClass { get; set; }
        public DbSet<VVehicleClass> VVehicleClass { get; set; }
        public DbSet<VehiclePlateColor> VehiclePlateColor { get; set; }
        public DbSet<VVehiclePlateColor> VVehiclePlateColor { get; set; }
        public DbSet<VehiclePlateSize> VehiclePlateSize { get; set; }
        public DbSet<VVehiclePlateSize> VVehiclePlateSize { get; set; }
        public DbSet<VehiclePlateSizeMapping> VehiclePlateSizeMapping { get; set; }
        public DbSet<VVehiclePlateSizeMapping> VVehiclePlateSizeMapping { get; set; }
        public DbSet<VehiclePlateType> VehiclePlateType { get; set; }
        public DbSet<VehiclePlateImage> VehiclePlateImage { get; set; }
        public DbSet<VVehiclePlateImage> VVehiclePlateImage { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<VState> VState { get; set; }
        public DbSet<HolidayType> HolidayType { get; set; }
        public DbSet<VHolidayType> VHolidayType { get; set; }
        public DbSet<DealerHoliday> DealerHoliday { get; set; }
        public DbSet<DealerHolidayType> DealerHolidayType { get; set; }
        public DbSet<VDealerHoliday> VDealerHoliday { get; set; }
        public DbSet<DealerWorkingDay> DealerWorkingDay { get; set; }
        public DbSet<VDealerWorkingDay> VDealerWorkingDay { get; set; }
        public DbSet<TimeSlot> TimeSlot { get; set; }
        public DbSet<VTimeSlot> VTimeSlot { get; set; }
        public DbSet<DealerSlotConfig> DealerSlotConfig { get; set; }
        public DbSet<VDealerSlotConfig> VDealerSlotConfig { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<VDistrict> VDistrict { get; set; }
        public DbSet<HomeFitmentPincode> HomeFitmentPincode { get; set; }
        public DbSet<VHomeFitmentPincode> VHomeFitmentPincode { get; set; }
        public DbSet<OnlinePlatePrice> OnlinePlatePrice { get; set; }
        public DbSet<VOnlinePlatePrice> VOnlinePlatePrice { get; set; }
        public DbSet<VehicleCategory> VehicleCategory { get; set; }
        public DbSet<VehicleType> VehicleType { get; set; }
        public DbSet<Fuel> Fuel { get; set; }
        public DbSet<HSRPPlateDimension> HSRPPlateDimension { get; set; }
        public DbSet<VHSRPPlateDimension> VHSRPPlateDimension { get; set; }
        public DbSet<HSRPUser> HSRPUser { get; set; }
        public DbSet<VHSRPUser> VHSRPUser { get; set; }
        public DbSet<HSRPUserType> HSRPUserType { get; set; }
        public DbSet<HSRPPartNumber> HSRPPartNumber { get; set; }
        public DbSet<VHSRPPartNumber> VHSRPPartNumber { get; set; }

        //Added on 2025/05/26 by HARIVIGNESH
        public DbSet<ProductionCalculation> ProductionCalculation { get; set; }
        public DbSet<VProductionCalculation> VProductionCalculation { get; set; }
        //Added on 2025/05/26 by HARIVIGNESH
        public DbSet<HydrolicPressure> HydrolicPressures { get; set; }
        //Added on 2025.06.02 by Harivignesh
        public DbSet<HydrolicConsumption> HydrolicConsumption { get; set; }
        public DbSet<VHydrolicPressure> VHydrolicPressure { get; set; }
        //Added on 2025.06.13 by Harivignesh
        public DbSet<HologramPunching> HologramPunching { get; set; }
        public DbSet<HologramConsumption> HologramConsumption { get; set; }
        public DbSet<VHologramPunching> VHologramPunching { get; set; }
        public DbSet<VHologramPunchingCompleted> VHologramPunchingCompleted { get; set; }
        //Added on 2025.06.20 by Harivignesh
        public DbSet<HydrolicPressureBatchStock> HydrolicPressureBatchStock { get; set; }
        //Added on 2025.06.28 by Harivignesh
        public DbSet<VComponentExceptType> VComponentExceptType { get; set; }
        //Added on 2025.07.02 by Harivignesh
        public DbSet<VHydrolicPressureCompleted> VHydrolicPressureCompleted { get; set; }
        public DbSet<LaserNoConsumption> LaserNoConsumption { get; set; }
        //Added on 2025.07.05 by Harivignesh
        public DbSet<VLaserNoMarking> VLaserNoMarking { get; set; }
        public DbSet<HSRPReplacementReason> HSRPReplacementReason { get; set; }
        public DbSet<VHSRPReplacementReason> VHSRPReplacementReason { get; set; }
        public DbSet<HSRPReplacementDocument> HSRPReplacementDocument { get; set; }
        public DbSet<VHSRPReplacementDocument> VHSRPReplacementDocument { get; set; }
        // Added On 2025.09.25
        public DbSet<LkupApplication> LkupApplication { get; set; }
        // Added On 2025.09.26
        public DbSet<OEMVendorCodeMapping> OEMVendorCodeMapping { get; set; }
        public DbSet<VOEMVendorCodeMapping> VOEMVendorCodeMapping { get; set; }

        public DbSet<OEMVendorDealerMapping> OEMVendorDealerMapping { get; set; }
        public DbSet<VOEMVendorDealerMapping> VOEMVendorDealerMapping { get; set; }
        public DbSet<ImportOEM> ImportOEM { get; set; }
        public DbSet<OEMConfig> OEMConfig { get; set; }
        public DbSet<VOEMImport> VOEMImport { get; set; }
        public DbSet<VOEMImportTrans> VOEMImportTrans { get; set; }
        public DbSet<OEMPricing> OEMPricing { get; set; }
        public DbSet<VOEMPricing> VOEMPricing { get; set; }
        public DbSet<VNumberPlateDispatchTrans> VNumberPlateDispatchTrans { get; set; }
        public DbSet<VHSRPOrder> VHSRPOrder { get; set; }
        public DbSet<OrderType> OrderType { get; set; }
        public DbSet<VHSRPOrderSummary> VHSRPOrderSummary { get; set; }
        public DbSet<VGetReadyforProcessingOrders> VGetReadyforProcessingOrders { get; set; }
        public DbSet<VHSrpLaserNoStock> VHSrpLaserNoStock { get; set; }
        public DbSet<HSrpLaserNoStatus> HSrpLaserNoStatus { get; set; }
        public DbSet<VHSRPLaserNoStockLog> VHSRPLaserNoStockLog { get; set; }
        public DbSet<VCreateJobCard> VCreateJobCard { get; set; }
        public DbSet<HSRPRoleConfig> HSRPRoleConfig { get; set; }
        public DbSet<VOEMConfig> VOEMConfig { get; set; }
        public DbSet<VHSRPRoleConfig> VHSRPRoleConfig { get; set; }
        public DbSet<HSRPJobCard> HSRPJobCard { get; set; }
        public DbSet<HSRPJobCardTrans> HSRPJobCardTrans { get; set; }
        public DbSet<VHSRPJobCardTrans> VHSRPJobCardTrans { get; set; }
        public DbSet<VHSRPJobCard> VHSRPJobCard { get; set; }
        public DbSet<VGetReadyforQualityProcessing> VGetReadyforQualityProcessing { get; set; }
        public DbSet<VQualityProcessing> VQualityProcessing { get; set; }
        public DbSet<VJobCardGenerated> VJobCardGenerated { get; set; }
        public DbSet<CreateInvoiceData> CreateInvoiceData { get; set; }
        public DbSet<VQCCompleted> VQCCompleted { get; set; }
        public DbSet<VHSRPInvoice> VHSRPInvoice { get; set; }
        public DbSet<VHSRPInvoiceTrans> VHSRPInvoiceTrans { get; set; }
        public DbSet<LaserNoPlate> LaserNoPlate { get; set; }
        public DbSet<VHSRPInvoiceByDealer> VHSRPInvoiceByDealer { get; set; }
        public DbSet<GenerateDeliveryData> GenerateDeliveryData { get; set; }
        public DbSet<GenerateDeliveryTrans> GenerateDeliveryTrans { get; set; }

        public DbSet<VGenerateDeliveryData> VGenerateDeliveryData { get; set; }
        public DbSet<VGenerateDeliveryTrans> VGenerateDeliveryTrans { get; set; }
        public DbSet<VAcknowledgeDispatchedOrders> VAcknowledgeDispatchedOrders { get; set; }
        public DbSet<VHSRPInvoiceForGenerateDelivery> VHSRPInvoiceForGenerateDelivery { get; set; }
        public DbSet<VDealerByOEM> VDealerByOEM { get; set; }

        //Added on 2025/10/31 by Harivignesh
        public DbSet<ScrapEntry> ScrapEntry { get; set; }
        public DbSet<ScrapEntryTrans> ScrapEntryTrans { get; set; }
        public DbSet<VScrapEntryTrans> VScrapEntryTrans { get; set; }
        public DbSet<VScrapEntry> VScrapEntry { get; set; }
        public DbSet<LkupHSRPOrderRectificationReason> LkupHSRPOrderRectificationReason { get; set; }
        public DbSet<RectifyLaserPlate> RectifyLaserPlate { get; set; }
        public DbSet<VListDispatchOrder> VListDispatchOrder { get; set; }
        public DbSet<VListDispatchOrderTrans> VListDispatchOrderTrans { get; set; }
        public DbSet<VGenerateDeliveryDataForShipment> VGenerateDeliveryDataForShipment { get; set; }
        public DbSet<VRejectedQualityProcessing> VRejectedQualityProcessing { get; set; }
        public DbSet<VDispatchedOrders> VDispatchedOrders { get; set; }
        public DbSet<HSRPVehiclePlateImage> HSRPVehiclePlateImage { get; set; }
        public DbSet<APISubmittedOrders> APISubmittedOrders { get; set; }

        public DbSet<VFittedOrders> VFittedOrders { get; set; }
        public DbSet<VFixationReUploaded> VFixationReUploaded { get; set; }

        public DbSet<VDeliveryAcknowledgement> VDeliveryAcknowledgement { get; set; }
        public DbSet<VFixationImageReupload> VFixationImageReupload { get; set; }
        public DbSet<VHSRPVehiclePlateImage> VHSRPVehiclePlateImage { get; set; }
        public DbSet<VCancelledorDamagedPlateOrders> VCancelledorDamagedPlateOrders { get; set; }
        public DbSet<VTotalCancelledOrders> VTotalCancelledOrders { get; set; }
        public DbSet<VExportInvoiceList> VExportInvoiceList { get; set; }
        public DbSet<VOnlineHSRPOrder> VOnlineHSRPOrder { get; set; }
        public DbSet<VOnlineReplacementOrderDetails> VOnlineReplacementOrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GlobalConfig>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<OEMConfig>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<HSRPRoleConfig>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<VOEMConfig>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<VHSRPRoleConfig>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<CreateInvoiceData>(entity => { entity.HasNoKey(); });

            modelBuilder.Entity<VHSRPInvoice>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<VHSRPInvoiceByDealer>(entity => { entity.HasNoKey(); });


            modelBuilder.Entity<User>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Role>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<GlobalConfig>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<RoleConfiguration>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<VRoleConfiguration>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<RoleModule>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Tax>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Unit>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Size>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Item>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Designation>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Color>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Supplier>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Employee>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<OtherCharges>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<PurchaseEntry>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<PendingInspection>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<PurchaseEntryTrans>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<ComponentType>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<PurchaseOrder>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<PurchaseOrderTrans>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<DocumentGroup>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<DocumentType>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<WareHouse>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<RackLocation>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<RackLocationSizeCapacity>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<StockRequest>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<ProductionConfiguration>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<ProductionInward>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<ProductionConsumption>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<Machine>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<HologramPunching>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<LaserNoMarking>().HasQueryFilter(w => !w.IsDeleted);
            //Added on 2025/04/17 by Harivignesh
            modelBuilder.Entity<Box>().HasQueryFilter(w => !w.IsDeleted);
            //Added on 2025.04.07
            modelBuilder.Entity<BatchStock>().HasQueryFilter(w => !w.IsDeleted);
            //Added on 2025/04/21 by Harivignesh
            modelBuilder.Entity<Packing>().HasQueryFilter(w => !w.IsDeleted);
            //Added on 2025/04/25 by Harivignesh
            modelBuilder.Entity<Courier>().HasQueryFilter(w => !w.IsDeleted);
            //Added on 2025/04/29 by Harivignesh
            modelBuilder.Entity<NumberPlateDispatch>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<NumberPlateDispatchTrans>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<VehicleClass>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<VehiclePlateColor>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<VehiclePlateSize>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<VehiclePlateImage>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<VehiclePlateSizeMapping>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<State>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<HolidayType>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<DealerHoliday>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<DealerHolidayType>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<DealerWorkingDay>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<TimeSlot>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<DealerSlotConfig>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<District>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<HomeFitmentPincode>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<OnlinePlatePrice>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<HSRPPlateDimension>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<HSRPUser>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<ProductionCalculation>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<HydrolicPressure>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<HydrolicConsumption>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<HologramPunching>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<HologramConsumption>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<LaserNoConsumption>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<OEMVendorDealerMapping>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<ImportOEM>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<ScrapEntry>().HasQueryFilter(w => !w.IsDeleted);
            modelBuilder.Entity<ScrapEntryTrans>().HasQueryFilter(w => !w.IsDeleted);

            //Configure Decimal Precision (Added on 2024.01.04)
            modelBuilder.Entity<Item>().Property(i => i.Price).HasPrecision(18, 4);
            modelBuilder.Entity<VItem>().Property(i => i.Price).HasPrecision(18, 4);
            modelBuilder.Entity<Tax>().Property(i => i.TaxPercentage).HasPrecision(18, 4);
            modelBuilder.Entity<VTax>().Property(i => i.TaxPercentage).HasPrecision(18, 4);

            modelBuilder.Entity<VPurchaseEntryTrans>(entity =>
            {
                entity.Property(i => i.Quantity).HasPrecision(18, 4);
                entity.Property(i => i.Pcs).HasPrecision(18, 4);
                entity.Property(i => i.Rate).HasPrecision(18, 4);
                entity.Property(i => i.MaterialValue).HasPrecision(18, 4);
                entity.Property(i => i.OtherChargesIDAmount1).HasPrecision(18, 4);
                entity.Property(i => i.OtherChargesIDAmount2).HasPrecision(18, 4);
                entity.Property(i => i.OtherChargesIDAmount3).HasPrecision(18, 4);
                entity.Property(i => i.TaxableChargesAmount).HasPrecision(18, 4);
                entity.Property(i => i.TaxPercentage1).HasPrecision(18, 4);
                entity.Property(i => i.TaxAmount1).HasPrecision(18, 4);
                entity.Property(i => i.TaxPercentage2).HasPrecision(18, 4);
                entity.Property(i => i.TaxAmount2).HasPrecision(18, 4);
                entity.Property(i => i.TaxAmount).HasPrecision(18, 4);
                entity.Property(i => i.SubTotal).HasPrecision(18, 4);
            });
            modelBuilder.Entity<PurchaseEntryTrans>(entity =>
            {
                entity.Property(i => i.Quantity).HasPrecision(18, 4);
                entity.Property(i => i.Pcs).HasPrecision(18, 4);
                entity.Property(i => i.Rate).HasPrecision(18, 4);
                entity.Property(i => i.MaterialValue).HasPrecision(18, 4);
                entity.Property(i => i.OtherChargesIDAmount1).HasPrecision(18, 4);
                entity.Property(i => i.OtherChargesIDAmount2).HasPrecision(18, 4);
                entity.Property(i => i.OtherChargesIDAmount3).HasPrecision(18, 4);
                entity.Property(i => i.TaxableChargesAmount).HasPrecision(18, 4);
                entity.Property(i => i.TaxPercentage1).HasPrecision(18, 4);
                entity.Property(i => i.TaxAmount1).HasPrecision(18, 4);
                entity.Property(i => i.TaxPercentage2).HasPrecision(18, 4);
                entity.Property(i => i.TaxAmount2).HasPrecision(18, 4);
                entity.Property(i => i.TaxAmount).HasPrecision(18, 4);
                entity.Property(i => i.SubTotal).HasPrecision(18, 4);
            });


            //Added on 2025.01.07 By Dhinesh
            modelBuilder.Entity<PurchaseEntry>().Property(i => i.GrossAmount).HasPrecision(18, 4);
            modelBuilder.Entity<PurchaseEntry>().Property(i => i.RoundedOffPlus).HasPrecision(18, 4);
            modelBuilder.Entity<PurchaseEntry>().Property(i => i.RoundedOffMinus).HasPrecision(18, 4);
            modelBuilder.Entity<PurchaseEntry>().Property(i => i.OtherChargesAmount).HasPrecision(18, 4);
            modelBuilder.Entity<PurchaseEntry>().Property(i => i.TaxPercentage1).HasPrecision(18, 4);
            modelBuilder.Entity<PurchaseEntry>().Property(i => i.TaxAmount1).HasPrecision(18, 4);
            modelBuilder.Entity<PurchaseEntry>().Property(i => i.TaxPercentage2).HasPrecision(18, 4);
            modelBuilder.Entity<PurchaseEntry>().Property(i => i.TaxAmount2).HasPrecision(18, 4);
            modelBuilder.Entity<PurchaseEntry>().Property(i => i.TaxAmount).HasPrecision(18, 4);
            modelBuilder.Entity<PurchaseEntry>().Property(i => i.PurchaseInvoiceAmount).HasPrecision(18, 4);

            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.GrossAmount).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.RoundedOffPlus).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.RoundedOffMinus).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.OtherChargesAmount).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.TaxPercentage1).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.TaxAmount1).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.TaxPercentage2).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.TaxAmount2).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.TaxAmount).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.PurchaseInvoiceAmount).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.TotalQuantity).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.TotalPcs).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.TotalOtherCharges).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.PurchaseInvoiceAmount).HasPrecision(18, 4);

            modelBuilder.Entity<PendingInspection>().Property(i => i.BatchQuantity).HasPrecision(18, 4);
            modelBuilder.Entity<PendingInspection>().Property(i => i.PendingQuantity).HasPrecision(18, 4);
            modelBuilder.Entity<VPendingInwardInspection>().Property(i => i.BatchQuantity).HasPrecision(18, 4);

            //Added on 2025.01.25
            modelBuilder.Entity<VPendingInwardInspection>().Property(i => i.Quantity).HasPrecision(18, 4);
            modelBuilder.Entity<PurchaseEntryTrans>().Property(i => i.OtherChargesAmount).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntry>().Property(i => i.TotalItemTax).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseEntryTrans>().Property(i => i.OtherChargesAmount).HasPrecision(18, 4);
            //Added on 2025.01.30 by Harivignesh
            modelBuilder.Entity<PurchaseOrder>().Property(i => i.PurchaseOrderValue).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseOrder>().Property(i => i.PurchaseOrderValue).HasPrecision(18, 4);
            modelBuilder.Entity<PurchaseOrderTrans>().Property(i => i.Quantity).HasPrecision(18, 4);
            modelBuilder.Entity<VPurchaseOrderTrans>().Property(i => i.Quantity).HasPrecision(18, 4);

            modelBuilder.Entity<VPurchaseOrder>()
                       .HasMany(p => p.PurchaseOrderTransList)
                       .WithOne()
                       .HasForeignKey("PurchaseOrderID");

            //Added on 2025.02.20
            modelBuilder.Entity<VPendingInwardInspection>().Property(i => i.PurchaseInvoiceAmount).HasPrecision(18, 4);
            modelBuilder.Entity<RackLocationSizeCapacity>().Property(i => i.Capacity).HasPrecision(18, 4);

            //Added on 2025.03.12
            modelBuilder.Entity<PendingInspection>(entity =>
            {
                entity.Property(i => i.ExcessQuantity).HasPrecision(18, 4);
                entity.Property(i => i.LessQuantity).HasPrecision(18, 4);
            });

            modelBuilder.Entity<VPendingInwardInspection>(entity =>
            {
                entity.Property(i => i.PurchaseInvoiceAmount).HasPrecision(18, 4);
                entity.Property(i => i.ExcessQuantity).HasPrecision(18, 4);
                entity.Property(i => i.LessQuantity).HasPrecision(18, 4);
                entity.Property(i => i.PendingQuantity).HasPrecision(18, 4);
            });

            //Added on 2025.03.12 by Harivignesh
            modelBuilder.Entity<BatchStock>(entity =>
            {
                entity.Property(i => i.BatchQuantity).HasPrecision(12, 4);
                entity.Property(i => i.ConsumedQty).HasPrecision(12, 4);
                entity.Property(i => i.BalanceQty).HasPrecision(12, 4);
                entity.Property(i => i.ProdConsumedQty).HasPrecision(12, 4);
                entity.Property(i => i.ProdBalanceQty).HasPrecision(12, 4);
                entity.Property(i => i.ProdWastageQty).HasPrecision(12, 4);
                entity.Property(i => i.ProbableProdConsumedQty).HasPrecision(12, 4);
                entity.Property(i => i.ProbableProductionQuantity).HasPrecision(12, 4);
            });
            modelBuilder.Entity<StockRequestTrans>().Property(i => i.Quantity).HasPrecision(12, 4);
            modelBuilder.Entity<VStockRequestTrans>(entity =>
            {
                entity.Property(i => i.Quantity).HasPrecision(18, 4);
                entity.Property(i => i.BalanceQty).HasPrecision(18, 4);
                entity.Property(i => i.ConsumedQty).HasPrecision(18, 4);
                entity.Property(i => i.BatchQuantity).HasPrecision(18, 4);
                entity.Property(i => i.ProductionQuantity).HasPrecision(18, 4);
                entity.Property(i => i.ProbableProdConsumedQty).HasPrecision(18, 4);
                entity.Property(i => i.ProdWastageQty).HasPrecision(18, 4);
                entity.Property(i => i.PerPlate).HasPrecision(18, 4);
                entity.Property(i => i.ProbableProductionQuantity).HasPrecision(18, 4);
            });
            //Added on 2025.03.17 by Harivignesh
            modelBuilder.Entity<ProductionInward>().Property(i => i.ExpectedProductionQty).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionInward>().Property(i => i.ActualProductionQty).HasPrecision(18, 4);

            modelBuilder.Entity<ProductionConsumption>().Property(i => i.ActualConsumedQty).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionConsumption>().Property(i => i.WastageQty).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionConsumption>().Property(i => i.WastagePercentage).HasPrecision(18, 4);
            modelBuilder.Entity<ProductionConsumption>().Property(i => i.BalanceQty).HasPrecision(18, 4);

            modelBuilder.Entity<HologramPunching>().Property(i => i.HologramFinishedQty).HasPrecision(18, 4);
            modelBuilder.Entity<HologramPunching>().Property(i => i.RejectedPlateQty).HasPrecision(18, 4);
            modelBuilder.Entity<HologramPunching>().Property(i => i.HologramWastageQty).HasPrecision(18, 4);
            //Added on 2025.04.22 by Harivignesh
            modelBuilder.Entity<PackingTrans>().Property(i => i.Quantity).HasPrecision(18, 4);
            //Added on 2025.04.26 by Harivignesh
            modelBuilder.Entity<VPacking>().Property(i => i.PcsPerBox).HasPrecision(18, 4);
            modelBuilder.Entity<VPacking>().Property(i => i.TotalQuantity).HasPrecision(18, 4);
            //Added on 2025.05.02 by Harivignesh
            modelBuilder.Entity<VPackingTrans>().Property(i => i.Quantity).HasPrecision(18, 4);
            modelBuilder.Entity<OnlinePlatePrice>(entity =>
            {
                entity.Property(i => i.Front).HasPrecision(18, 4);
                entity.Property(i => i.Rear).HasPrecision(18, 4);
                entity.Property(i => i.SnapLock).HasPrecision(18, 4);
                entity.Property(i => i.TLPSticker).HasPrecision(18, 4);
                entity.Property(i => i.EmbossingFitmentCharges).HasPrecision(18, 4);
                entity.Property(i => i.DealerFitmentCharges).HasPrecision(18, 4);
                entity.Property(i => i.HomeFitmentCharges).HasPrecision(18, 4);
                entity.Property(i => i.DealerCourierCharge).HasPrecision(18, 4);
                entity.Property(i => i.DealerLocationChangeCharge).HasPrecision(18, 4);
                entity.Property(i => i.OtherCharges).HasPrecision(18, 4);
            });
            modelBuilder.Entity<VOnlinePlatePrice>(entity =>
            {
                entity.Property(i => i.Front).HasPrecision(18, 4);
                entity.Property(i => i.Rear).HasPrecision(18, 4);
                entity.Property(i => i.SnapLock).HasPrecision(18, 4);
                entity.Property(i => i.TLPSticker).HasPrecision(18, 4);
                entity.Property(i => i.EmbossingFitmentCharges).HasPrecision(18, 4);
                entity.Property(i => i.DealerFitmentCharges).HasPrecision(18, 4);
                entity.Property(i => i.HomeFitmentCharges).HasPrecision(18, 4);
                entity.Property(i => i.DealerCourierCharge).HasPrecision(18, 4);
                entity.Property(i => i.DealerLocationChangeCharge).HasPrecision(18, 4);
                entity.Property(i => i.OtherCharges).HasPrecision(18, 4);
            });
            modelBuilder.Entity<ProductionCalculation>(entity =>
            {
                entity.Property(i => i.ProductionQuantity).HasPrecision(18, 2);
                entity.Property(i => i.QuantityForOneUnit).HasPrecision(18, 2);
                entity.Property(i => i.PerPlate).HasPrecision(18, 4);
            });
            modelBuilder.Entity<VProductionCalculation>(entity =>
            {
                entity.Property(i => i.ProductionQuantity).HasPrecision(18, 2);
                entity.Property(i => i.QuantityForOneUnit).HasPrecision(18, 2);
                entity.Property(i => i.PerPlate).HasPrecision(18, 4);
            });

            modelBuilder.Entity<HydrolicPressure>().Property(i => i.OtherWastageQty).HasPrecision(12, 4);
            modelBuilder.Entity<VHydrolicPressure>().Property(i => i.OtherWastageQty).HasPrecision(12, 4);
            modelBuilder.Entity<VHydrolicPressure>().Property(i => i.ProdConsumedQty).HasPrecision(12, 4);
            modelBuilder.Entity<VHydrolicPressure>().Property(i => i.ProdBalanceQty).HasPrecision(12, 4);
            modelBuilder.Entity<VHydrolicPressure>().Property(i => i.ProdWastageQty).HasPrecision(12, 4);
            modelBuilder.Entity<VHydrolicPressure>().Property(i => i.ProbableProductionQuantity).HasPrecision(12, 4);

            modelBuilder.Entity<VHydrolicPressureCompleted>().Property(i => i.OtherWastageQty).HasPrecision(12, 4);
            modelBuilder.Entity<VHydrolicPressureCompleted>().Property(i => i.ProdConsumedQty).HasPrecision(12, 4);
            modelBuilder.Entity<VHydrolicPressureCompleted>().Property(i => i.ProdBalanceQty).HasPrecision(12, 4);
            modelBuilder.Entity<VHydrolicPressureCompleted>().Property(i => i.ProdWastageQty).HasPrecision(12, 4);
            modelBuilder.Entity<VHydrolicPressureCompleted>().Property(i => i.ProbableProductionQuantity).HasPrecision(12, 4);

            modelBuilder.Entity<HydrolicConsumption>().Property(i => i.ActualConsumedQty).HasPrecision(12, 4);
            modelBuilder.Entity<HydrolicConsumption>().Property(i => i.WastageQty).HasPrecision(12, 4);
            modelBuilder.Entity<HydrolicConsumption>().Property(i => i.WastagePercentage).HasPrecision(12, 4);
            modelBuilder.Entity<HydrolicConsumption>().Property(i => i.BalanceQty).HasPrecision(12, 4);
            modelBuilder.Entity<HologramPunching>(entity =>
            {
                entity.Property(i => i.HologramFinishedQty).HasPrecision(12, 4);
                entity.Property(i => i.HologramWastageQty).HasPrecision(12, 4);
                entity.Property(i => i.RejectedPlateQty).HasPrecision(12, 4);
            });
            modelBuilder.Entity<VHologramPunching>(entity =>
            {
                entity.Property(e => e.HologramFinishedQty).HasPrecision(12, 4);
                entity.Property(e => e.RejectedPlateQty).HasPrecision(12, 4);
                entity.Property(e => e.HologramWastageQty).HasPrecision(12, 4);
            });
            modelBuilder.Entity<VHologramPunchingCompleted>(entity =>
            {
                entity.Property(e => e.HologramFinishedQty).HasPrecision(12, 4);
                entity.Property(e => e.RejectedPlateQty).HasPrecision(12, 4);
                entity.Property(e => e.HologramWastageQty).HasPrecision(12, 4);
                entity.Property(e => e.ProbableProductionQuantity).HasPrecision(12, 4);
                entity.Property(e => e.ProdBalanceQty).HasPrecision(12, 4);
                entity.Property(e => e.ProdConsumedQty).HasPrecision(12, 4);
                entity.Property(e => e.ProdWastageQty).HasPrecision(12, 4);
            });

            modelBuilder.Entity<HologramConsumption>(entity =>
            {
                entity.Property(i => i.ActualConsumedQty).HasPrecision(12, 4);
                entity.Property(i => i.WastageQty).HasPrecision(12, 4);
                entity.Property(i => i.WastagePercentage).HasPrecision(12, 4);
                entity.Property(i => i.BalanceQty).HasPrecision(12, 4);
            });

            //Added on 2025.06.20 by Harivignesh
            modelBuilder.Entity<HydrolicPressureBatchStock>(entity =>
            {
                entity.Property(e => e.ProductionQty).HasPrecision(18, 4);
                entity.Property(e => e.WastageQty).HasPrecision(18, 4);
                entity.Property(e => e.OtherWastageQty).HasPrecision(18, 4);
                entity.Property(e => e.ProdConsumedQty).HasPrecision(18, 4);
                entity.Property(e => e.ProdBalanceQty).HasPrecision(18, 4);
                entity.Property(e => e.ProdWastageQty).HasPrecision(18, 4);

                entity.Property(e => e.BatchQuantity).HasPrecision(18, 4);
                entity.Property(e => e.ConsumedQty).HasPrecision(18, 4);
                entity.Property(e => e.BalanceQty).HasPrecision(18, 4);
                entity.Property(e => e.ProductionQuantity).HasPrecision(18, 4);
                entity.Property(e => e.PerPlate).HasPrecision(18, 4);
                entity.Property(e => e.BSProdConsumedQty).HasPrecision(18, 4);
                entity.Property(e => e.BSProdWastageQty).HasPrecision(18, 4);
                entity.Property(e => e.BSProdBalanceQty).HasPrecision(18, 4);
                entity.Property(e => e.ProbableProdConsumedQty).HasPrecision(18, 4);
            });

            modelBuilder.Entity<OEMPricing>(entity =>
            {
                entity.Property(p => p.CourierCharges).HasPrecision(18, 4);
                entity.Property(p => p.Rate).HasPrecision(18, 4);
                entity.Property(p => p.TotalAmount).HasPrecision(18, 4);
            });

            modelBuilder.Entity<VOEMPricing>(entity =>
            {
                entity.Property(p => p.CourierCharges).HasPrecision(18, 4);
                entity.Property(p => p.Rate).HasPrecision(18, 4);
                entity.Property(p => p.TotalAmount).HasPrecision(18, 4);

            });

            modelBuilder.Entity<VHSRPInvoice>(entity =>
            {
                entity.Property(p => p.NetAmount).HasPrecision(18, 4);

            });

            modelBuilder.Entity<VHSRPInvoiceTrans>(entity =>
            {
                entity.Property(p => p.Amount).HasPrecision(18, 4);
                entity.Property(p => p.GST).HasPrecision(18, 4);
                entity.Property(p => p.Qty).HasPrecision(18, 4);
                entity.Property(p => p.Rate).HasPrecision(18, 4);
            });

            modelBuilder.Entity<VCreateJobCard>(entity =>
            {
                //entity.Property(p => p.NetAmount).HasPrecision(18, 4);
                //entity.Property(p => p.TaxAmount).HasPrecision(18, 4);
                //entity.Property(p => p.Rate).HasPrecision(18, 4);
            });

            modelBuilder.Entity<VHSRPJobCardTrans>(entity =>
            {
                entity.Property(p => p.NetAmount).HasPrecision(18, 4);
                entity.Property(p => p.TaxAmount).HasPrecision(18, 4);
                entity.Property(p => p.Rate).HasPrecision(18, 4);
                entity.Property(p => p.CourierCharges).HasPrecision(18, 4);
                entity.Property(p => p.GrossTotal).HasPrecision(18, 4);
            });

            modelBuilder.Entity<VQualityProcessing>(entity =>
            {
                //entity.Property(p => p.NetAmount).HasPrecision(18, 4);
                //entity.Property(p => p.TaxAmount).HasPrecision(18, 4);
                //entity.Property(p => p.Rate).HasPrecision(18, 4);
                //entity.Property(p => p.CourierCharges).HasPrecision(18, 4);
                //entity.Property(p => p.GrossTotal).HasPrecision(18, 4);
            });

            modelBuilder.Entity<VHSRPInvoiceForGenerateDelivery>(entity =>
            {
    
                entity.Property(p => p.Rate).HasPrecision(18, 4);
                entity.Property(p => p.GST).HasPrecision(18, 4);
                entity.Property(p => p.Qty).HasPrecision(18, 4);
            });

            modelBuilder.Entity<VHSRPInvoiceTrans>(entity =>
            {
                entity.Property(p => p.Amount).HasPrecision(18, 4);
                entity.Property(p => p.GST).HasPrecision(18, 4);
                entity.Property(p => p.Qty).HasPrecision(18, 4);
                entity.Property(p => p.Rate).HasPrecision(18, 4);
            });

            // And so on for VCreateJobCard, VOEMPricing, etc.


            modelBuilder.Entity<LaserNoConsumption>(entity =>
            {
                entity.Property(i => i.WastagePercentage).HasPrecision(12, 4);
            });
            modelBuilder.Entity<VHSRPOrderSummary>(entity => { entity.HasNoKey(); });

            modelBuilder.Entity<VNumberPlateDispatchTrans>(entity =>
            {
                                entity.Property(i => i.TotalQuantity).HasPrecision(12, 4);
                entity.Property(i => i.PcsPerBox).HasPrecision(12, 4);
            });

            modelBuilder.Entity<ScrapEntry>().Property(i => i.TotalSoldQty).HasPrecision(9, 2);
            modelBuilder.Entity<VScrapEntry>().Property(i => i.TotalSoldQty).HasPrecision(9, 2);
            modelBuilder.Entity<ScrapEntryTrans>().Property(i => i.SoldQty).HasPrecision(9, 2);
            modelBuilder.Entity<VScrapEntryTrans>().Property(i => i.SoldQty).HasPrecision(9, 2);

            modelBuilder.Entity<VExportInvoiceList>(entity =>
            {
                entity.Property(p => p.Amount).HasPrecision(18, 4);
                entity.Property(p => p.GST).HasPrecision(18,4);
                entity.Property(p => p.Qty).HasPrecision(18, 4);
                entity.Property(p => p.Rate).HasPrecision(18, 4);
            });

            modelBuilder.Entity<VQCCompleted>(entity =>
            {
                entity.Property(p => p.CourierCharges).HasPrecision(18, 4);
                entity.Property(p => p.GrossTotal).HasPrecision(18, 4);
                entity.Property(p => p.NetAmount).HasPrecision(18, 4);
                entity.Property(p => p.Rate).HasPrecision(18, 4);
                entity.Property(p => p.TaxAmount).HasPrecision(18, 4);
            });

            modelBuilder.Entity<VHSRPInvoiceForGenerateDelivery>().Property(i => i.Amount).HasPrecision(18, 4);
        }

    }
}