namespace SVAuroraERP.Application.Interfaces.Persistance.HR
{
    public interface IEmployeeServiceRepository
    {
        DataResponse GetEmployee();
        DataResponse GetByID(int ID);
        DataResponse Save(Employee Employee);
        DataResponse Update(Employee Employee);
        DataResponse Delete(int ID);
        DataResponse GetBloodGroupList();
        DataResponse GetEmployeeDataTable(DataTableRequest request);
    }
}
