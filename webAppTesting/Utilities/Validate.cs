using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace webApp.Test.Utilities
{
    public static class Validate
    {
        public static IList<ValidationResult> Model(object model)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(model);

            Validator.TryValidateObject(model, validationContext, validationResults, true);

            return validationResults;
        }
    }
}
