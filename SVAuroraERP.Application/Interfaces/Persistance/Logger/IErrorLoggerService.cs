namespace SVAuroraERP.Application.Interfaces.Persistance.Logger
{
    public interface IErrorLoggerService
    {
        DataResponse LogException(Exception ex, object requestObject, string methodName);
    }
}