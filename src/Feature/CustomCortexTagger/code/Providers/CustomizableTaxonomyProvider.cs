using LV.Foundation.AI.CustomCortexTagger.Settings.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentTagging.Core.Comparers;
using Sitecore.ContentTagging.Core.Models;
using Sitecore.ContentTagging.Core.Providers;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.SecurityModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LV.Feature.AI.CustomCortexTagger.Providers
{
    public class CustomizableTaxonomyProvider : ITaxonomyProvider
    {
        private readonly ICustomTaggerSettingService _tagsSettingService;

        protected static Database Database => Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;

        public string ProviderId => nameof(DefaultTaxonomyProvider);

        public CustomizableTaxonomyProvider()
        {
            _tagsSettingService = ServiceProviderServiceExtensions.GetService<ICustomTaggerSettingService>(ServiceLocator.ServiceProvider);
        }

        public IEnumerable<Tag> CreateTags(IEnumerable<TagData> tagData)
        {
            return CreateTags(null, tagData);
        }

        public IEnumerable<Tag> CreateTags(Item contentItem, IEnumerable<TagData> tagData)
        {
            var tagsSettingsModel = _tagsSettingService.GetCustomTaggerSettingModel(contentItem);

            var template = new TemplateItem(Database.GetItem(new ID(tagsSettingsModel.TagEntryTemplate)));
            if (template == null)
            {
                Sitecore.Diagnostics.Log.Warn($"CustomTagger: template item with ID {tagsSettingsModel.TagEntryTemplate} for tags not found", this);
                return null;
            }

            var tagsRepositoryRootItem = Database.GetItem(new ID(tagsSettingsModel.TagsCollectionRootItem));
            if (tagsRepositoryRootItem == null)
            {
                Sitecore.Diagnostics.Log.Warn($"CustomTagger: tags root item with ID {tagsSettingsModel.TagsCollectionRootItem} not found", this);
                return null;
            }

            var tagFieldEntry = new ID(tagsSettingsModel.TagEntryValueField);
            if (tagFieldEntry == ID.Null || tagFieldEntry == ID.Undefined)
            {
                Sitecore.Diagnostics.Log.Warn($"CustomTagger: tag field with ID {tagsSettingsModel.TagsCollectionRootItem} not found", this);
                return null;
            }

            if (template.Fields.Select(x => x.ID).Contains(tagFieldEntry))
            {
                List<Tag> tagList = new List<Tag>();
                foreach (TagData data in tagData.Distinct(new TagNameComparer()))
                {
                    PrepareNewTag(template, tagsRepositoryRootItem, tagFieldEntry, tagList, data);
                }
                return tagList;
            }
            else
            {
                Sitecore.Diagnostics.Log.Warn($"CustomTagger: template {tagsSettingsModel.TagEntryTemplate} doesn't contain field with ID {tagsSettingsModel.TagsCollectionRootItem}", this);
                return null;
            }
        }

        private void PrepareNewTag(TemplateItem template, Item tagsRepositoryRootItem, ID tagFieldEntry, List<Tag> tagList, TagData data)
        {
            var existingTag = GetTag(data.TagName);
            if (existingTag != null)
            {
                tagList.Add(existingTag);
                return;
            }
            var newTagId = CreateTag(data, template, tagsRepositoryRootItem, tagFieldEntry);
            if (newTagId != (ID)null)
            {
                var tag = new Tag()
                {
                    TagName = data.TagName,
                    ID = newTagId.ToString(),
                    TaxonomyProviderId = ProviderId,
                    Data = data
                };
                tagList.Add(tag);
            }
        }

        protected virtual ID CreateTag(TagData data, TemplateItem template, Item tagsRepositoryRootItem, ID tagFieldEntry)
        {
            string name = ItemUtil.ProposeValidItemName(RemoveDiacritics(data.TagName), "tag");
            using (new SecurityDisabler())
            {
                var tagItem = tagsRepositoryRootItem.Add(name, template);
                if (name != data.TagName)
                {
                    tagItem.Editing.BeginEdit();
                    tagItem.Fields[Sitecore.FieldIDs.DisplayName].Value = data.TagName;
                    tagItem.Fields[tagFieldEntry].Value = data.TagName;
                    tagItem.Editing.EndEdit();
                }
                return tagItem.ID;
            }
        }

        public Tag GetTag(string tagName)
        {
            using (var context = ContentSearchManager.GetIndex($"sitecore_{Database.Name}_index").CreateSearchContext())
            {
                var searchQuery = context
                    .GetQueryable<SearchResultItem>()
                    .Where(item => item.Name == ItemUtil.ProposeValidItemName(RemoveDiacritics(tagName), "tag"));

                if(searchQuery.Any())
                {
                    return new Tag() { TagName = searchQuery.FirstOrDefault()?.Name } ;
                }
            }
            
            return null;
        }

        public Tag GetParent(string tagId)
        {
            return null;
        }

        public IEnumerable<Tag> GetChildren(string tagId)
        {
            return null;
        }

        protected virtual string RemoveDiacritics(string s)
        {
            return Encoding.ASCII.GetString(Encoding.GetEncoding(1251).GetBytes(s));
        }
    }
}