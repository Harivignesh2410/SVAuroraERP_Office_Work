namespace SVAuroraERP.Application.Interfaces.Persistance.HR
{
    public interface IDesignationServiceRepository
    {
        DataResponse GetDesignation();
        DataResponse GetByID(int ID);
        DataResponse Save(Designation Designation);
        DataResponse Update(Designation Designation);
        DataResponse Delete(int DesignationID, int UserID, long LoginAuditID);
        DataResponse GetDesignationDataTable(DataTableRequest request);

    }
}
