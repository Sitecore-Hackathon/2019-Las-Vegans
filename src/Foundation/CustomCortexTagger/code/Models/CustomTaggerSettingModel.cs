using System;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Models
{
    public class CustomTaggerSettingModel : ICustomTaggerSettingModel
    {
        public Guid TagsCollectionRootItem { get; set; }

        public Guid TagEntryTemplate { get; set; }

        public Guid TagValueField { get; set; }

        public Guid TagsFieldTarget { get; set; }
    }
}