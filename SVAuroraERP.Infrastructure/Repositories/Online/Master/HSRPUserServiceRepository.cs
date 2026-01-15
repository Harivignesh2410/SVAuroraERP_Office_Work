namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class HSRPUserServiceRepository : IHSRPUserServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<HSRPUserServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IUserServiceRepository _userservice;
        private readonly IAuditLogger _auditLogger;
        public HSRPUserServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<HSRPUserServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IErrorLoggerService errorLoggerService,
                                     IUserServiceRepository userservice,
                                      IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _errorLoggerService = errorLoggerService;
            _userservice = userservice;
            _auditLogger = auditLogger;
        }
        public DataResponse GetAdmin()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPUser.Where(o => o.HSRPUserTypeID == (byte)HSRPUserTypeEnum.Admin).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VHSRPUser", ActionType.ListData, null, resultdata, null, "HSRPUserServiceRepository.GetAdmin()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPUserServiceRepository.GetAdmin()");
            }

            return dataResponse;
        }
        public DataResponse GetEmbossingStation()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPUser.Where(o => o.HSRPUserTypeID == (byte)HSRPUserTypeEnum.EmbossingStation).ToList();

                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VHSRPUser", ActionType.ListData, null, resultdata, null, "HSRPUserServiceRepository.GetEmbossingStation()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPUserServiceRepository.GetEmbossingStation()");
            }

            return dataResponse;
        }
        public DataResponse GetOEM()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPUser.Where(o => o.HSRPUserTypeID == (byte)HSRPUserTypeEnum.OEM).ToList();

                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VHSRPUser", ActionType.ListData, null, resultdata, null, "HSRPUserServiceRepository.GetOEM()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPUserServiceRepository.GetOEM()");
            }

            return dataResponse;
        }
        public DataResponse GetDealerByOEMID(int OEMID)
        {
            DataResponse response = new DataResponse();
            try
            {
                var district = _dbcontext.VHSRPUser.Where(w => w.OEMID == OEMID && w.HSRPUserTypeID != 5 && w.HSRPUserTypeID != 7).ToList();

                response.Count = district.Count;
                response.Value = district;
                response.ID = OEMID;
                _auditLogger.SaveActionLog("HSRPUser", ActionType.Select, OEMID.ToString(), OEMID, null, "HSRPUserServiceRepository.GetDealerByOEMID()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, OEMID, "HSRPUserServiceRepository.GetDealerByOEMID()");
            }
            return response;

        }
        public DataResponse GetOEMSubDealer()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPUser.Where(o => o.HSRPUserTypeID == (byte)HSRPUserTypeEnum.OEMSubUsers).ToList();

                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VHSRPUser", ActionType.ListData, null, null, null, "HSRPUserServiceRepository.GetOEMSubDealer()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPUserServiceRepository.GetOEMSubDealer()");
            }

            return dataResponse;
        }
        public DataResponse GetDealer()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPUser.Where(o => o.HSRPUserTypeID == (byte)HSRPUserTypeEnum.Dealer).ToList();

                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VHSRPUser", ActionType.ListData, null, resultdata, null, "HSRPUserServiceRepository.GetDealer()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPUserServiceRepository.GetDealer()");
            }

            return dataResponse;
        }

        public DataResponse GetDealerByOEMIDForFilter(int oemID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dealerList = _dbcontext.VHSRPUser
                                           .Where(w => w.OEMID == oemID && w.HSRPUserTypeID == 4)
                                           .ToList();

                if (dealerList == null || dealerList.Count == 0)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;
                    return dataResponse;
                }

                dataResponse.Error = false;
                dataResponse.Success = true;
                dataResponse.ID = oemID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = dealerList;

                _auditLogger.SaveActionLog("HSRPUser", ActionType.Select, oemID.ToString(), oemID, null, "HSRPUserServiceRepository.GetDealerByOEMID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, oemID, "HSRPUserServiceRepository.GetDealerByOEMID()");
            }

            return dataResponse;
        }

        public DataResponse GetSubDealer()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPUser.Where(o => o.HSRPUserTypeID == (byte)HSRPUserTypeEnum.DealerSubUsers).ToList();

                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VHSRPUser", ActionType.ListData, null, null, null, "HSRPUserServiceRepository.GetSubDealer()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPUserServiceRepository.GetSubDealer()");
            }

            return dataResponse;
        }
        public DataResponse GetEmbossingStationSubUser()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPUser.Where(o => o.HSRPUserTypeID == (byte)HSRPUserTypeEnum.EmbossingSubUsers).ToList();

                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VHSRPUser", ActionType.ListData, null, null, null, "HSRPUserServiceRepository.GetEmbossingStationSubUser()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPUserServiceRepository.GetEmbossingStationSubUser()");
            }

            return dataResponse;
        }
        public DataResponse GetHSRPUserByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPUser.FirstOrDefault(w => w.HSRPUserID == ID);
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
                _auditLogger.SaveActionLog("HSRPUser", ActionType.Select, ID.ToString(), ID, null, "HSRPUserServiceRepository.GetHSRPUserByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "HSRPUserServiceRepository.GetHSRPUserByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(HSRPUser HSRPUser)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                Task<Tuple<bool, string, int>> obj = null;
                obj = _userservice.SaveUser(HSRPUser.Userdata);

                if (obj.Result.Item1 == false)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = obj.Result.Item2;
                    return dataResponse;
                }
                HSRPUser.UserID = obj.Result.Item3;
                HSRPUser.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.HSRPUser.Add(HSRPUser);
                _dbcontext.SaveChanges();

                dataResponse.ID = HSRPUser.HSRPUserID;
                dataResponse.Message = Constants.SavedSucessfully;
                _auditLogger.SaveActionLog("HSRPUser", ActionType.Insert, HSRPUser.HSRPUserID.ToString(), HSRPUser, null, "HSRPUserServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPUser, "HSRPUserServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(HSRPUser HSRPUser)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {

                var dataexists = _dbcontext.HSRPUser.FirstOrDefault(r => r.HSRPUserID == HSRPUser.HSRPUserID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }

                Task<Tuple<bool, string, int>> obj = null;
                if (dataexists.UserID == null)
                {
                    HSRPUser.Userdata.UserID = 0;
                    obj = _userservice.SaveUser(HSRPUser.Userdata);
                    dataexists.UserID = obj.Result.Item3;
                }
                else
                {
                    HSRPUser.Userdata.UserID = (int)dataexists.UserID;

                    obj = _userservice.UpdateUser(HSRPUser.Userdata);
                }

                if (obj.Result.Item1 == false)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = obj.Result.Item2;
                    return dataResponse;
                }
                _auditLogger.SaveActionLog("HSRPUser", ActionType.Update, dataexists.UserID.ToString(), HSRPUser, dataexists, "HSRPUserServiceRepository.Update()");
                dataexists.OEMID = HSRPUser.OEMID;
                dataexists.DealerID = HSRPUser.DealerID;
                dataexists.EmbossingStationID = HSRPUser.EmbossingStationID;
                dataexists.HSRPUserCode = HSRPUser.HSRPUserCode;
                dataexists.CompanyName = HSRPUser.CompanyName;
                dataexists.Address1 = HSRPUser.Address1;
                dataexists.Address2 = HSRPUser.Address2;
                dataexists.LastUpdatedBy = HSRPUser.LastUpdatedBy;
                dataexists.DistrictID = HSRPUser.DistrictID;
                dataexists.City = HSRPUser.City;
                dataexists.Pincode = HSRPUser.Pincode;
                dataexists.GSTIN = HSRPUser.GSTIN;
                dataexists.OEMID = HSRPUser.OEMID;
                dataexists.ContactPerson = HSRPUser.ContactPerson;
                dataexists.ContactNo = HSRPUser.ContactNo;
                dataexists.DeliveryAddress1 = HSRPUser.DeliveryAddress1;
                dataexists.DeliveryAddress2 = HSRPUser.DeliveryAddress2;
                dataexists.DeliveryDistrictID = HSRPUser.DeliveryDistrictID;
                dataexists.DeliveryCity = HSRPUser.DeliveryCity;
                dataexists.DeliveryPincode = HSRPUser.DeliveryPincode;
                dataexists.IsActive = HSRPUser.IsActive;
                dataexists.IsDealerEnabledOnline = HSRPUser.IsDealerEnabledOnline;
                dataexists.IsOEMEnabledOnline = HSRPUser.IsOEMEnabledOnline;
                dataexists.OnlineOEMName = HSRPUser.OnlineOEMName;
                dataexists.LastUpdatedBy = HSRPUser.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.HSRPUserID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPUser, "HSRPUserServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int HSRPUserID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.HSRPUser.FirstOrDefault(w => w.HSRPUserID == HSRPUserID);


                if (dataexists == null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = true;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }

                Task<Tuple<bool, string>> obj = null;

                var FKUserID = (int)dataexists.UserID;

                obj = _userservice.DeleteUser(FKUserID, LoginAuditID);

                if (obj.Result.Item1 == false)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = obj.Result.Item2;
                    return dataResponse;
                }

                dataexists.LastUpdatedDate = DateTime.UtcNow;
                dataexists.LastUpdatedBy = UserID;
                dataexists.IsDeleted = true;
                _dbcontext.SaveChanges();

                dataResponse.ID = dataexists.HSRPUserID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("HSRPUser", ActionType.Delete, null, HSRPUserID, null, "HSRPUserServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPUserID, "HSRPUserServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetApplication()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.LkupApplication.OrderBy(O => O.ApplicationName).ToList();

                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("LkupApplication", ActionType.ListData, null, null, null, "HSRPUserServiceRepository.GetApplication()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPUserServiceRepository.GetApplication()");
            }

            return dataResponse;
        }

        public DataResponse GetRoleIDByPageID(int PageID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                VRole? resultdata = null;
                if (PageID == 1)
                {
                    resultdata = _dbcontext.VRole.Where(w => w.RoleID == (_dbcontext.HSRPRoleConfig.FirstOrDefault()).AdminRoleID).FirstOrDefault();
                }
                else if (PageID == 2)
                {
                    resultdata = _dbcontext.VRole.Where(w => w.RoleID == (_dbcontext.HSRPRoleConfig.FirstOrDefault()).EmbossingStationRoleID).FirstOrDefault();
                }
                else if (PageID == 3)
                {
                    resultdata = _dbcontext.VRole.Where(w => w.RoleID == (_dbcontext.HSRPRoleConfig.FirstOrDefault()).OEMRoleID).FirstOrDefault();
                }
                else if (PageID == 4)
                {
                    resultdata = _dbcontext.VRole.Where(w => w.RoleID == (_dbcontext.HSRPRoleConfig.FirstOrDefault()).DealerRoleID).FirstOrDefault();
                }
                else if (PageID == 5)
                {
                    resultdata = _dbcontext.VRole.Where(w => w.RoleID == (_dbcontext.HSRPRoleConfig.FirstOrDefault()).DealerSubUserID).FirstOrDefault();
                }
                else if (PageID == 6)
                {
                    resultdata = _dbcontext.VRole.Where(w => w.RoleID == (_dbcontext.HSRPRoleConfig.FirstOrDefault()).EmbossingSubUserID).FirstOrDefault();
                }
                else if (PageID == 7)
                {
                    resultdata = _dbcontext.VRole.Where(w => w.RoleID == (_dbcontext.HSRPRoleConfig.FirstOrDefault()).OEMSubUserID).FirstOrDefault();
                }
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VRole", ActionType.Select, PageID.ToString(), PageID, null, "HSRPUserServiceRepository.GetRoleIDForAdmin()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPUserServiceRepository.GetRoleIDForAdmin()");
            }
            return dataResponse;
        }

        public DataResponse GetHSRPUserByUserID(int UserID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPUser.FirstOrDefault(w => w.UserID == UserID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                dataResponse.ID = UserID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VHSRPUser", ActionType.Select, UserID.ToString(), UserID, null, "HSRPUserServiceRepository.GetHSRPUserByUserID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, UserID, "HSRPUserServiceRepository.GetHSRPUserByUserID()");
            }

            return dataResponse;
        }
        public DataResponse GetDealerListByOEM(OEMDataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VDealerByOEM> query = _dbcontext.VDealerByOEM;
                if (request.OEMID > 0)
                    query = query.Where(o => o.OEMID == request.OEMID);

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.CompanyName.Contains(request.SearchValue) ||
                                     d.HSRPUserCode.Contains(request.SearchValue)
                                     );
                }
                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VDealerByOEM.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                           .Select(w => new
                                           {
                                               w.HSRPUserID,
                                               w.CompanyName,
                                               w.HSRPUserCode,
                                               w.Address1,
                                               w.Address2,
                                               w.Pincode,
                                               w.City,
                                               w.DistrictName,
                                               w.StateName,
                                               w.DeliveryAddress1,
                                               w.DeliveryAddress2,
                                               w.DeliveryCity,
                                               w.DeliveryPincode,
                                               w.DeliveryDistrictName,
                                               w.DeliveryStateName,
                                               w.GSTIN,
                                               w.ContactPerson,
                                               w.ContactNo,
                                               w.IsActive,
                                               w.LastUpdatedBy,
                                               w.LastUpdatedDate
                                           }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VQCCompleted", ActionType.ListData, null, request, null, "CreateInvoiceServiceRepository.GetListInvoiceTrans()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "CreateInvoiceServiceRepository.GetListInvoiceTrans()");
            }
            return response;
        }
        public DataResponse GetOEMByEmbossingStation(int EmbossingStationID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                List<VHSRPUser> resultdata;

                if (EmbossingStationID > 0)
                {
                    var oemIds = _dbcontext.VOEMVendorDealerMapping
                        .Where(o => o.EmbossingStationID == EmbossingStationID)
                        .Select(o => o.HSRPOEMID)
                        .Distinct()
                        .ToList();

                    resultdata = _dbcontext.VHSRPUser
                        .Where(o => oemIds.Contains(o.HSRPUserID))
                        .ToList();
                }
                else
                {
                    resultdata = _dbcontext.VHSRPUser
                        .Where(o => o.HSRPUserTypeID == (byte)HSRPUserTypeEnum.OEM)
                        .ToList();
                }

                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VHSRPUser", ActionType.ListData, null, resultdata, null, "HSRPUserServiceRepository.GetOEMByEmbossingStation()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPUserServiceRepository.GetOEMByEmbossingStation()");
            }
            return dataResponse;
        }
        public DataResponse GetEmbossingStationByUser(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPUser.Where(w => w.HSRPUserID == ID).Select(w => new { w.HSRPUserID, w.CompanyName, w.City }).ToList();

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
                _auditLogger.SaveActionLog("HSRPUser", ActionType.Select, ID.ToString(), ID, null, "HSRPUserServiceRepository.GetHSRPUserByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "HSRPUserServiceRepository.GetHSRPUserByID()");
            }

            return dataResponse;
        }
        public DataResponse GetEmbossingStationByHSRPOnlineOrderID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var oemdata = _dbcontext.VOnlineHSRPOrder.FirstOrDefault(w => w.OnlineHSRPOrderID == ID);

                var resultdata = _dbcontext.VOEMVendorDealerMapping.Where(w => w.HSRPOEMID == oemdata.OEMID && w.DealerID == oemdata.DealerID).Select(w => new { w.EmbossingStationID, w.EmbossingStationName }).ToList();

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
                _auditLogger.SaveActionLog("VOEMVendorDealerMapping", ActionType.Select, ID.ToString(), ID, null, "HSRPUserServiceRepository.GetEmbossingStationByHSRPOnlineOrderID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "HSRPUserServiceRepository.GetEmbossingStationByHSRPOnlineOrderID()");
            }

            return dataResponse;
        }
        public DataResponse GetHSRPUserDataTableList(HSRPUserRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHSRPUser> query = _dbcontext.VHSRPUser;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.OEMName).Contains(request.SearchValue)
                                            || (d.CompanyName).Contains(request.SearchValue) 
                                            || (d.HSRPUserCode).Contains(request.SearchValue)
                                            || (d.DealerName).Contains(request.SearchValue)
                                            || (d.EmbossingStationName).Contains(request.SearchValue)
                                            || (d.OEMCompanyName).Contains(request.SearchValue)
                                            || (d.ContactPerson).Contains(request.SearchValue)
                                            || (d.Pincode).Contains(request.SearchValue)
                                            || (d.DeliveryPincode).Contains(request.SearchValue)
                                            || (d.DeliveryDistrict).Contains(request.SearchValue)
                                            || (d.DistrictName).Contains(request.SearchValue));
                }
                if (request.UserTypeID>0)
                {
                    { query = query.Where(w => w.HSRPUserTypeID == request.UserTypeID); }
                }
                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VHSRPUser.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.HSRPUserID,
                                w.OEMName,
                                w.ApplicationName,
                                w.RoleName,
                                w.HSRPUserCode,
                                w.CompanyName,
                                w.ContactPerson, 
                                w.Address1,
                                w.Address2,
                                w.City,
                                w.DistrictName,
                                w.Pincode,
                                w.DeliveryAddress1,
                                w.DeliveryAddress2,
                                w.DeliveryCity,
                                w.DeliveryDistrict,
                                w.DeliveryPincode,
                                w.IsActive,
                                w.DealerName,
                                w.EmbossingStationName,
                                w.OEMCompanyName
                            }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("HSRPUser", ActionType.Select, null, request, null, "HSRPUserServiceRepository.GetHSRPUserDataTableList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "HSRPUserServiceRepository.GetHSRPUserDataTableList()");
            }
            return response;
        }
    }
}