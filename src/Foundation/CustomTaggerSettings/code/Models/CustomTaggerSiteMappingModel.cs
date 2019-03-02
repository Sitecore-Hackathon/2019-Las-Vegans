namespace LV.Foundation.AI.CustomCortexTagger.Settings.Models
{
    public class CustomTaggerSiteMappingModel : ICustomTaggerSiteMappingModel
    {
        public string Name { get; set; }

        public string SettingsItemPath { get; set; }

        public CustomTaggerSiteMappingModel()
        {

        }

        public CustomTaggerSiteMappingModel(string name, string settingsItemPath)
        {
            this.Name = name;
            this.SettingsItemPath = settingsItemPath;
        }
    }
}