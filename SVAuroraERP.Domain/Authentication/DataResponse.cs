namespace SVAuroraERP.Domain.Authentication
{
    public class DataResponse
    {
        public bool Error { get; set; } = false;
        public bool Success { get; set; } = true;
        public string? Message { get; set; } = string.Empty;
        public Object? Value { get; set; }
        public int ID { get; set; } = 0;
        public int Count { get; set; } = 0;
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
    }
}