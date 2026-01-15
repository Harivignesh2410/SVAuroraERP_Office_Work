using SVAuroraERP.Application.Interfaces.Persistance.OnlineOrders;
using SVAuroraERP.Infrastructure.Repositories.OnlineOrders;

namespace SVAuroraERP.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistence(configuration);

            return services;
        }

        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGlobalConfigServiceRepository, GlobalConfigServiceRepository>();
            services.AddScoped<IAuthenticaionServiceRepository, AuthenticaionServiceRepository>();
            services.AddScoped<IMenuServiceRepository, MenuServiceRepository>();

            services.AddScoped<ITransLogRespository, TransLogRespository>();
            services.AddScoped<IRoleServiceRepository, RoleServiceRepository>();
            services.AddScoped<IRoleConfigurationServiceRepository, RoleConfigurationServiceRepository>();

            services.AddScoped<IUnitServiceRespository, UnitServiceRepository>();
            services.AddScoped<ISizeServiceRepository, SizeServiceRepository>();
            services.AddScoped<ITaxServiceRepository, TaxserviceRepository>();
            services.AddScoped<ICompanyServiceRepository, CompanyServiceRepository>();
            services.AddScoped<IItemServiceRepository, ItemServiceRepository>();
            //Added on 2025.01.04
            services.AddScoped<IColorServiceRespository, ColorServiceRepository>();     
            services.AddScoped<IDesignationServiceRepository, DesignationServiceRepository>();
            services.AddScoped<IEmployeeServiceRepository, EmployeeServiceRepository>();
            services.AddScoped<ISupplierServiceRepository, SupplierServiceRepository>();
            services.AddScoped<IOtherChargesServiceRepository, OtherChargesServiceRepository>();
            //Added on 2025.01.07 by Harivignesh
            services.AddScoped<IPurchaseEntryTransServiceRepository, PurchaseEntryTransServiceRepository>();

            //Added on 2025.01.07 By Dhinesh
            services.AddScoped<IPurchaseEntryServiceRepository, PurchaseEntryServiceRepository>();

            // Added on 2025.01.21 by Harivignesh (US 44)
            services.AddScoped<IPendingInspectionServiceRepository, PendingInspectionServiceRepository>();
            // Added on 2025.01.29 by Harivignesh
            services.AddScoped<IComponentServiceRepository, ComponentServiceRepository>();
            //Added on 2025.01.30 by Harivignesh
            services.AddScoped<IPurchaseOrderServiceRepository, PurchaseOrderServiceRepository>();
            services.AddScoped<IPurchaseOrderTransServiceRepository, PurchaseOrderTransServiceRepository>();

            //Added on 2025.01.02
            services.AddScoped<ICategoryServiceRespository, CategoryServiceRepository>();
            services.AddScoped<IDocumentGroupServiceRepository, DocumentGroupServiceRepository>();
            services.AddScoped<IDocumentTypeServiceRepository, DocumentTypeServiceRepository>();
            services.AddScoped<IUserServiceRepository, UserServiceRepository>();

            services.AddScoped<IAppLauncherServiceRepository, AppLauncherServiceRepository>();

            //Added on 2025.03.04
            services.AddScoped<IWareHouseServiceRepository, WareHouseServiceRepository>();
            services.AddScoped<IRackLocationServiceRepository, RackLocationServiceRepository>();

            //Added on 2025.03.12
            services.AddScoped<IProcessTypeServiceRepository, ProcessTypeServiceRepository>();
            services.AddScoped<IStockRequestServiceRepository, StockRequestServiceRepository>();
            services.AddScoped<IProductionConfigurationServiceRepository, ProductionConfigurationServiceRepository>();
            services.AddScoped<IStockRequestTransServiceRepository, StockRequestTransServiceRepository>();
            services.AddScoped<IPendingApprovalFilterServiceRepository, PendingApprovalServiceRepository>();
            services.AddScoped<IProductionInwardServiceRepository, ProductionInwardServiceRepository>();
            services.AddScoped<IMachineServiceRepository, MachineServiceRepository>();
            services.AddScoped<IHologramPunchingServiceRepository, HologramPunchingServiceRepository>();
            services.AddScoped<ILaserNoMarkingServiceRepository, LaserNoMarkingServiceRepository>();
            //Added on 2025/04/17 by Harivignesh
            services.AddScoped<IBoxServiceRepository, BoxServiceRepository>();
            services.AddScoped<IPackingServiceRepository, PackingServiceRepository>();
            services.AddScoped<IPackingTransServiceRepository, PackingTransServiceRepository>();
            //Added on 2025/04/25 by Harivignesh
            services.AddScoped<ICourierServiceRepository, CourierServiceRepository>();
            //Added on 2025/04/25 by Harivignesh
            services.AddScoped<INumberPlateDispatchServiceRepository, NumberPlateDispatchServiceRepository>();
            services.AddScoped<INumberPlateDispatchServiceRepositoryTrans, NumberPlateDispatchServiceRepositoryTrans>();
            services.AddScoped<IVehicleClassServiceRepository, VehicleClassServiceRepository>();
            services.AddScoped<IVehiclePlateColorServiceRepository, VehiclePlateColorServiceRepository>();
            services.AddScoped<IVehiclePlateColorServiceRepository, VehiclePlateColorServiceRepository>();
            services.AddScoped<IVehiclePlateSizeServiceRepository, VehiclePlateSizeServiceRepository>();
            services.AddScoped<IDealerHolidayServiceRepository, DealerHolidayServiceRepository>();
            services.AddScoped<IDealerWorkingDayServiceRepository, DealerWorkingDayServiceRepository>();
            services.AddScoped<ITimeSlotServiceRepository, TimeSlotServiceRepository>();
            services.AddScoped<IDealerSlotConfigServiceRepository, DealerSlotConfigServiceRepository>();
            services.AddScoped<IHolidayTypeServiceRepository, HolidayTypeServiceRepository>();
            services.AddScoped<IVehiclePlateSizeMappingServiceRepository, VehiclePlateSizeMappingServiceRepository>();
            services.AddScoped<IVehiclePlateImageServiceRepository, VehiclePlateImageServiceRepository>();
            services.AddScoped<IStateServiceRepository,StateServiceRepository>();
            services.AddScoped<IDistrictServiceRepository, DistrictServiceRepository>();
            services.AddScoped<IHomeFitmentPincodeServiceRepository, HomeFitmentPincodeServiceRepository>();
            services.AddScoped<IOnlinePlatePriceServiceRepository, OnlinePlatePriceServiceRepository>();
            services.AddScoped<IHSRPPlateDimensionServiceRepository, HSRPPlateDimensionServiceRepository>();
            services.AddScoped<IHSRPUserServiceRepository, HSRPUserServiceRepository>();
            services.AddScoped<IHSRPPartNumberServiceRepository, HSRPPartNumberServiceRepository>();
            services.AddScoped<IPermissionServiceRepository, PermissionServiceRepository>();
            services.AddScoped<IProductionCalculationServiceRepository, ProductionCalculationServiceRepository>();
            services.AddScoped<IHydrolicPressureServiceRepository, HydrolicPressureServiceRepository>();

            //Added on 2024.07.07
            services.AddScoped<IAuditLogger, AuditLogger>();
            services.AddScoped<IErrorLoggerService, ErrorLoggerService>();
            services.AddScoped<IHSRPReplacementReasonServiceRepository, HSRPReplacementReasonServiceRepository>();
            services.AddScoped<IHSRPReplacementDocumentServiceRepository, HSRPReplacementDocumentServiceRepository>();
            services.AddScoped<IOEMVendorCodeMappingServiceRepository, OEMVendorCodeMappingServiceRepository>();
            services.AddScoped<IOEMVendorDealerMappingServiceRepository, OEMVendorDealerMappingServiceRepository>();
            
            //Added on 2025.05.27 by Harivignesh
            services.AddScoped<IImportOEMServiceRepository, ImportOEMServiceRepository>();
            services.AddScoped<IOEMPricingServiceRepository, OEMPricingServiceRepository>();
            services.AddScoped<IHSRPOrdersServiceRepository, HSRPOrdersServiceRepository>();
            services.AddScoped<ILaserNoAllocationServiceRepository, LaserNoAllocationServiceRepository>();
            services.AddScoped<IHSRPLaserNoStockServiceRepository, HSRPLaserNoStockServiceRepository>();

            //Added on 2025.10.14
            services.AddScoped<IMapPlateColorServiceRepository, MapPlateColorServiceRepository>();
            services.AddScoped<IMapPlateSizeServiceRepository, MapPlateSizeServiceRepository>();
            services.AddScoped<ICreateJobCardServiceRepository, CreateJobCardServiceRepository>();
            services.AddScoped<IHSRPConfigServiceRepository, HSRPConfigServiceRepository>();

            services.AddScoped<IQualityProcessingServiceRepository, QualityProcessingServiceRepository>();
            services.AddScoped<IQCCompletedServiceRepository, QCCompletedServiceRepository>();
            services.AddScoped<ICreateInvoiceServiceRepository, CreateInvoiceServiceRepository>();
            services.AddScoped<IListInvoiceServiceRepository, ListInvoiceServiceRepository>();


            services.AddScoped<IUpdateOrderDataServiceRepository, UpdateOrderDataServiceRepository>();
            services.AddScoped<IGenerateDeliveryDataServiceRepository, GenerateDeliveryDataServiceRepository>();

            services.AddScoped<IScrapEntryServiceRepository, ScrapEntryServiceRepository>();
            services.AddScoped<IScrapEntryTransServiceRepository, ScrapEntryTransServiceRepository>();
            services.AddScoped<IListDispatchedOrdersServiceRepository, ListDispatchedOrdersServiceRepository>(); 
            services.AddScoped<IRejectedQualityProcessingServiceRepository, RejectedQualityProcessingServiceRepository>();
            services.AddScoped<IFittedOrdersServiceRepository, FittedOrdersServiceRepository>();
            services.AddScoped<IDispatchedServiceRepository, DispatchedServiceRepository>();
            services.AddScoped<IDeliveryAcknowledgementServiceRepository, DeliveryAcknowledgementServiceRepository>();


            services.AddScoped<IAPISubmissionServiceRepository,APISubmissionServiceRepository>();
            services.AddScoped<IFixationImageReuploadServiceRepository, FixationImageReuploadServiceRepository>();
            services.AddScoped<IFixationReUploadedServiceRepository, FixationReUploadedServiceRepository>();
            services.AddScoped<ICancelledorDamagedPlateOrdersServiceRepository, CancelledorDamagedPlateOrdersServiceRepository>();
            services.AddScoped<ITotalCancelledOrdersServiceRepository , TotalCancelledOrdersServiceRepository>();
            services.AddScoped<IHsrpDashboardServiceRepository, HsrpDashboardServiceRepository>();
            services.AddScoped<IOnlineHSRPOrderServiceRepository, OnlineHSRPOrderServiceRepository>();
            services.AddScoped<IOnlineReplacementOrderServiceRepository, OnlineReplacementOrderServiceRepository>();
            return services;
        }
    }
}