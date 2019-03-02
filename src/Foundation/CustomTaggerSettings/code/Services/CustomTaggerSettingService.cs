using LV.Foundation.AI.CustomCortexTagger.Settings.Models;
using Sitecore.Data.Items;
using Sitecore.Data;
using System.Linq;
using Sitecore.Web;
using System;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Services
{
    public class CustomTaggerSettingService : ICustomTaggerSettingService
    {
        private readonly ID _defaultCustomTaggerSettingsItemId = new ID("{82239F2F-D096-4DB4-A6B5-776B210D47F9}");

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

        public ICustomTaggerSettingModel GetCustomTaggerSettingModel(Item contentItem = null)
        {
            Item customTaggerSettingsItem = null;
            var siteName = GetSite(contentItem)?.Name;

            if (!string.IsNullOrWhiteSpace(siteName))
            {
                var xmlNode = Sitecore.Configuration.Factory.GetConfigNode("customTagger");
                var sitesMappings = Sitecore.Configuration.Factory.CreateObject<ICustomTaggerSitesMappingsModel>(xmlNode);
                var site = sitesMappings.CustomTaggerSitesMappings.FirstOrDefault(m => m.Name.Equals(siteName));

                if (site != null && !string.IsNullOrWhiteSpace(site.SettingsItemPath))
                {
                    customTaggerSettingsItem = Database.GetItem(site.SettingsItemPath);
                }
            }

            if (customTaggerSettingsItem == null)
            {
                customTaggerSettingsItem = Database.GetItem(_defaultCustomTaggerSettingsItemId);
            }

            return new CustomTaggerSettingModel(customTaggerSettingsItem);
        }

        private SiteInfo GetSite(Item item)
        {
            var siteInfoList = Sitecore.Configuration.Factory.GetSiteInfoList();
            SiteInfo currentSiteinfo = null;
            var matchLength = 0;
            foreach (var siteInfo in siteInfoList)
            {
                if (siteInfo.Database == "core" || siteInfo.Name == "modules_website")
                {
                    continue;
                }
                if (item.Paths.FullPath.StartsWith(siteInfo.RootPath, StringComparison.OrdinalIgnoreCase) && siteInfo.RootPath.Length > matchLength)
                {
                    matchLength = siteInfo.RootPath.Length;
                    currentSiteinfo = siteInfo;
                }
            }
            return currentSiteinfo;
        }
    }
}