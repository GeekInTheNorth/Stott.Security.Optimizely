using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Stott.Optimizely.Csp.Common.Validation
{
    public class ValidationModel
    {
        public ValidationModel(ModelStateDictionary modelState)
        {
            Errors = modelState
                .Where(x => x.Value.ValidationState == ModelValidationState.Invalid)
                .Select(x => new ValidationItemModel(x.Key, x.Value))
                .ToList();
        }

        public ValidationModel(string propertyName, string errorMessage)
        {
            Errors = new List<ValidationItemModel> { new ValidationItemModel(propertyName, errorMessage) };
        }

        public List<ValidationItemModel> Errors { get; set; }
    }
}
