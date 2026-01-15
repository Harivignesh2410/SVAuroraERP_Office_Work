//Added on 2025/05/24 by HARIVIGNESH
namespace SVAuroraERP.Domain.Authentication
{
    public class PagePermissions
    {
        public byte PageControlID { get; set; }
        public bool HasAccess { get; set; }
        public bool HasAdd { get; set; }
        public bool HasEdit { get; set; }
        public bool HasDelete { get; set; }
        public bool HasView { get; set; }
        public bool HasExport { get; set; }
    }
}
