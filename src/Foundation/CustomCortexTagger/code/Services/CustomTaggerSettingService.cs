using LV.Foundation.AI.CustomCortexTagger.Settings.Models;
using LV.Foundation.DependencyInjection;
using Sitecore.Data.Items;
using Sitecore.Data;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Services
{
    [Service]
    public class CustomTaggerSettingService : ICustomTaggerSettingService
    {
        //TODO: change to multi-site solution
        private readonly ID CustomTaggerSettingsItemId = new ID("{82239F2F-D096-4DB4-A6B5-776B210D47F9}");
        private readonly Item CustomTaggerSettingsItem;

        public CustomTaggerSettingService()
        {
            this.CustomTaggerSettingsItem = Sitecore.Context.Database.GetItem(this.CustomTaggerSettingsItemId);
        }

        public ICustomTaggerSettingModel GetCustomTaggerSettingModel()
        {
            var result = new CustomTaggerSettingModel(this.CustomTaggerSettingsItem);

            return result;
        }
    }
}