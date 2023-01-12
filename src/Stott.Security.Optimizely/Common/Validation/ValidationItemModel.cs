namespace Stott.Security.Optimizely.Common.Validation;

using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.ModelBinding;

public sealed class ValidationItemModel
{
    public ValidationItemModel(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }

    public ValidationItemModel(string propertyName, ModelStateEntry? modelStateEntry)
    {
        PropertyName = propertyName;

        var errorMessages = modelStateEntry?.Errors.Select(x => x.ErrorMessage) ?? new List<string>(0);
        ErrorMessage = string.Join(". ", errorMessages);
    }

    public string PropertyName { get; set; }

    public string ErrorMessage { get; set; }
}
