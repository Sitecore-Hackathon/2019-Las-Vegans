namespace LV.Foundation.AI.CustomCortexTagger.Settings.Models
{
    public interface ICustomTaggerSiteMappingModel
    {
        string Name { get; set; }

        string SettingsItemPath { get; set; }
    }
}