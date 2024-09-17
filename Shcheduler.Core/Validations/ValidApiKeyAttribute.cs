using System.ComponentModel.DataAnnotations;

namespace Shcheduler.Core.Validations
{
    public class ValidApiKeyAttribute : ValidationAttribute
    {
        public ValidApiKeyAttribute()
        {
            ErrorMessage = "Invalid API key.";
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            var apiKey = value.ToString();
            return apiKey == "123";
        }
    }
}