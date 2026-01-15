namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface ICompanyServiceRepository
    {
        DataResponse GetCompany();
        DataResponse Save(Company company);
    }
}