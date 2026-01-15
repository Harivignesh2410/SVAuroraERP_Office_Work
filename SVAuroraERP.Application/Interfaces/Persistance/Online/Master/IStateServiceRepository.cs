namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface  IStateServiceRepository
    {
        DataResponse GetState();
        DataResponse GetStateByID(int ID);
        DataResponse Save(State State);
        DataResponse Update(State State);
        DataResponse Delete(int StateID, int UserID, long LoginAuditID);
        DataResponse GetStateDataTableList(DataTableRequest request);
    }
}