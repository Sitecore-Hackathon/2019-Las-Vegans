using LV.Foundation.CustomTagger.Models;
using LV.Foundation.DependencyInjection;

namespace LV.Foundation.CustomTagger.Services
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