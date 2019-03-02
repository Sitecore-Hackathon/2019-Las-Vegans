using System;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Models
{
    public interface ICustomTaggerSettingModel
    {
        Guid TagsCollectionRootItem { get; }

        Guid TagEntryTemplate { get; }

        Guid TagEntryValueField { get; }

        Guid TagsFieldTarget { get; }
    }
}