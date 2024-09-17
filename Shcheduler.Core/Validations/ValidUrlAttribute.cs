using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Shcheduler.Core.Validations
{
    public class ValidUrlAttribute : ValidationAttribute
    {
        public ValidUrlAttribute()
        {
            ErrorMessage = "Invalid URL format.";
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            var urlString = value.ToString();
            var pattern = @"^(http|https):\/\/([a-zA-Z0-9\-\.]+(:[a-zA-Z0-9]*)?@)?([a-zA-Z0-9\-\.]+)(:[0-9]{1,5})?(\/.*)?$";
            return Regex.IsMatch(urlString, pattern);
        }
    }
}