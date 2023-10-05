using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO
{
    public class TrimStringModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // First check if request validation is required
            var shouldPerformRequestValidation = controllerContext.Controller.ValidateRequest &&
                bindingContext.ModelMetadata.RequestValidationEnabled;

            // determine if the value provider is IUnvalidatedValueProvider, if it is, pass in the 
            // flag to perform request validation (e.g. [AllowHtml] is set on the property)
            var unvalidatedProvider = bindingContext.ValueProvider as IUnvalidatedValueProvider;

            var valueProviderResult = unvalidatedProvider?.GetValue(bindingContext.ModelName, !shouldPerformRequestValidation) ??
                bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            return valueProviderResult?.AttemptedValue?.Trim();
        }
    }
}