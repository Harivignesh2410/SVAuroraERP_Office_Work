using Azure.Core;

namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class VehiclePlateImageServiceRepository : IVehiclePlateImageServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<VehiclePlateImageServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public VehiclePlateImageServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<VehiclePlateImageServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetVehiclePlateImage()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VVehiclePlateImage.ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VVehiclePlateImage", ActionType.ListData, null, null,null, "VehiclePlateImageServiceRepository.GetVehiclePlateImage()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "VehiclePlateImageServiceRepository.GetVehiclePlateImage()");
            }

            return dataResponse;
        }
        public DataResponse GetVehiclePlateImageByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VVehiclePlateImage.FirstOrDefault(w => w.VehiclePlateImageID == ID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                dataResponse.ID = ID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VVehiclePlateImage", ActionType.Select, ID.ToString(), ID, null, "VehiclePlateImageServiceRepository.GetVehiclePlateImageByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "VehiclePlateImageServiceRepository.GetVehiclePlateImageByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(VehiclePlateImage VehiclePlateImage)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.VehiclePlateImage.FirstOrDefault(r => r.VehiclePlateSizeID == VehiclePlateImage.VehiclePlateSizeID && r.VehiclePlateColorID==VehiclePlateImage.VehiclePlateColorID);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.VehiclePlateImageID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                VehiclePlateImage.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.VehiclePlateImage.Add(VehiclePlateImage);
                dataResponse.Message = Constants.SuccessMessage;
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("VehiclePlateImage", ActionType.Insert, VehiclePlateImage.VehiclePlateImageID.ToString(), VehiclePlateImage,null, "VehiclePlateImageServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, VehiclePlateImage, "VehiclePlateImageServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(VehiclePlateImage VehiclePlateImage)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.VehiclePlateImage.FirstOrDefault(r => r.VehiclePlateImageID!= VehiclePlateImage.VehiclePlateImageID 
                                                                    && r.VehiclePlateSizeID == VehiclePlateImage.VehiclePlateSizeID
                                                                    && r.VehiclePlateColorID == VehiclePlateImage.VehiclePlateColorID);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.VehiclePlateColorID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.VehiclePlateImage.FirstOrDefault(r => r.VehiclePlateImageID == VehiclePlateImage.VehiclePlateImageID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("Box", ActionType.Update, dataexists.VehiclePlateImageID.ToString(), VehiclePlateImage, dataexists, "VehiclePlateImageServiceRepository.Update()");
                dataexists.VehiclePlateSizeID = VehiclePlateImage.VehiclePlateSizeID;
                dataexists.VehiclePlateColorID = VehiclePlateImage.VehiclePlateColorID;
                dataexists.FrontImageURL = VehiclePlateImage.FrontImageURL;
                dataexists.RearImageURL = VehiclePlateImage.RearImageURL;
                dataexists.LastUpdatedBy = VehiclePlateImage.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.VehiclePlateColorID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, VehiclePlateImage, "VehiclePlateImageServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int VehiclePlateImageID, int UserID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.VehiclePlateImage.FirstOrDefault(w => w.VehiclePlateImageID == VehiclePlateImageID);
                if (dataexists == null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = true;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }

                dataexists.LastUpdatedDate = DateTime.UtcNow;
                dataexists.LastUpdatedBy = UserID;
                dataexists.IsDeleted = true;
                _dbcontext.SaveChanges();

                dataResponse.ID = dataexists.VehiclePlateImageID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("VehiclePlateImage", ActionType.Delete, null, VehiclePlateImageID,null, "VehiclePlateImageServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, VehiclePlateImageID, "VehiclePlateImageServiceRepository.Delete()");
            }

            return dataResponse;
        }
    }
}