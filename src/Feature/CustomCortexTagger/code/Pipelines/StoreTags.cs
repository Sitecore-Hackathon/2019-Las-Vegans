using Sitecore.ContentTagging.Pipelines.TagContent;
using Sitecore.ContentTagging.Core.Models;
using System.Collections.Generic;
using Sitecore.ContentTagging.Core.Providers;
using LV.Feature.AI.CustomCortexTagger.Providers;

namespace LV.Feature.AI.CustomCortexTagger.Pipelines
{
    /// <summary>
    /// Pipeline additionally pass content item to taxonomy provider
    /// </summary>
    public class StoreTags
    {
        public void Process(TagContentArgs args)
        {
            List<Tag> list = new List<Tag>();
            foreach (ITaxonomyProvider taxonomyProvider in args.Configuration.TaxonomyProviders)
            {
                IEnumerable<Tag> tags;
                if (taxonomyProvider is CustomizableTaxonomyProvider)
                {
                    tags = ((CustomizableTaxonomyProvider)taxonomyProvider).CreateTags(args.ContentItem, args.TagDataCollection);
                }
                else
                {
                    tags = taxonomyProvider.CreateTags(args.TagDataCollection);
                }
                list.AddRange(tags);
            }
            args.Tags = list;
        }
    }
}