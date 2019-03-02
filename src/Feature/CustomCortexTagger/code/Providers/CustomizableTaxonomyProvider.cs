using LV.Foundation.AI.CustomCortexTagger.Settings.Services;
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
        //public string ProviderId => "CustomizableTaxonomyProvider";

        //private const string TagRepositoryId = "{154D56CC-0DE2-43C7-BBC0-A25BD7FFD901}";
        /// <summary>Tag repository</summary>
        //protected ITagRepository TagRepository;

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

        public IEnumerable<Tag> CreateTags(IEnumerable<TagData> tagData)
        {
            List<Tag> tagList = new List<Tag>();
            foreach (TagData data in tagData.Distinct(new TagNameComparer()))
            {
                Tag tag1 = this.GetTag(data.TagName);
                if (tag1 != null)
                {
                    tagList.Add(tag1);
                }
                else
                {
                    ID tag2 = this.CreateTag(data);
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
            return tagList;
        }

        protected virtual ID CreateTag(TagData data)
        {
            ////var tagRepository = ServiceLocator.ServiceProvider.GetService(ICustomTaggerSettingService);

            //TemplateItem template = new TemplateItem(CustomizableTaxonomyProvider.Database.GetItem(BucketConfigurationSettings.TagRepositoryId));
            //Item obj1 = Sitecore.Context.ContentDatabase.GetItem("{154D56CC-0DE2-43C7-BBC0-A25BD7FFD901}");
            //if (obj1 == null)
            //    return (ID)null;
            //string name = ItemUtil.ProposeValidItemName(this.RemoveDiacritics(data.TagName), "tag");
            //using (new SecurityDisabler())
            //{
            //    Item obj2 = obj1.Add(name, template);
            //    if (name != data.TagName)
            //    {
            //        obj2.Editing.BeginEdit();
            //        obj2.Fields["__Display Name"].Value = data.TagName;
            //        obj2.Editing.EndEdit();
            //    }
            //    return obj2.ID;
            //}

            return null;
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