using System.Collections;
using System.Globalization;
using System.Resources;

namespace Inovatec.OfficeManagementTool.Localization
{
    public class LocalizationManager
    {
        public static object GetJsonResources(string lang)
        {
            var resourceDic = new Dictionary<string, object>();

            AddResourceToDictionary(resourceDic, lang);

            return resourceDic;
        }
        private static string ResourceBaseName(string resource,string lang)
        {
            string cultureName = lang switch
            {
                "sr" => "sr-Latn-RS",
                "en" => "en-US",
                _ => "en-US"
            };

            return $"Inovatec.OfficeManagementTool.Localization.Resources.{resource}.{resource}-{cultureName}";
        }
        private static void AddResourceToDictionary(Dictionary<string, object> resourceDic, string lang)
        {
            List<string> resources = new List<string>
            {
                "Common",
                "Routes",
                "PrimeNG",
                "UserAdministration",
                "CategoryAdministration",
                "ProductAdministration",
                "OrderAdministration",
                "Comments",
                "Equipment",
                "LogsAdministration",
                "ReportAdministration",
                "OfficeAdministration",
                "Notification",
                "ScheduleAdministration",
                "SupplierAdministration"
            };

            ResourceManager rm = null;

            foreach (string resource in resources)
            {
                rm = new ResourceManager(ResourceBaseName(resource,lang),
                    typeof(LocalizationManager).Assembly);

                var rs = rm.GetResourceSet(CultureInfo.CreateSpecificCulture(lang), true, true);

                var dic = rs.Cast<DictionaryEntry>().ToDictionary(i => i.Key.ToString(), i => i.Value.ToString());

                resourceDic.Add(resource, dic);
            }
        }
    }
}