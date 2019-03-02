﻿using System;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Models
{
    public interface ICustomTaggerSettingModel
    {
        Guid TagsCollectionRootItem { get; set; }

        Guid TagEntryTemplate { get; set; }

        Guid TagValueField { get; set; }

        Guid TagsFieldTarget { get; set; }
    }
}