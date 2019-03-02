using LV.Foundation.AI.CustomCortexTagger.Settings.Models;
using Sitecore.Data.Items;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Services
{
    public interface ICustomTaggerSettingService
    {
        ICustomTaggerSettingModel GetCustomTaggerSettingModel(Item contentItem = null);
    }
}