using LV.Foundation.AI.CustomCortexTagger.Settings.Models;
using LV.Foundation.DependencyInjection;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Services
{
    [Service]
    public class CustomTaggerSettingService
    {
        public CustomTaggerSettingService()
        {

        }

        public ICustomTaggerSettingModel GetCustomTaggerSettingModel()
        {
            return new CustomTaggerSettingModel();
        }
    }
}