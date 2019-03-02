using Sitecore.Pipelines.GetLookupSourceItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LV.Foundation.CustomTagger.Processors
{
    public class ProcessDependentUponSource
    {
        private const string DependentUponTag = "@dependentupon";

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

            return string.Format($"query://*[@@id='{itemId}']");
        }
    }
}