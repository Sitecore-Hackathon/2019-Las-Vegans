using LV.Foundation.AI.CustomCortexTagger.Settings.Models;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Services
{
    public interface ICustomTaggerSettingService
    {
        ICustomTaggerSettingModel GetCustomTaggerSettingModel(string siteName);
    }
}