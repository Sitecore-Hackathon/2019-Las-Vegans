using System.Collections.Generic;
using System.Xml;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Models
{
    public class CustomTaggerSitesMappingsModel : ICustomTaggerSitesMappingsModel
    {
        private const string SiteNameAttribute = "name";

        private const string SettingsAttribute = "settingsItemPath";

        public IList<ICustomTaggerSiteMappingModel> CustomTaggerSitesMappings { get; }

        public CustomTaggerSitesMappingsModel() : this(new List<ICustomTaggerSiteMappingModel>()) { }

        public CustomTaggerSitesMappingsModel(IList<ICustomTaggerSiteMappingModel> customTaggerSitesMappings)
        {
            this.CustomTaggerSitesMappings = customTaggerSitesMappings;
        }

        protected void AddCustomTaggerSitesMappings(XmlNode node)
        {
            if (node?.Attributes[CustomTaggerSitesMappingsModel.SiteNameAttribute]?.Value == null || 
                node?.Attributes[CustomTaggerSitesMappingsModel.SettingsAttribute]?.Value == null)
                return;

            this.CustomTaggerSitesMappings.Add(new CustomTaggerSiteMappingModel()
            {
                Name = node.Attributes[CustomTaggerSitesMappingsModel.SiteNameAttribute].Value,
                SettingsItemPath = node.Attributes[CustomTaggerSitesMappingsModel.SettingsAttribute].Value
            });
        }
    }
}