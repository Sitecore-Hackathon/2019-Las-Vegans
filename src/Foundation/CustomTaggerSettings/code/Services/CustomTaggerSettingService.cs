using LV.Foundation.AI.CustomCortexTagger.Settings.Models;
using Sitecore.Data.Items;
using Sitecore.Data;
using System.Linq;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Services
{
    public class CustomTaggerSettingService : ICustomTaggerSettingService
    {
        private readonly ID DefaultCustomTaggerSettingsItemId = new ID("{82239F2F-D096-4DB4-A6B5-776B210D47F9}");

        public CustomTaggerSettingService()
        {
        }

        private Database Database
        {
            get
            {
                return Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;
            }
        }

        public ICustomTaggerSettingModel GetCustomTaggerSettingModel(string siteName)
        {
            Item customTaggerSettingsItem = null;

            if (!string.IsNullOrWhiteSpace(siteName))
            {
                var xmlNode = Sitecore.Configuration.Factory.GetConfigNode("customTagger");
                var sitesMappings = Sitecore.Configuration.Factory.CreateObject<ICustomTaggerSitesMappingsModel>(xmlNode);
                var site = sitesMappings.CustomTaggerSitesMappings.FirstOrDefault(m => m.Name.Equals(siteName));

                if (site != null && !string.IsNullOrWhiteSpace(site.SettingsItemPath))
                {
                    customTaggerSettingsItem = this.Database.GetItem(site.SettingsItemPath);
                }
            }

            if (customTaggerSettingsItem == null)
            {
                customTaggerSettingsItem = this.Database.GetItem(this.DefaultCustomTaggerSettingsItemId);
            }

            return new CustomTaggerSettingModel(customTaggerSettingsItem);
        }
    }
}