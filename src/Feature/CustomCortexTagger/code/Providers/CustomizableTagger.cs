using Sitecore.ContentTagging.Core.Models;
using Sitecore.ContentTagging.Core.Providers;
using Sitecore.Data;
using Sitecore.Data.Fields;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LV.Feature.AI.CustomCortexTagger.Providers
{
    public class CustomizableTagger : ITagger<Sitecore.Data.Items.Item>
    {
        public void TagContent(Sitecore.Data.Items.Item contentItem, IEnumerable<Tag> tags)
        {
            var semanticField = (MultilistField)contentItem.Fields["__Semantics"];
            contentItem.Editing.BeginEdit();
            foreach (var tag in tags)
            {
                if (ID.TryParse(tag.ID, out ID id) && !semanticField.TargetIDs.Contains(id))
                {
                    semanticField.Add(tag.ID);
                }
            }
            contentItem.Editing.EndEdit();
        }
    }
}