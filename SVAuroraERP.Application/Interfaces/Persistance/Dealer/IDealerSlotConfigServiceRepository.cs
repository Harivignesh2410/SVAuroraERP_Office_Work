using SVAuroraERP.Domain.Dealer;

namespace SVAuroraERP.Application.Interfaces.Persistance.Dealer
{
    public interface IDealerSlotConfigServiceRepository
    {
        DataResponse GetDealerSlotConfig();
        DataResponse GetDealerSlotConfig(int? OEMID, int? DealerID, DateTime? FromDate, DateTime? ToDate);
        DataResponse GetDealerSlotConfigToDataTable(DealerSlotConfigDataTableRequest request);
        DataResponse GetDealerSlotConfigByID(int ID);
        DataResponse Save(DealerSlotConfig DealerSlotConfig);
        DataResponse Update(DealerSlotConfig DealerSlotConfig);
        DataResponse Delete(int ConfigID, int UserID, long LoginAuditID);
    }
}

