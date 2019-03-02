using System.Collections.Generic;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Models
{
    public interface ICustomTaggerSitesMappingsModel
    {
        IList<ICustomTaggerSiteMappingModel> CustomTaggerSitesMappings { get; }
    }
}