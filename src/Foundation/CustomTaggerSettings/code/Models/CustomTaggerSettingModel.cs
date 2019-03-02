using System;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Models
{
    public class CustomTaggerSettingModel : ICustomTaggerSettingModel
    {
        private readonly ID _tagsCollectionRootItemFieldId = new ID("{EECFD4D2-D64C-4214-8150-CFE9C491D779}");
        private readonly Guid _tagsCollectionRootItem;

        private readonly ID _tagEntryTemplateFieldId = new ID("{5A4DE458-E557-4DE1-B185-FC5EF282E3C8}");
        private readonly Guid _tagEntryTemplate;

        private readonly ID _tagEntryValueFieldFieldId = new ID("{6D18469E-2395-4735-8EB1-84C06E3CDCCA}");
        private readonly Guid _tagEntryValueField;

        private readonly ID _tagsFieldTargetFieldId = new ID("{55DD19B5-9143-4CA1-9ABB-AA4826F60031}");
        private readonly Guid _tagsFieldTarget;

        public Guid TagsCollectionRootItem => this._tagsCollectionRootItem;

        public Guid TagEntryTemplate => this._tagEntryTemplate;

        public Guid TagEntryValueField => this._tagEntryValueField;

        public Guid TagsFieldTarget => this._tagsFieldTarget;

        public CustomTaggerSettingModel(Item customTaggerSettingsItem)
        {
            Assert.IsNotNull(customTaggerSettingsItem, "customTaggerSettingsItem");

            var tagsCollectionRootItemField = customTaggerSettingsItem.Fields[this._tagsCollectionRootItemFieldId]?.GetValue(true);
            var tagEntryTemplateField = customTaggerSettingsItem.Fields[this._tagEntryTemplateFieldId]?.GetValue(true);
            var tagEntryValueFieldField = customTaggerSettingsItem.Fields[this._tagEntryValueFieldFieldId]?.GetValue(true);
            var tagsFieldTargetField = customTaggerSettingsItem.Fields[this._tagsFieldTargetFieldId]?.GetValue(true);

            Assert.IsTrue(Guid.TryParse(tagsCollectionRootItemField, out this._tagsCollectionRootItem), "");
            Assert.IsTrue(Guid.TryParse(tagEntryTemplateField, out this._tagEntryTemplate), "");
            Assert.IsTrue(Guid.TryParse(tagEntryValueFieldField, out this._tagEntryValueField), "");
            Assert.IsTrue(Guid.TryParse(tagsFieldTargetField, out this._tagsFieldTarget), "");
        }
    }
}