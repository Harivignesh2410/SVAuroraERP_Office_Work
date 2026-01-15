using Microsoft.Extensions.Options;

namespace SVAuroraERP.WebUI.Custom
{
    public class AppVersionService
    {
        private readonly ApplicationSettings _appSettings;

        public AppVersionService(IOptions<ApplicationSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string GetAppVersion()
        {
            return _appSettings.AppVersion;
        }
        public string GetBuildDate()
        {
            var buildDate = _appSettings.BuildDate;
            return buildDate.ToString($"d'{GetOrdinal(buildDate.Day)}' MMMM yyyy 'at' hh:mm tt");
        }
        public string GetAppEdition()
        {
            return _appSettings.AppEdition;
        }
        private string GetOrdinal(int day)
        {
            if (day >= 11 && day <= 13) return "th"; // Special case for 11th, 12th, 13th

            return (day % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            };
        }
    }
}