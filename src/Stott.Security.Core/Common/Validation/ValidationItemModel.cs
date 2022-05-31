using System.Linq;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Stott.Security.Core.Common.Validation
{
    public class ValidationItemModel
    {
        public ValidationItemModel(string propertyName, string errorMessage)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }

        public ValidationItemModel(string propertyName, ModelStateEntry modelStateEntry)
        {
            PropertyName = propertyName;
            ErrorMessage = string.Join(". ", modelStateEntry.Errors.Select(x => x.ErrorMessage));
        }

        public string PropertyName { get; set; }

        public string ErrorMessage { get; set; }
    }
}
