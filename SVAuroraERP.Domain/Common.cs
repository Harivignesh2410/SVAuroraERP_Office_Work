namespace SVAuroraERP.Domain
{
    //Added on 2024.12.16 by Sivakumar
    public class Constants
    {
        public const string ExceptionMessage = "Something went wrong! Please contact Administrator with Request Payload Detail(s).";

        public const string WarningMessage = "Something went wrong! Please contact Administrator with Request Payload Detail(s).";
        public const string InvalidLoginSession = "Invalid Login Session/Expired, Please SignIn back!";
        public const string InvalidRequestPayload = "Invalid Request Payload";
        public const string SuccessMessage = "Success";
        public const string FailedMessage = "Failed";
        public const string TokenExpired = "Token Expired";
        public const string AppVersionMissing = "App Version Missing!";
        public const string InvalidPassword = "Invalid Password";
        public const string InvalidUsernamePassword = "Invalid Username/Password";
        public const string RequestDataNotFound = "Request Data Not Found!";
        public const string NoRecordFound = "NoRecord";
        public const string MandatoryDataMissing = "Mandatory Fields Data Missing";
        public const string SubscriptionDetailsNotFound = "Subscription Details Not Found";
        public const string RecordFound = "Record Found";
        public const string DataAlreadyExist = "Data Already Exitst";


        //common
        public const string UpdatedSucessfully = "Updated Sucessfully";
        public const string SavedSucessfully = "Saved Sucessfully";
        public const string DeletedSuccesfully = "Deleted Sucessfully";

        //Added on 2025.03.13
        public const string DateFormat = "dd/MM/yyyy";
    }

    public class StoredProcedure
    {
        public const string INSERTSTOCKREQUEST = "dbo.InsertStockRequest";
        public const string UPDATESTOCKREQUEST = "dbo.UpdateStockRequest";
        public const string DELETESTOCKREQUEST = "dbo.DeleteStockRequest";

        public const string INSERTSTOCKREQUESTTRANS = "dbo.InsertStockRequestTrans";
        public const string DELETESTOCKREQUESTTRANS = "dbo.DeleteStockRequestTrans";

        public const string UPDATESTOCKREQUESTSTATUS = "dbo.UpdateStockRequestStatus";

        public const string INSERTPRODUCTIONINWARD = "dbo.InsertProductionInward";
        public const string UPDATEPRODUCTIONINWARD = "dbo.UpdateProductionInward";
        public const string DELETEPRODUCTIONINWARD = "dbo.DeleteProductionInward";

        public const string INSERTPRODUCTIONCONSUMPTION = "dbo.InsertProductionConsumption";
        public const string UPDATEPRODUCTIONCONSUMPTION = "dbo.UpdateProductionConsumption";
        public const string DELETEPRODUCTIONCONSUMPTION = "dbo.DeleteProductionConsumption";

        public const string INSERTHOLOGRAMPUNCHING = "dbo.InsertHologramPunching";

        public const string INSERTLASERNOMARKING = "dbo.InsertLaserNoMarking";
        public const string GETAVAILABLELASERNOS = "dbo.GetAvailableLaserNos";
        public const string INSERTLASERCONSUMPTION = "dbo.InsertLaserConsumption";
        public const string UPDATELASERNOMARKING = "dbo.UpdateLaserNoMarking";
        public const string DELETELASERNOMARKING = "dbo.DeleteLaserNoMarking";
        public const string COMPLETELASERNOMARKING = "dbo.CompleteLaserNoMarking";

        public const string INSERTPACKING = "dbo.InsertPacking";
        public const string INSERTPACKINGTRANS = "dbo.InsertPackingTrans";
        public const string DELETEPACKINGTRANS = "dbo.DeletePackingTrans";

        public const string INSERTNUMBERPLATEDISPATCH = "dbo.InsertNumberPlateDispatch";
        public const string INSERTNUMBERPLATEDISPATCHTRANS = "dbo.InsertNumberPlateDispatchTrans";
        public const string DELETENUMBERPLATEDISPATCHTRANS = "dbo.DeleteNumberPlateDispatchTrans";
        public const string GETPACKINGBYNUMBERPLATEDISPATCHID = "dbo.GetPackingByNumberPlateDispatchID";
        public const string DELETENUMBERPLATEDISPATCH = "dbo.DeleteNumberPlateDispatch";

        public const string INSERTHYDROLICPRESSURE = "dbo.InsertHydrolicPressure";
        public const string INSERTHYDROLICCONSUMPTION = "dbo.InsertHydrolicConsumption";
        public const string UPDATEHYDROLICPRESSURE = "dbo.UpdateHydrolicPressure";
        public const string GETHYDROLICPRESSUREDETAILS = "dbo.GetHydrolicPressureDetails";
        public const string DELETEHYDROLICPRESSURE = "dbo.DeleteHydrolicPressure";
        public const string COMPLETEHYDROLICPRESSURE = "dbo.CompleteHydrolicPressure";
        public const string UPDATEHYDROLICCONSUMPTION = "dbo.UpdateHydrolicConsumption";

        public const string INSERTHOLOGRAMCONSUMPTION = "dbo.InsertHologramConsumption";
        public const string DELETEHOLOGRAMPUNCHING = "dbo.DeleteHologramPunching";
        public const string UPDATEHOLOGRAMPUNCHING = "dbo.UpdateHologramPunching";
        public const string COMPLETEHOLOGRAMPUNCHING = "dbo.CompleteHologramPunching";
        public const string DELETEIMPORTEDDATA = "dbo.DeleteImportedData";
        //Added on 10/09/25
        public const string INSERTHSRPLASERSTOCKTRANSID = "dbo.InsertHSRPLaserStockTransID";
        //Added on 2025.10.13
        public const string ALLOCATEORDERLASERNO = "dbo.AllocateOrderLaserNo";
        // Added on 2025.10.14
        public const string GETDEALERPENDINGSUMMARY = "dbo.GetDealerPendingSummary";
        public const string GETCANCELLEDORDAMAGEDPLATEORDERS = "dbo.GetCancelledOrDamagedPLateOrders";
        public const string GETTOTALCANCELLEDORDERS = "dbo.GetTotalCancelledOrders";

        // Added on 2025.10.31 by Harivignesh
        public const string GETSCRAPDATABYCOMPONENTTYPEID = "dbo.GetScrapDataByComponentTypeID";
        public const string INSERTSCRAPENTRY = "dbo.InsertScrapEntry";
        public const string INSERTORUPDATESCRAPENTRYTRANS = "dbo.InsertOrUpdateScrapEntryTrans";
        public const string DELETESCRAPENTRYDATA = "dbo.DeleteScrapEntryData";
        public const string UPDATELASERNOFORORDER = "dbo.UpdateLaserNoForOrder";
        public const string REJECTHSRPLASERNOPLATE = "dbo.RejectHSRPLaserNoPlate";
        public const string GETNUMBERPLATESTOCKREPORT = "dbo.GetNumberPlateStockReport";
        public const string GETSCRAPSTOCK = "dbo.GetScrapStock";

        public const string INSERTRECTIFYLASERPLATE = "dbo.InsertRectifyLaserPlate";
        public const string CHECKAVAILABLEORDERLASERNO = "dbo.CheckAvailableOrderLaserNo";
        public const string GETHSRPDASHBOARD = "dbo.GetHSRPDashboard";

    }
    public class Common
    {
        public enum Pages
        {
            Unit = 3,
            Tax = 4,
            Size = 5,
            Item = 6,
            Roles = 7,
            RoleConfiguration = 8,
            ManageUsers = 9,
            Designation = 10,
            Employee = 11,
            Supplier = 12,
            Company = 13,
            Color = 14,
            PurchaseEntry = 15,
            OtherCharges = 16,
            PendingInspection = 17,
            ComponentType = 18,
            PurchaseOrder = 19,
            Category = 20,
            DocumentGroup = 21,
            DocumentType = 22,
            StockReport = 23,
            CompletedInspection = 24,
            SearchPurchaseEntry = 25,
            Warehouse = 26,
            RackLocation = 27,
            ProductionConfiguration = 28,
            StockRequest = 29,
            PendingForApproval = 30,
            BatchStock = 31,
            ProductionInward = 32,
            ComponenetStock = 33,
            Machine = 34,
            HologramPunching = 35,
            LaserNoMarking = 36,
            Packing = 37,
            Box = 38,
            Courier = 39,
            NumberPlateDispatch = 40,
            DeliveryAcknowledgement = 41,
            ProductionCalculation = 60,
            HydrolicPressure = 61,
            AppLauncher = 1,
            Dashboard = 2,
            AssignedLaserNo = 69,
            HSRPConfig = 75,
            District = 48,
            HomeFitmentPincode = 49,
            HSRPPartNumber = 59,
            HSRPPlateDimension = 51,
            HSRPReplacementReason = 63,
            HSRPReplacementDocument = 64,
            OEMPricing = 62,
            OnlinePlatePrice = 50,
            State = 47,
            VehicleClass = 42,
            VehiclePlateColor = 43,
            VehiclePlateImage = 46,
            VehiclePlateSize = 44,
            VehiclePlateSizeMapping = 45,
            OEMVendorCodeMapping = 66,
            OEMVendorDealerMapping = 67,
            DealerSubUser = 57,
            EmbossingSubUser = 58,
            OEMSubUser = 56,
            Admin = 52,
            Dealer = 55,
            EmbossingStation = 53,
            OEM = 54,
            ImportOEMData = 65,
            CreateInvoice = 80,
            ListInvoice = 81,
            ManageJobCard = 77,
            Allorder = 70,
            CreateJobCard = 74,
            LaserNoAllocation = 71,
            QCCompleted = 79,
            QualityProcessing = 78,
            GenerateDeliveryData = 82,
            ScrapEntry = 83,
            UpdateOrderData = 76,
            ListDeliveryOrders = 84,
            AcknowledgeDispatchedOrders = 85,
            ViewAllOrder = 88,
            ViewLaserNoAllocation = 89,
            ViewCreateJobCard = 90,
            ViewQualityProcessing = 91,
            ViewQCCompleted = 92,
            ViewAssignedLaserNo = 87,
            DealerList = 86,
            RejectedQualityProcessing = 94,
            RawMaterialReport = 95,
            NumberPlateStock = 96,
            ScrabStock = 97,
            HSRRPVehiclePlateImage = 100,
            APISubmittedOrders = 101,
            FixationReUploaded = 103,
            TotalCancelOrder = 105,
            CancelledorDamagedPlateOrders = 106,
            HsrpDeliveryAcknowledgement = 99,
            ViewDispatched = 107,
            Dispatched = 98,
            ViewRejectedQualityProcessing = 108,
            ViewFittedOrders = 110,
            ExportInvoice = 104,
            ViewDeliveryAcknowledgement = 109,
            FixationImageReupload = 102,
            ViewFixationImageReupload = 111,
            ViewFixationReUploaded = 112,
            ViewAPISubmittedOrders = 113,
            ViewTotalCancelledOrder = 114,
            ViewCancelledorDamagedPlateOrders = 115,
            HSRPDashboard = 117,
            ApproveOnlineOrders = 116,
            ApproveReplacementOrders = 118,
            HolidayType = 86, //Added on 2025.12.29
            DealerHoliday = 119,
            DealerWorkingDay = 120,
            TimeSlot = 121,
            DealerSlotConfig = 122
        }

        //Added on 2025.11.06
        public enum Application
        {
            Inventory = 1,
            HSRPPortal = 2
        }

        //Added on 2025.11.08
        public enum HSRPOrderStatus
        {
            ReadyforProcessing = 1,
            LaserNoAssigned = 2,
            JobCardGenerated = 3,
            QualityProcessing = 4,
            QCCompleted = 5,
            InvoiceGenerated = 6,
            Dispatched = 7,
            Delivered = 8,
            CancelledOrders = 12
        }

        public enum HSRPUserType
        {
            Admin = 1,
            Dealer = 4,
            EmbossingStation = 2,
            OEM = 3
        }
    }
}