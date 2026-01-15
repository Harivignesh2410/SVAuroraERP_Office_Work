namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface IMachineServiceRepository
    {
        DataResponse GetMachineList();
        DataResponse GetMachineByID(int ID);
        DataResponse Save(Machine Machine);
        DataResponse Update(Machine Machine);
        DataResponse Delete(int MachineID, int UserID, long LoginAuditID);
        DataResponse GetMachineTypeList();
        DataResponse GetMachineDataTable(DataTableRequest request);
    }
}
