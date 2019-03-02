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

            var tagsCategories = tagData.SelectMany(x => x.Properties.Where(y => y.Key == "_typeGroup" && y.Value != null).Select(z => z.Value as string).Distinct());
            
            if (tagsCategories == null || !tagsCategories.Any())
            {
                Sitecore.Diagnostics.Log.Warn($"CustomizableTaxonomyProvider: Tags categories are null or empty", this);
                return null;
            }

            var tagsSettingsModel = _tagsSettingService.GetCustomTaggerSettingModel();

            var template = new TemplateItem(Database.GetItem(new ID(tagsSettingsModel.TagEntryTemplate)));
            if (template == null)
            {
                Sitecore.Diagnostics.Log.Warn($"CustomizableTaxonomyProvider: template item with ID {tagsSettingsModel.TagEntryTemplate} for tags not found", this);
                return null;
            }

            var tagsRepositoryRootItem = Database.GetItem(new ID(tagsSettingsModel.TagsCollectionRootItem));
            if (tagsRepositoryRootItem == null)
            {
                Sitecore.Diagnostics.Log.Warn($"CustomizableTaxonomyProvider: tags root item with ID {tagsSettingsModel.TagsCollectionRootItem} not found", this);
                return null;
            }

            var tagFieldEntry = new ID(tagsSettingsModel.TagEntryValueField);
            if (tagFieldEntry == ID.Null || tagFieldEntry == ID.Undefined)
            {
                Sitecore.Diagnostics.Log.Warn($"CustomizableTaxonomyProvider: tag field with ID {tagsSettingsModel.TagsCollectionRootItem} not found", this);
                return null;
            }

            if (template.Fields.Select(x => x.ID).Contains(tagFieldEntry))
            {
                List<Tag> tagList = new List<Tag>();
                foreach (var tagCategory in tagsCategories)
                {
                    var tagsForCategory = tagData.Distinct(new TagNameComparer())
                        .Where(x => x.Properties.Any(p => p.Key == "_typeGroup" && p.Value != null && p.Value as string == tagCategory));
                    
                    var categoryItem = PrepareNewCategory(tagCategory, tagsRepositoryRootItem);

                    foreach (var data in tagsForCategory)
                    {
                        PrepareNewTag(template, tagFieldEntry, tagList, data, categoryItem);
                    }
                }

                var tagsWithoutCategory = tagData.Distinct(new TagNameComparer())
                        .Where(x => x.Properties.All(p => (p.Key != "_typeGroup") || (p.Value == null || !tagsCategories.Contains(p.Value))));

                foreach (var tagWithoutCategory in tagsWithoutCategory)
                {
                    PrepareNewTag(template, tagFieldEntry, tagList, tagWithoutCategory, tagsRepositoryRootItem);
                }

                return tagList;
            }
            else
            {
                Sitecore.Diagnostics.Log.Warn($"CustomTagger: template {tagsSettingsModel.TagEntryTemplate} doesn't contain field with ID {tagsSettingsModel.TagsCollectionRootItem}", this);
                return null;
            }
        }

        private Item PrepareNewCategory(string categoryName, Item tagsRepositoryRootItem)
        {
            var existingCategory = GetCategory(categoryName);
            if (existingCategory == (ID)null)
            {
                return null;
            }

            var newCategory = CreateCategory(categoryName, tagsRepositoryRootItem);
            var categoryItem = Database.GetItem(newCategory);

            return categoryItem;
        }


        protected virtual ID CreateCategory(string categoryName, Item tagsRepositoryRootItem)
        {
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                using (new SecurityDisabler())
                {
                    var categoryItem = tagsRepositoryRootItem.Add(categoryName, template);

                    categoryItem.Editing.BeginEdit();
                    categoryItem.Fields[Sitecore.FieldIDs.DisplayName].Value = categoryName;
                    categoryItem.Editing.EndEdit();

                    return categoryItem.ID;
                }
            }
        }

        public ID GetCategory(string categoryName)
        {
            using (var context = ContentSearchManager.GetIndex($"sitecore_{Database.Name}_index").CreateSearchContext())
            {
                var searchQuery = context
                    .GetQueryable<SearchResultItem>()
                    .Where(item => item.Name == categoryName);

                if(searchQuery.Any())
                {
                    return searchQuery.FirstOrDefault().ItemId;
                }
            }
            
            return null;
        }

        private void PrepareNewTag(TemplateItem template, ID tagFieldEntry, List<Tag> tagList, TagData data, Item parentItem)
        {
            var existingTag = GetTag(data.TagName);
            if (existingTag != null)
            {
                tagList.Add(existingTag);
                return;
            }
            var newTagId = CreateTag(data, template, tagFieldEntry, parentItem);
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

        protected virtual ID CreateTag(TagData data, TemplateItem template, ID tagFieldEntry, Item parentItem)
        {
            string name = ItemUtil.ProposeValidItemName(RemoveDiacritics(data.TagName), "tag");
            using (new SecurityDisabler())
            {
                var tagItem = parentItem.Add(name, template);
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

                if (searchQuery.Any())
                {
                    return new Tag() { TagName = searchQuery.FirstOrDefault()?.Name };
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