using LV.Foundation.AI.CustomCortexTagger.Settings.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
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

        private const string PropertyKey = "JToken";
        private const string PropertyValueKey = "_typeGroup";

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
            
            var categoryTemplate = new TemplateItem(Database.GetItem(new ID(tagsSettingsModel.TagCategoryTemplate)));
            if (categoryTemplate == null)
            {
                Sitecore.Diagnostics.Log.Warn($"CustomizableTaxonomyProvider: category template item with ID {tagsSettingsModel.TagCategoryTemplate} for tags not found", this);
                return new List<Tag>();
            }

            var template = new TemplateItem(Database.GetItem(new ID(tagsSettingsModel.TagEntryTemplate)));
            if (template == null)
            {
                Sitecore.Diagnostics.Log.Warn($"CustomizableTaxonomyProvider: template item with ID {tagsSettingsModel.TagEntryTemplate} for tags not found", this);
                return new List<Tag>();
            }

            var tagsRepositoryRootItem = Database.GetItem(new ID(tagsSettingsModel.TagsCollectionRootItem));
            if (tagsRepositoryRootItem == null)
            {
                Sitecore.Diagnostics.Log.Warn($"CustomizableTaxonomyProvider: tags root item with ID {tagsSettingsModel.TagsCollectionRootItem} not found", this);
                return new List<Tag>();
            }

            var tagFieldEntry = new ID(tagsSettingsModel.TagEntryValueField);
            if (tagFieldEntry == ID.Null || tagFieldEntry == ID.Undefined)
            {
                Sitecore.Diagnostics.Log.Warn($"CustomizableTaxonomyProvider: tag field with ID {tagsSettingsModel.TagsCollectionRootItem} not found", this);
                return new List<Tag>();
            }
            
            //var tagsWithCategories = new List<TagData>();
            var tagsWithoutCategories = new List<TagData>();
            var tagsCategories = new Dictionary<string, List<TagData>>();

            foreach (var tag in tagData)
            {
                if (!tag.Properties.Any(p =>
                    {
                        if (p.Key != CustomizableTaxonomyProvider.PropertyKey || p.Value == null)
                        {
                            return false;
                        }
                        
                        var jObject = p.Value as JObject;
                        if (jObject == null)
                        {
                            return false;
                        }

                        if (!jObject.ContainsKey(CustomizableTaxonomyProvider.PropertyValueKey))
                        {
                            return false;
                        }

                        var categoryName = jObject.GetValue(CustomizableTaxonomyProvider.PropertyValueKey);

                        //var valueStart = jObject.IndexOf('"', startIndex + CustomizableTaxonomyProvider.PropertyValueKey.Length + 1) + 1;
                        //if (valueStart < 0)
                        //{
                        //    return false;
                        //}

                        //jObject = jObject.Substring(valueStart + 1);

                        //var valueEnd = jObject.IndexOf('"');
                        //if (valueEnd < 0)
                        //{
                        //    return false;
                        //}

                        //jObject = jObject.Substring(0, valueEnd);
                        //if (string.IsNullOrWhiteSpace(jObject))
                        //{
                        //    return false;
                        //}

                        if (!tagsCategories.ContainsKey(categoryName.ToString()))
                        {
                            tagsCategories.Add(categoryName.ToString(), new List<TagData>());
                        }

                        tagsCategories[categoryName.ToString()].Add(tag);

                        return true;
                    }
                    ))
                {
                    tagsWithoutCategories.Add(tag);
                }
            }

            if (template.Fields.Select(x => x.ID).Contains(tagFieldEntry))
            {
                List<Tag> tagList = new List<Tag>();
                foreach (var tagCategory in tagsCategories)
                {                    
                    var categoryItem = PrepareNewCategory(tagCategory.Key, tagsRepositoryRootItem, categoryTemplate);

                    if (categoryItem != null)
                    {
                        foreach (var data in tagCategory.Value)
                        {
                            PrepareNewTag(template, tagFieldEntry, tagList, data, categoryItem);
                        }
                    }
                    else
                    {
                        tagsWithoutCategories.AddRange(tagCategory.Value);
                    }
                }

                foreach (var tagWithoutCategory in tagsWithoutCategories)
                {
                    PrepareNewTag(template, tagFieldEntry, tagList, tagWithoutCategory, tagsRepositoryRootItem);
                }

                return tagList;
            }
            else
            {
                Sitecore.Diagnostics.Log.Warn($"CustomTagger: template {tagsSettingsModel.TagEntryTemplate} doesn't contain field with ID {tagsSettingsModel.TagsCollectionRootItem}", this);
                return new List<Tag>();
            }
        }

        private Item PrepareNewCategory(string categoryName, Item tagsRepositoryRootItem, TemplateItem categoryTemplate)
        {
            var category = GetCategory(categoryName);
            if (category == ID.Null)
            {
                category = CreateCategory(categoryName, tagsRepositoryRootItem, categoryTemplate);
            }

            if (category != ID.Null)
            {
                return Database.GetItem(category);
            }

            return null;
        }


        protected virtual ID CreateCategory(string categoryName, Item tagsRepositoryRootItem, TemplateItem categoryTemplate)
        {
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                using (new SecurityDisabler())
                {
                    var categoryItem = tagsRepositoryRootItem.Add(categoryName, categoryTemplate);

                    categoryItem.Editing.BeginEdit();
                    categoryItem.Fields[Sitecore.FieldIDs.DisplayName].Value = categoryName;
                    categoryItem.Editing.EndEdit();

                    return categoryItem.ID;
                }
            }

            return ID.Null;
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
            
            return ID.Null;
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
            if (newTagId != ID.Null)
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