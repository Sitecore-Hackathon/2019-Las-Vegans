using LV.Foundation.AI.CustomCortexTagger.Settings.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.ContentTagging.Core.Models;
using Sitecore.ContentTagging.Core.Providers;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LV.Feature.AI.CustomCortexTagger.Providers
{
    public class CustomizableTagger : ITagger<Item>
    {
        private readonly ICustomTaggerSettingService _settingsService;

        public CustomizableTagger()
        {
            _settingsService = ServiceProviderServiceExtensions.GetService<ICustomTaggerSettingService>(ServiceLocator.ServiceProvider);
        }

        public void TagContent(Item contentItem, IEnumerable<Tag> tags)
        {
            var tagsFieldId = _settingsService.GetCustomTaggerSettingModel("website").TagsFieldTarget;
            if (tagsFieldId == Guid.Empty)
            {
                Log.Warn("CustomTagger: Tags field name not defined in settings", this);
                return;
            }

            var tagsField = (MultilistField)contentItem.Fields[new ID(tagsFieldId)];
            if (tagsField == null)
            {
                Log.Warn($"CustomTagger: Field {tagsFieldId} not found or wrong type in item {contentItem.ID}", this);
                return;
            }

            contentItem.Editing.BeginEdit();
            foreach (var tag in tags)
            {
                if (ID.TryParse(tag.ID, out ID id) && !tagsField.TargetIDs.Contains(id))
                {
                    tagsField.Add(tag.ID);
                }
            }
            contentItem.Editing.EndEdit();
        }
    }
}