using System;

namespace LV.Foundation.CustomTagger.Models
{
    public class CustomTaggerSettingModel : ICustomTaggerSettingModel
    {
        public Guid TagsCollectionRootItem { get; set; }

        public Guid TagEntryTemplate { get; set; }

        public Guid TagValueField { get; set; }

        public Guid TagsFieldTarget { get; set; }
    }
}