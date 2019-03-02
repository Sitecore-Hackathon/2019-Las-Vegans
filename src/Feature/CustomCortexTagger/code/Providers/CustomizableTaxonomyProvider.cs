﻿using LV.Foundation.AI.CustomCortexTagger.Settings.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.ContentTagging.Core.Comparers;
using Sitecore.ContentTagging.Core.Models;
using Sitecore.ContentTagging.Core.Providers;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.DependencyInjection;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LV.Feature.AI.CustomCortexTagger.Providers
{
    public class CustomizableTaxonomyProvider : ITaxonomyProvider
    {
        private readonly ICustomTaggerSettingService _tagsSettingService;

        /// <summary>Database</summary>
        protected static Database Database
        {
            get
            {
                return Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;
            }
        }

        /// <summary>Provider id</summary>
        public string ProviderId
        {
            get
            {
                return nameof(DefaultTaxonomyProvider);
            }
        }

        public ICustomTaggerSettingService TagsSettingsService
        {
            get
            {
                return _tagsSettingService;
            }
        }

        public CustomizableTaxonomyProvider()
        {
            _tagsSettingService = ServiceProviderServiceExtensions.GetService<ICustomTaggerSettingService>(ServiceLocator.ServiceProvider);
        }

        public IEnumerable<Tag> CreateTags(IEnumerable<TagData> tagData)
        {
            var tagsSettingsModel = TagsSettingsService.GetCustomTaggerSettingModel();

            TemplateItem template = new TemplateItem(Database.GetItem(new ID(tagsSettingsModel.TagEntryTemplate)));
            if (template != null)
            {
                Item tagsRepositoryRootItem = Database.GetItem(new ID(tagsSettingsModel.TagsCollectionRootItem));
                if (tagsRepositoryRootItem == null)
                {
                    return null;
                }

                List<Tag> tagList = new List<Tag>();
                foreach (TagData data in tagData.Distinct(new TagNameComparer()))
                {
                    PrepareNewTag(template, tagsRepositoryRootItem, tagList, data);
                }
                return tagList;
            }
            return null;
        }

        private void PrepareNewTag(TemplateItem template, Item tagsRepositoryRootItem, List<Tag> tagList, TagData data)
        {
            Tag tag1 = this.GetTag(data.TagName);
            if (tag1 != null)
            {
                tagList.Add(tag1);
            }
            else
            {
                ID tag2 = this.CreateTag(data, template, tagsRepositoryRootItem);
                if (!(tag2 == (ID)null))
                {
                    Tag tag3 = new Tag()
                    {
                        TagName = data.TagName,
                        ID = tag2.ToString(),
                        TaxonomyProviderId = this.ProviderId,
                        Data = data
                    };
                    tagList.Add(tag3);
                }
            }
        }

        protected virtual ID CreateTag(TagData data, TemplateItem template, Item tagsRepositoryRootItem)
        {
            string name = ItemUtil.ProposeValidItemName(this.RemoveDiacritics(data.TagName), "tag");
            using (new SecurityDisabler())
            {
                Item tagItem = tagsRepositoryRootItem.Add(name, template);
                if (name != data.TagName)
                {
                    tagItem.Editing.BeginEdit();
                    tagItem.Fields["__Display Name"].Value = data.TagName;
                    tagItem.Editing.EndEdit();
                }
                return tagItem.ID;
            }
        }

        public Tag GetTag(string tagId)
        {
            // receive tag from repository based on tagId here
            Tag tag = null;
            return tag;
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