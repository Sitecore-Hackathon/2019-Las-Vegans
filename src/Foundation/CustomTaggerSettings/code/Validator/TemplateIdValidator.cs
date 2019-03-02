using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Validators;
using Sitecore.Diagnostics;
using System;
using System.Runtime.Serialization;

namespace LV.Foundation.AI.CustomCortexTagger.Settings.Validator
{
    [Serializable]
    public class TemplateIdValidator : StandardValidator
    {
        public override string Name
        {
            get
            {
                return "TemplateIdValidator";
            }
        }
        
        public TemplateIdValidator(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public TemplateIdValidator()
        {
        }

        protected override ValidatorResult Evaluate()
        {
            var templateId = base.Parameters["TemplateId"];
            if (string.IsNullOrWhiteSpace(templateId))
            {
                return ValidatorResult.Valid;
            }

            Item item = this.GetItem();
            if (item == null)
            {
                return ValidatorResult.Valid;
            }

            var controlValidationValue = this.ControlValidationValue;
            if (string.IsNullOrEmpty(controlValidationValue))
            {
                return ValidatorResult.Valid;
            }

            var fieldName = base.GetField().Title;


            Guid value;
            if (!Guid.TryParse(controlValidationValue, out value))
            {
                base.Text = this.GetText("Field \"{0}\" contains incorrect value", new string[]
                {
                    //string.IsNullOrWhiteSpace(fieldName)? base.GetField().Name : fieldName
                    base.GetFieldDisplayName()
                });
                return ValidatorResult.FatalError;
            }

            var selectedItem = this.GetItem()?.Database?.GetItem(new ID(value));


            if (selectedItem != null && selectedItem.TemplateID == new ID(templateId))
            {
                return ValidatorResult.Valid;
            }

            base.Text = this.GetText("Field \"{0}\" contains incorrect value", new string[]
            {
                    //string.IsNullOrWhiteSpace(fieldName)? base.GetField().Name : fieldName
                    base.GetFieldDisplayName()
            });
            return base.GetFailedResult(ValidatorResult.CriticalError);
        }

        protected override ValidatorResult GetMaxValidatorResult()
        {
            return base.GetFailedResult(ValidatorResult.CriticalError);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Assert.ArgumentNotNull(info, "info");
            info.AddValue("Name", this.Name);
            base.GetObjectData(info, context);
        }
    }
}