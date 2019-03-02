using Sitecore.Pipelines.GetLookupSourceItems;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Processors
{
    public class ProcessDependentUponSource
    {
        private const string DependentUponTag = "@dependentupon";
        private const string TemplateFieldId = "{455A3E98-A627-4B40-8035-E683A0331AC7}";

        public void Process(GetLookupSourceItemsArgs args)
        {
            if (!args.Source.StartsWith(ProcessDependentUponSource.DependentUponTag))
            {
                return;
            }
            
            var fieldName = this.GetFieldName(args.Source);

            args.Source = this.GetDataSource(args.Item[fieldName]);
        }

        private string GetFieldName(string source)
        {
            var result = string.Empty;

            result = source.Replace(ProcessDependentUponSource.DependentUponTag, string.Empty); //["name_of_field"]

            return result.Substring(2, result.Length - 4);
        }

        private string GetDataSource(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return string.Empty;
            }

            return $"query://*[@@id='{itemId}']/*/*[@@templateid='{ProcessDependentUponSource.TemplateFieldId}']";
        }
    }
}