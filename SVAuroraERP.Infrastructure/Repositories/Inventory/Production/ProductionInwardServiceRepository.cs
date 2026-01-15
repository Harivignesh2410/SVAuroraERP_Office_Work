namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Production
{
    public class ProductionInwardServiceRepository : IProductionInwardServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;

        public ProductionInwardServiceRepository(SVAuroraERPDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public Tuple<bool, bool> SaveProductionInward(ProductionInward request)
        {
            int id = 0;
            bool IsSuccess = false;
            bool IsError = false;
            using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = StoredProcedure.INSERTPRODUCTIONINWARD;
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var pkProductionInwardIDParam = command.CreateParameter();
                    pkProductionInwardIDParam.ParameterName = "@PK_ProductionInwardID";
                    pkProductionInwardIDParam.Direction = System.Data.ParameterDirection.Output;
                    pkProductionInwardIDParam.DbType = System.Data.DbType.Int32;
                    pkProductionInwardIDParam.Value = 0;

                    var StockRequestIDParam = command.CreateParameter();
                    StockRequestIDParam.ParameterName = "@StockRequestID";
                    StockRequestIDParam.Value = request.StockRequestID;

                    var OutputComponentTypeID = command.CreateParameter();
                    OutputComponentTypeID.ParameterName = "@OutputComponentTypeID";
                    OutputComponentTypeID.Value = request.OutputComponentTypeID;

                    var ExpectedProductionQtyParam = command.CreateParameter();
                    ExpectedProductionQtyParam.ParameterName = "@ExpectedProductionQty";
                    ExpectedProductionQtyParam.Value = request.ExpectedProductionQty;

                    var ActualProductionQtyParam = command.CreateParameter();
                    ActualProductionQtyParam.ParameterName = "@ActualProductionQty";
                    ActualProductionQtyParam.Value = request.ActualProductionQty;

                    var ItemIDParam = command.CreateParameter();
                    ItemIDParam.ParameterName = "@ItemID";
                    ItemIDParam.Value = request.ItemID;

                    var RackLocationIDParam = command.CreateParameter();
                    RackLocationIDParam.ParameterName = "@RackLocationID";
                    RackLocationIDParam.Value = request.RackLocationID;

                    var LastUpdatedByParam = command.CreateParameter();
                    LastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                    LastUpdatedByParam.Value = request.LastUpdatedBy;

                    var FKOperatorIParam = command.CreateParameter();
                    FKOperatorIParam.ParameterName = "@FK_OperatorID";
                    FKOperatorIParam.Value = request.OperatorID;

                    command.Parameters.Add(pkProductionInwardIDParam);
                    command.Parameters.Add(StockRequestIDParam);
                    command.Parameters.Add(OutputComponentTypeID);
                    command.Parameters.Add(ExpectedProductionQtyParam);
                    command.Parameters.Add(ActualProductionQtyParam);
                    command.Parameters.Add(ItemIDParam);
                    command.Parameters.Add(RackLocationIDParam);
                    command.Parameters.Add(LastUpdatedByParam);
                    command.Parameters.Add(FKOperatorIParam);

                    command.ExecuteNonQuery();
                    id = (int)pkProductionInwardIDParam.Value;

                }
            }
            foreach (var productionconsumption in request.ProductionConsumption)
            {
                productionconsumption.ProductionInwardID = id;
                if (SaveProductionConsumption(productionconsumption)) IsSuccess = true;
                else IsError = true;
            }
            return Tuple.Create(IsSuccess, IsError);
        }
        public bool SaveProductionConsumption(ProductionConsumption request)
        {
            bool IsSuccess = false;
            using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = StoredProcedure.INSERTPRODUCTIONCONSUMPTION;
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var ProductionInwardIDParam = command.CreateParameter();
                    ProductionInwardIDParam.ParameterName = "@ProductionInwardID";
                    ProductionInwardIDParam.Value = request.ProductionInwardID;

                    var StockRequestTransIDParam = command.CreateParameter();
                    StockRequestTransIDParam.ParameterName = "@StockRequestTransID";
                    StockRequestTransIDParam.Value = request.StockRequestTransID;

                    var ActualConsumedQtyParam = command.CreateParameter();
                    ActualConsumedQtyParam.ParameterName = "@ActualConsumedQty";
                    ActualConsumedQtyParam.Value = request.ActualConsumedQty;

                    var WastageQtyParam = command.CreateParameter();
                    WastageQtyParam.ParameterName = "@WastageQty";
                    WastageQtyParam.Value = request.WastageQty;

                    var WastagePercentageParam = command.CreateParameter();
                    WastagePercentageParam.ParameterName = "@WastagePercentage";
                    WastagePercentageParam.Value = request.WastagePercentage;

                    var BalanceQtyParam = command.CreateParameter();
                    BalanceQtyParam.ParameterName = "@BalanceQty";
                    BalanceQtyParam.Value = request.BalanceQty;


                    command.Parameters.Add(ProductionInwardIDParam);
                    command.Parameters.Add(StockRequestTransIDParam);
                    command.Parameters.Add(ActualConsumedQtyParam);
                    command.Parameters.Add(WastageQtyParam);
                    command.Parameters.Add(WastagePercentageParam);
                    command.Parameters.Add(BalanceQtyParam);

                    command.ExecuteNonQuery();
                    IsSuccess = true;
                }
            }
            return IsSuccess;
        }
        public Tuple<bool, bool> UpdateProductionInward(ProductionInward request)
        {
            bool IsSuccess = false;
            bool IsError = false;

            using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = StoredProcedure.UPDATEPRODUCTIONINWARD;
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var productionInwardIDParam = command.CreateParameter();
                    productionInwardIDParam.ParameterName = "@ProductionInwardID";
                    productionInwardIDParam.Value = request.ProductionInwardID;

                    var stockRequestIDParam = command.CreateParameter();
                    stockRequestIDParam.ParameterName = "@StockRequestID";
                    stockRequestIDParam.Value = request.StockRequestID;

                    var outputComponentTypeIDParam = command.CreateParameter();
                    outputComponentTypeIDParam.ParameterName = "@OutputComponentTypeID";
                    outputComponentTypeIDParam.Value = request.OutputComponentTypeID;

                    var expectedProductionQtyParam = command.CreateParameter();
                    expectedProductionQtyParam.ParameterName = "@ExpectedProductionQty";
                    expectedProductionQtyParam.Value = request.ExpectedProductionQty;

                    var actualProductionQtyParam = command.CreateParameter();
                    actualProductionQtyParam.ParameterName = "@ActualProductionQty";
                    actualProductionQtyParam.Value = request.ActualProductionQty;

                    var itemIDParam = command.CreateParameter();
                    itemIDParam.ParameterName = "@ItemID";
                    itemIDParam.Value = request.ItemID;

                    var lastUpdatedByParam = command.CreateParameter();
                    lastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                    lastUpdatedByParam.Value = request.LastUpdatedBy;

                    command.Parameters.Add(productionInwardIDParam);
                    command.Parameters.Add(stockRequestIDParam);
                    command.Parameters.Add(outputComponentTypeIDParam);
                    command.Parameters.Add(expectedProductionQtyParam);
                    command.Parameters.Add(actualProductionQtyParam);
                    command.Parameters.Add(itemIDParam);
                    command.Parameters.Add(lastUpdatedByParam);

                    command.ExecuteNonQuery();
                }
            }
            foreach (var productionconsumption in request.ProductionConsumption)
            {
                productionconsumption.ProductionInwardID = request.ProductionInwardID;
                if (UpdateProductionConsumption(productionconsumption)) IsSuccess = true;
                else IsError = true;
            }

            return Tuple.Create(IsSuccess, IsError);
        }
        public bool UpdateProductionConsumption(ProductionConsumption request)
        {
            bool IsSuccess = false;

            using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = StoredProcedure.UPDATEPRODUCTIONCONSUMPTION;
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var productionConsumptionIDParam = command.CreateParameter();
                    productionConsumptionIDParam.ParameterName = "@ProductionConsumptionID";
                    productionConsumptionIDParam.Value = request.ProductionConsumptionID;

                    var productionInwardIDParam = command.CreateParameter();
                    productionInwardIDParam.ParameterName = "@ProductionInwardID";
                    productionInwardIDParam.Value = request.ProductionInwardID;

                    var stockRequestTransIDParam = command.CreateParameter();
                    stockRequestTransIDParam.ParameterName = "@StockRequestTransID";
                    stockRequestTransIDParam.Value = request.StockRequestTransID;

                    var actualConsumedQtyParam = command.CreateParameter();
                    actualConsumedQtyParam.ParameterName = "@ActualConsumedQty";
                    actualConsumedQtyParam.Value = request.ActualConsumedQty;

                    var wastageQtyParam = command.CreateParameter();
                    wastageQtyParam.ParameterName = "@WastageQty";
                    wastageQtyParam.Value = request.WastageQty;

                    var wastagePercentageParam = command.CreateParameter();
                    wastagePercentageParam.ParameterName = "@WastagePercentage";
                    wastagePercentageParam.Value = request.WastagePercentage;

                    var balanceQtyParam = command.CreateParameter();
                    balanceQtyParam.ParameterName = "@BalanceQty";
                    balanceQtyParam.Value = request.BalanceQty;

                    command.Parameters.Add(productionConsumptionIDParam);
                    command.Parameters.Add(productionInwardIDParam);
                    command.Parameters.Add(stockRequestTransIDParam);
                    command.Parameters.Add(actualConsumedQtyParam);
                    command.Parameters.Add(wastageQtyParam);
                    command.Parameters.Add(wastagePercentageParam);
                    command.Parameters.Add(balanceQtyParam);

                    command.ExecuteNonQuery();
                    IsSuccess = true;
                }
            }

            return IsSuccess;
        }
        public Tuple<bool, bool> DeleteProductionInward(int productionInwardID)
        {
            bool IsSuccess = false;
            bool IsError = false;

            using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = StoredProcedure.DELETEPRODUCTIONINWARD;
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var productionInwardIDParam = command.CreateParameter();
                    productionInwardIDParam.ParameterName = "@ProductionInwardID";
                    productionInwardIDParam.Value = productionInwardID;

                    command.Parameters.Add(productionInwardIDParam);

                    command.ExecuteNonQuery();
                    IsSuccess = true;
                }
            }
            if (DeleteProductionConsumption(productionInwardID)) IsSuccess = true;
            else IsError = true;

            return Tuple.Create(IsSuccess, IsError);
        }
        public bool DeleteProductionConsumption(int productionConsumptionID)
        {
            bool IsSuccess = false;

            using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = StoredProcedure.DELETEPRODUCTIONCONSUMPTION;
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var productionConsumptionIDParam = command.CreateParameter();
                    productionConsumptionIDParam.ParameterName = "@ProductionConsumptionID";
                    productionConsumptionIDParam.Value = productionConsumptionID;

                    command.Parameters.Add(productionConsumptionIDParam);

                    command.ExecuteNonQuery();
                    IsSuccess = true;
                }
            }

            return IsSuccess;
        }
    }
}
